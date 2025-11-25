using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers.Api;

[ApiController]
[Route("api/Pisos")]
public class PisosApiController : ControllerBase
{
    private readonly IPisoServicio _pisosServicio;

    public PisosApiController(IPisoServicio pisosServicio)
    {
        _pisosServicio = pisosServicio;
    }

    [HttpGet("{parqueaderoId}")]
    public async Task<ActionResult<IEnumerable<Piso>>> ObtenerTodosPorParqueadero(int parqueaderoId)
    {
        return Ok(await _pisosServicio.ObtenerTodosPorParqueadero(parqueaderoId));
    }

    [HttpPost("{parqueaderoId}")]
    public async Task<ActionResult<Piso>> InsertarPiso(int parqueaderoId)
    {
        var piso = await _pisosServicio.InsertarPiso(parqueaderoId);
        return Ok(piso);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> EliminarPiso(int id)
    {
        try
        {
            if (await _pisosServicio.Eliminar(id))
                return NoContent();
            return NotFound(new { mensaje = "No se encontró el piso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}