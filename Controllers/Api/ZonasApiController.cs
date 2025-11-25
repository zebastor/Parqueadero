using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers.Api;

[ApiController]
[Route("api/Zonas")]
public class ZonasApiController : ControllerBase
{
    private readonly IZonaServicio _zonasServicio;

    public ZonasApiController(IZonaServicio zonasServicio)
    {
        _zonasServicio = zonasServicio;
    }

    [HttpGet("{pisoId}")]
    public async Task<ActionResult<IEnumerable<Zona>>> ObtenerTodosPorPiso(int pisoId)
    {
        return Ok(await _zonasServicio.ObtenerTodosPorPiso(pisoId));
    }

    [HttpGet("TipoVehiculo/{placa}/Piso/{pisoId}")]
    public async Task<ActionResult<IEnumerable<Zona>>> ObtenerTodosPorTipoVehiculo(string placa, int pisoId)
    {
        return Ok(await _zonasServicio.ObtenerTodosPorTipoVehiculoYPiso(placa, pisoId));
    }

  [HttpPost("Insertar")]
public async Task<ActionResult<IEnumerable<Zona>>> Insertar(
    [FromBody] Zona zona, 
    [FromQuery] int cantidad = 1,
    [FromQuery] char letraInicial = 'A') // Nueva opción
{
    try
    {
        if (cantidad <= 0 || cantidad > 26)
            return BadRequest(new { mensaje = "La cantidad debe estar entre 1 y 26" });

        if (!Enum.IsDefined(typeof(TipoVehiculo), zona.TipoVehiculo))
            return BadRequest(new { mensaje = "Valor de tipo de vehículo inválido" });

        // Validar letra inicial
        letraInicial = char.ToUpper(letraInicial);
        if (letraInicial < 'A' || letraInicial > 'Z')
            return BadRequest(new { mensaje = "La letra inicial debe estar entre A y Z" });

        var zonasCreadas = new List<Zona>();

        for (int i = 0; i < cantidad; i++)
        {
            char letra = (char)(letraInicial + i);
            if (letra > 'Z')
                break; // No pasar de la Z

            var nuevaZona = new Zona
            {
                PisoId = zona.PisoId,
                Codigo = zona.Codigo,
                Nombre = letra.ToString(),
                TipoVehiculo = zona.TipoVehiculo,
                Estado = EstadoPlaza.Libre
            };

            var creada = await _zonasServicio.Insertar(nuevaZona);
            zonasCreadas.Add(creada);
        }

        return CreatedAtAction(nameof(ObtenerPorId), new { id = zonasCreadas.First().Id }, zonasCreadas);
    }
    catch (Exception ex)
    {
        return BadRequest(new { mensaje = ex.Message });
    }
}



    [HttpGet("ObtenerPorId/{id}")]
    public async Task<ActionResult<Zona>> ObtenerPorId(int id)
    {
        var zona = await _zonasServicio.ObtenerPorId(id);
        if (zona == null)
            return NotFound();

        return Ok(zona);
    }

    [HttpPut("{id}")]
    public ActionResult<Zona> Actualizar(int id, [FromBody] Zona zona)
    {
        try
        {
            if (!Enum.IsDefined(typeof(TipoVehiculo), zona.TipoVehiculo)) // Asumiendo que tu enum se llama TipoVehiculo
            {
                return BadRequest(new { mensaje = "Valor de tipo de vehículo inválido" });
            }

            _zonasServicio.Actualizar(zona);
            return Ok(zona);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            if (await _zonasServicio.Eliminar(id))
                return NoContent();
            return NotFound(new { mensaje = "No se encontró la zona." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}