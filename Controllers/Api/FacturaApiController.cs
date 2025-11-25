using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;
using System.Security.Claims; // <-- Asegúrate que esté

namespace Parqueadero.Controllers.Api;

[Route("api/Facturas")]
[ApiController]
// --- INICIO DE MODIFICACIÓN ---
// 1. Quitamos el [Authorize] de la clase y lo ponemos en cada método
[Authorize] // (Ahora solo requiere estar logueado)
// --- FIN DE MODIFICACIÓN ---
public class FacturaApiController : ControllerBase
{
    private readonly IReciboServicio _reciboServicio;

    public FacturaApiController(IReciboServicio reciboServicio)
    {
        _reciboServicio = reciboServicio;
    }

    // 2. Este método sigue siendo solo para Admins
    [HttpGet]
    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador")]
    public async Task<ActionResult<IEnumerable<Recibo>>> ObtenerTodas()
    {
        var facturas = await _reciboServicio.ObtenerTodo();
        return Ok(facturas);
    }

    // 3. Este método sigue siendo solo para Admins
    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador")]
    public async Task<ActionResult<Recibo>> ObtenerPorId(int id)
    {
        var factura = await _reciboServicio.ObtenerPorId(id);
        if (factura == null)
        {
            return NotFound();
        }
        return Ok(factura);
    }

    // --- INICIO DE MODIFICACIÓN ---
    // 4. AÑADIMOS "Cliente" a este método
    [HttpGet("{id}/comprobante")]
    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador,Cliente")]
    public async Task<IActionResult> DescargarComprobante(int id)
    {
        try
        {
            // 5. ¡AÑADIMOS UN CHEQUEO DE SEGURIDAD!
            // Nos aseguramos de que el cliente solo pueda ver SUS propias facturas.
            var recibo = await _reciboServicio.ObtenerPorId(id);
            if (recibo == null) return NotFound();

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            // El Repositorio carga Cobro.Reserva.Usuario
            if (User.IsInRole("Cliente") && recibo.Cobro?.Reserva?.UsuarioId.ToString() != userIdClaim)
            {
                // Si el ID del usuario en la factura no coincide con el ID del cliente logueado
                return Forbid(); // Error 403 Prohibido
            }

            // Si es Admin o si es el Cliente correcto, generamos el comprobante
            var (comprobante, contentType, extension) = await _reciboServicio.GenerarComprobante(id, "pdf");

            return File(comprobante, contentType, $"comprobante.{extension}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al generar el comprobante", message = ex.Message });
        }
    }
    // --- FIN DE MODIFICACIÓN ---
}