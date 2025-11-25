using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers.Api;

[Authorize(Roles = "Administrador,SuperUsuario")]
[Route("api/Descuentos")]
[ApiController]
public class DescuentosApiController : ControllerBase
{
    private readonly IDescuentoServicio _descuentoServicio;

    public DescuentosApiController(IDescuentoServicio descuentoServicio)
    {
        _descuentoServicio = descuentoServicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Descuento>>> ObtenerTodos()
    {
        var descuentos = await _descuentoServicio.ObtenerTodo();
        return Ok(descuentos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Descuento>> ObtenerPorId(int id)
    {
        var descuento = await _descuentoServicio.ObtenerPorId(id);
        if (descuento == null)
        {
            return NotFound();
        }
        return Ok(descuento);
    }

    [HttpPost]
    public async Task<ActionResult<Descuento>> Crear([FromBody] Descuento descuento)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var descuentoCreado = await _descuentoServicio.Insertar(descuento);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = descuentoCreado.Id }, descuentoCreado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] Descuento descuento)
    {
        if (id != descuento.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _descuentoServicio.Actualizar(descuento);
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
            if (await _descuentoServicio.Eliminar(id))
                return NoContent();
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("codigo/{codigo}")]
    public async Task<ActionResult<Descuento?>> BuscarPorCodigo(string codigo)
    {
        var descuento = await _descuentoServicio.BuscarPorCodigo(codigo);
        return Ok(descuento);
    }
}
