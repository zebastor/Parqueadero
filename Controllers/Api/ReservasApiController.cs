using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;
using System.Text.RegularExpressions;
using openalprnet;
using System.Security.Claims;
using System.Drawing;          
using System.Drawing.Imaging;  
using System.Threading;        
using System.IO;

namespace Parqueadero.Controllers.Api
{
    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador,Cliente")]
    [Route("api/Reservas")]
    [ApiController]
    public class ReservasApiController : ControllerBase
    {
        private readonly IZonaServicio _zonasServicio;
        private readonly IReservaServicio _reservaServicio;
        private readonly IWebHostEnvironment _environment;

        public ReservasApiController(IReservaServicio reservaServicio, IZonaServicio zonasServicio, IWebHostEnvironment environment)
        {
            _zonasServicio = zonasServicio;

            _reservaServicio = reservaServicio;
            _environment = environment;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> ObtenerTodas()
        {
            if (User.IsInRole("Cliente"))
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                    return Unauthorized();

                var reservasCliente = await _reservaServicio.ObtenerPorUsuario(int.Parse(userIdString));
                return Ok(reservasCliente);
            }

            var reservas = await _reservaServicio.ObtenerTodo();
            return Ok(reservas);
        }


        [HttpGet("{id}")]
    public async Task<ActionResult<Reserva>> ObtenerPorId(int id)
    {
        var reserva = await _reservaServicio.ObtenerPorId(id);
        if (reserva == null)
        {
            return NotFound();
        }
        return Ok(reserva);
    }

