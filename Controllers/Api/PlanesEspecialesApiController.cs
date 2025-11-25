using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers.Api;

[Authorize(Roles = "Administrador,SuperUsuario")]
[Route("api/PlanesEspeciales")]
[ApiController]
public class PlanesEspecialesApiController : ControllerBase
{
    private readonly IPlanEspecialServicio _planEspecialServicio;

    public PlanesEspecialesApiController(IPlanEspecialServicio planEspecialServicio)
    {
        _planEspecialServicio = planEspecialServicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlanEspecial>>> ObtenerTodos()
    {
        var planes = await _planEspecialServicio.ObtenerTodo();
        return Ok(planes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlanEspecial>> ObtenerPorId(int id)
    {
        var plan = await _planEspecialServicio.ObtenerPorId(id);
        if (plan == null)
        {
            return NotFound();
        }
        return Ok(plan);
    }

    [HttpPost]
    public async Task<ActionResult<PlanEspecial>> Crear([FromBody] PlanEspecial plan)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var planCreado = await _planEspecialServicio.Insertar(plan);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = planCreado.Id }, planCreado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] PlanEspecial plan)
    {
        if (id != plan.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _planEspecialServicio.Actualizar(plan);
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
            if (await _planEspecialServicio.Eliminar(id))
                return NoContent();
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
