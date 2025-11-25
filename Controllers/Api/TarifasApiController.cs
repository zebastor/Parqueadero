using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers.Api;

[Authorize(Roles = "Administrador,SuperUsuario")]
[Route("api/Tarifas")]
[ApiController]
public class TarifasApiController : ControllerBase
{
    private readonly ITarifaServicio _tarifaServicio;

    public TarifasApiController(ITarifaServicio tarifaServicio)
    {
        _tarifaServicio = tarifaServicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tarifa>>> ObtenerTodas()
    {
        var tarifas = await _tarifaServicio.ObtenerTodo();
        return Ok(tarifas);
    }

    [HttpGet("parqueadero/{parqueaderoId}")]
    public async Task<ActionResult<IEnumerable<Tarifa>>> ObtenerPorParqueadero(int parqueaderoId)
    {
        var tarifas = await _tarifaServicio.ObtenerTarifasPorParqueadero(parqueaderoId);
        return Ok(tarifas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Tarifa>> ObtenerPorId(int id)
    {
        var tarifa = await _tarifaServicio.ObtenerPorId(id);
        if (tarifa == null)
        {
            return NotFound();
        }
        return Ok(tarifa);
    }

    [HttpPost]
    public async Task<ActionResult<Tarifa>> Crear([FromBody] Tarifa tarifa)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var tarifaCreada = await _tarifaServicio.Insertar(tarifa);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = tarifaCreada.Id }, tarifaCreada);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] Tarifa tarifa)
    {
        if (id != tarifa.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _tarifaServicio.Actualizar(tarifa);
            return NoContent();
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
            if (await _tarifaServicio.Eliminar(id))
                return NoContent();
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