        [HttpPost]
        public async Task<ActionResult<Reserva>> Crear([FromBody] Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                if (User.IsInRole("Cliente"))
                {
                    reserva.UsuarioId = int.Parse(userIdString);
                }

                DateTime horaLocal = DateTime.SpecifyKind(reserva.HoraEntrada, DateTimeKind.Local);

                // 2. La convertimos a UTC (Hora Universal) para PostgreSQL
                reserva.HoraEntrada = horaLocal.ToUniversalTime();
                reserva.Estado = EstadoReserva.Activa;

                var reservaCreada = await _reservaServicio.Insertar(reserva);

                // =====================================
                // Cambiar estado de la zona a Ocupada
                // =====================================
                var zona = await _zonasServicio.ObtenerPorId(reserva.ZonaId);
                if (zona != null)
                {
                    zona.Estado = EstadoPlaza.Reservada;
                    await _zonasServicio.Actualizar(zona);
                }

                return CreatedAtAction(nameof(ObtenerPorId), new { id = reservaCreada.Id }, reservaCreada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{id}/imagen")]
        [AllowAnonymous] 
        public async Task<IActionResult> ObtenerImagen(int id)
        {
            var reserva = await _reservaServicio.ObtenerPorId(id);
            if (reserva == null || reserva.ImagenPlaca == null)
            {
                // Puedes retornar una imagen por defecto o 404
                return NotFound("La reserva no tiene imagen asociada.");
            }

            // Retorna los bytes como archivo de imagen (image/jpeg o image/png)
            return File(reserva.ImagenPlaca, "image/jpeg");
        }
        [HttpPost("{id}/AdjuntarImagen")]
        public async Task<IActionResult> AdjuntarImagen(int id, [FromBody] ImagenRequest request)
        {
            if (string.IsNullOrEmpty(request.ImageData))
                return BadRequest(new { mensaje = "No se recibió imagen." });

            var reserva = await _reservaServicio.ObtenerPorId(id);
            if (reserva == null) return NotFound();

            try
            {
                // Limpieza básica del Base64
                string base64 = request.ImageData.Contains(",") ? request.ImageData.Split(',')[1] : request.ImageData;
                byte[] imageBytes = Convert.FromBase64String(base64);

                // Guardar en BD
                reserva.ImagenPlaca = imageBytes;
                await _reservaServicio.Actualizar(reserva); // Asegúrate de que tu servicio llame al repo y haga SaveChanges

                return Ok(new { success = true, mensaje = "Imagen guardada en BD correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = $"Error al procesar imagen: {ex.Message}" });
            }
        }
        [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] Reserva reserva)
    {
        if (id != reserva.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _reservaServicio.Actualizar(reserva);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("{id}/Cancelar")]
    public async Task<IActionResult> CancelarReserva(int id)
    {
        try
        {
            if (await _reservaServicio.CancelarReserva(id))
                return NoContent();
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("{id}/finalizar")]
    public async Task<IActionResult> FinalizarReserva(int id)
    {
        try
        {
            var reserva = await _reservaServicio.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }

            if (reserva.Estado == EstadoReserva.Finalizada)
            {
                return BadRequest(new { mensaje = "La reserva ya está finalizada" });
            }

            reserva.Estado = EstadoReserva.Finalizada;
            reserva.HoraSalida = DateTime.Now;
            _reservaServicio.Actualizar(reserva);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

// ========================================
// Guardar imagen capturada en wwwroot/capturas
// ========================================
[HttpPost("GuardarImagenCapturada")]
public IActionResult GuardarImagenCapturada([FromBody] ImagenRequest request)
{
    if (string.IsNullOrEmpty(request.ImageData))
        return BadRequest(new { success = false, mensaje = "No se recibió imagen." });

    try
    {
        string base64 = request.ImageData.Contains(",") ? request.ImageData.Split(',')[1] : request.ImageData;
        byte[] imageBytes = Convert.FromBase64String(base64);

        // Crear carpeta si no existe
        string capturasDir = Path.Combine(_environment.WebRootPath, "capturas");
        if (!Directory.Exists(capturasDir))
            Directory.CreateDirectory(capturasDir);

        // Generar nombre único
        string fileName = $"captura_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
        string filePath = Path.Combine(capturasDir, fileName);

        // Guardar imagen
        System.IO.File.WriteAllBytes(filePath, imageBytes);

        // Retornar URL pública
        string url = $"/capturas/{fileName}";
        return Ok(new { success = true, url });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { success = false, mensaje = $"Error al guardar imagen: {ex.Message}" });
    }
}

// ========================================
// Clase para recibir imagen
// ========================================
public class ImagenRequest
{
    public string ImageData { get; set; }
}


        // ========================================
        // Procesar imagen y detectar placa
        // ========================================
       [AllowAnonymous]
[HttpPost("ProcesarImagen")]
public IActionResult ProcesarImagen([FromBody] PlacaRequest request)
{
    var response = new PlacaRespuesta();

    if (string.IsNullOrEmpty(request.ImageData))
    {
        response.Success = false;
        response.Message = "No se recibió imagen.";
        return Ok(response);
    }

    try
    {
        // 1. Limpiar Base64 (quitar encabezado, espacios y saltos)
        string base64 = request.ImageData.Contains(",") ? request.ImageData.Split(',')[1] : request.ImageData;
        base64 = base64.Trim().Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\"", "");

        int mod4 = base64.Length % 4;
        if (mod4 > 0) base64 += new string('=', 4 - mod4);

        byte[] imageBytes = Convert.FromBase64String(base64);

        // 2. Llamar a OpenALPR para reconocer placa
        string placaDetectada = ReconocerPlaca(imageBytes);

        if (string.IsNullOrWhiteSpace(placaDetectada))
        {
            response.Success = false;
            response.Message = "No se pudo detectar la placa. Intente acercar más la cámara o mejorar la iluminación.";
            return Ok(response);
        }

        // 3. Normalizar y validar formato
        placaDetectada = placaDetectada.ToUpper().Replace(" ", "").Replace("-", "");
        if (!ValidarFormatoPlaca(placaDetectada))
        {
            response.Success = false;
            response.Message = $"Placa detectada pero formato inválido: {placaDetectada}";
            return Ok(response);
        }

        // 4. Éxito
        response.Success = true;
        response.Placa = placaDetectada;
        response.Message = $"Placa detectada correctamente: {placaDetectada}";
        return Ok(response);
    }
    catch (Exception ex)
    {
        response.Success = false;
        response.Message = $"Error al procesar la imagen: {ex.Message}";
        return Ok(response);
    }
}


        // Método auxiliar temporal sin Hilos para probar las librerías gráficas
        private void AplicarFiltrosDiagnostico(byte[] imageBytes, string directorioDestino)
        {
            // Solo probamos UNO para ver si System.Drawing funciona
            using (var ms = new MemoryStream(imageBytes))
            using (var bmp = new Bitmap(ms))
            {
                string path = Path.Combine(directorioDestino, $"filtro_diagnostico_{DateTime.Now.Ticks}.jpg");
                bmp.Save(path, ImageFormat.Jpeg);
            }
        }

        // ============================================================
        // LÓGICA DE FILTROS EN 4 HILOS CONCURRENTES
        // ============================================================
        private void AplicarFiltrosEnHilos(byte[] imageBytes)
        {
            // Asegurar que el directorio exista
            string filtrosDir = Path.Combine(_environment.WebRootPath, "filtros");
            if (!Directory.Exists(filtrosDir))
                Directory.CreateDirectory(filtrosDir);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // HILO 1: Escala de Grises
            Thread hiloGris = new Thread(() =>
            {
                try
                {
                    using (var ms = new MemoryStream(imageBytes))
                    using (var bmp = new Bitmap(ms))
                    {
                        ColorMatrix colorMatrix = new ColorMatrix(
                            new float[][]
                            {
                                new float[] {.3f, .3f, .3f, 0, 0},
                                new float[] {.59f, .59f, .59f, 0, 0},
                                new float[] {.11f, .11f, .11f, 0, 0},
                                new float[] {0, 0, 0, 1, 0},
                                new float[] {0, 0, 0, 0, 1}
                            });

                        using (var attributes = new ImageAttributes())
                        {
                            attributes.SetColorMatrix(colorMatrix);
                            using (var grisBmp = new Bitmap(bmp.Width, bmp.Height))
                            using (var g = Graphics.FromImage(grisBmp))
                            {
                                g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
                                            0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                                
                                string path = Path.Combine(filtrosDir, $"filtro_gris_{timestamp}.jpg");
                                grisBmp.Save(path, ImageFormat.Jpeg);
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Error Hilo Gris: {ex.Message}"); }
            });

            // HILO 2: Reducción de Tamaño (50%)
            Thread hiloReduccion = new Thread(() =>
            {
                try
                {
                    using (var ms = new MemoryStream(imageBytes))
                    using (var bmp = new Bitmap(ms))
                    {
                        int newWidth = bmp.Width / 2;
                        int newHeight = bmp.Height / 2;
                        using (var smallBmp = new Bitmap(bmp, new Size(newWidth, newHeight)))
                        {
                            string path = Path.Combine(filtrosDir, $"filtro_small_{timestamp}.jpg");
                            smallBmp.Save(path, ImageFormat.Jpeg);
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Error Hilo Reduccion: {ex.Message}"); }
            });

            // HILO 3: Brillo (Aumentar brillo)
            Thread hiloBrillo = new Thread(() =>
            {
                try
                {
                    using (var ms = new MemoryStream(imageBytes))
                    using (var bmp = new Bitmap(ms))
                    {
                        float brillo = 0.2f; 
                        ColorMatrix colorMatrix = new ColorMatrix(
                            new float[][]
                            {
                                new float[] {1, 0, 0, 0, 0},
                                new float[] {0, 1, 0, 0, 0},
                                new float[] {0, 0, 1, 0, 0},
                                new float[] {0, 0, 0, 1, 0},
                                new float[] {brillo, brillo, brillo, 0, 1}
                            });

                        using (var attributes = new ImageAttributes())
                        {
                            attributes.SetColorMatrix(colorMatrix);
                            using (var brilloBmp = new Bitmap(bmp.Width, bmp.Height))
                            using (var g = Graphics.FromImage(brilloBmp))
                            {
                                g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height),
                                            0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);

                                string path = Path.Combine(filtrosDir, $"filtro_brillo_{timestamp}.jpg");
                                brilloBmp.Save(path, ImageFormat.Jpeg);
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Error Hilo Brillo: {ex.Message}"); }
            });

            // HILO 4: Rotación (45°, 90°, 180°)
            Thread hiloRotacion = new Thread(() =>
            {
                try
                {
                    using (var ms = new MemoryStream(imageBytes))
                    using (var bmpOriginal = new Bitmap(ms))
                    {
                        // Rotación 90
                        using (var bmp90 = (Bitmap)bmpOriginal.Clone())
                        {
                            bmp90.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            bmp90.Save(Path.Combine(filtrosDir, $"filtro_rot90_{timestamp}.jpg"), ImageFormat.Jpeg);
                        }

                        // Rotación 180
                        using (var bmp180 = (Bitmap)bmpOriginal.Clone())
                        {
                            bmp180.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            bmp180.Save(Path.Combine(filtrosDir, $"filtro_rot180_{timestamp}.jpg"), ImageFormat.Jpeg);
                        }

                        // Rotación 45
                        double angle = 45;
                        double radians = angle * Math.PI / 180.0;
                        double cos = Math.Abs(Math.Cos(radians));
                        double sin = Math.Abs(Math.Sin(radians));
                        int newW = (int)(bmpOriginal.Width * cos + bmpOriginal.Height * sin);
                        int newH = (int)(bmpOriginal.Width * sin + bmpOriginal.Height * cos);

                        using (var bmp45 = new Bitmap(newW, newH))
                        using (var g = Graphics.FromImage(bmp45))
                        {
                            g.TranslateTransform(newW / 2.0f, newH / 2.0f);
                            g.RotateTransform((float)angle);
                            g.TranslateTransform(-bmpOriginal.Width / 2.0f, -bmpOriginal.Height / 2.0f);
                            g.DrawImage(bmpOriginal, new Point(0, 0));
                            
                            bmp45.Save(Path.Combine(filtrosDir, $"filtro_rot45_{timestamp}.jpg"), ImageFormat.Jpeg);
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine($"Error Hilo Rotacion: {ex.Message}"); }
            });

            // Iniciar hilos
            hiloGris.Start();
            hiloReduccion.Start();
            hiloBrillo.Start();
            hiloRotacion.Start();
        }


        // ========================================
        // Binding C# de OpenALPR
        // ========================================
        private string ReconocerPlaca(byte[] imageBytes)
{
    string basePath = Path.Combine(_environment.ContentRootPath, "openalpr_64");
    string runtimeDir = Path.Combine(basePath, "runtime_data");
    string configPath = Path.Combine(basePath, "openalpr.conf");

    if (!System.IO.File.Exists(configPath))
        throw new Exception($"openalpr.conf no encontrado en {configPath}");
    if (!System.IO.Directory.Exists(runtimeDir))
        throw new Exception($"runtime_data no encontrado en {runtimeDir}");

    string country = "us"; // o "co" si tienes el modelo colombiano

    try
    {
        using (var alpr = new AlprNet(country, configPath, runtimeDir))
        {
            if (!alpr.IsLoaded())
                throw new Exception("Error al cargar AlprNet. Verifica DLLs nativos y runtime_data.");

            alpr.TopN = 5;

            string tempPath = Path.GetTempFileName();
            System.IO.File.WriteAllBytes(tempPath, imageBytes);

            try
            {
                Console.WriteLine($"[Alpr] Procesando imagen temporal: {tempPath}");
                var result = alpr.Recognize(tempPath);

                if (result?.Plates != null && result.Plates.Count > 0)
                {
                    var placa = result.Plates[0].BestPlate?.Characters;
                    Console.WriteLine($"[Alpr] Placa detectada: {placa}");
                    return placa;
                }

                Console.WriteLine("[Alpr] No se detectó ninguna placa.");
                return null;
            }
            finally
            {
                System.IO.File.Delete(tempPath);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Alpr] Error: {ex.Message}");
        return null; // así siempre devuelve algo y no bloquea el frontend
    }
}


        // ========================================
        // Validación simple de placas colombianas
        // ========================================
        private bool ValidarFormatoPlaca(string placa)
        {
            return Regex.IsMatch(placa, @"^[A-Z]{3}\d{3}$|^[A-Z]{3}\d{2}[A-Z]$");
        }
    }

    public class PlacaRequest
    {
        public string ImageData { get; set; }
    }

    public class PlacaRespuesta
    {
        public bool Success { get; set; }
        public string Placa { get; set; }
        public string Message { get; set; }
    }
}
