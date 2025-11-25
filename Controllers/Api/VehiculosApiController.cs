using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;
using System.Security.Claims;

namespace Parqueadero.Controllers.Api;

[Authorize(Roles = "Administrador,SuperUsuario,Trabajador,Cliente")]
[Route("api/Vehiculos")]
[ApiController]
public class VehiculosApiController : ControllerBase
{
    private readonly IVehiculoServicio _vehiculoServicio;
    private readonly IUsuarioServicio _usuarioServicio;

    public VehiculosApiController(IVehiculoServicio vehiculoServicio, IUsuarioServicio usuarioServicio)
    {
        _vehiculoServicio = vehiculoServicio;
        _usuarioServicio = usuarioServicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> ObtenerTodos()
    {
        if (User.IsInRole("Cliente"))
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            var vehiculosCliente = await _vehiculoServicio.ObtenerPorUsuario(int.Parse(userIdString));
            return Ok(vehiculosCliente);
        }

        var todos = await _vehiculoServicio.ObtenerTodo();
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Vehiculo>> ObtenerPorId(int id)
    {
        var vehiculo = await _vehiculoServicio.ObtenerPorId(id);
        if (vehiculo == null)
        {
            return NotFound();
        }
        return Ok(vehiculo);
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<Vehiculo>> ObtenerPorUsuario(int usuarioId)
    {
        var vehiculo = await _vehiculoServicio.ObtenerPorUsuario(usuarioId);
        if (vehiculo == null)
        {
            return NotFound();
        }
        return Ok(vehiculo);
    }

  [HttpGet("placa/{placa}")]
public async Task<ActionResult<object>> ObtenerPorPlaca(string placa)
{
    var vehiculo = await _vehiculoServicio.ObtenerPorPlaca(placa);
    if (vehiculo == null)
        return NotFound();

    // Traer usuario asociado
    var usuario = await _usuarioServicio.ObtenerPorId(vehiculo.UsuarioId);

    return Ok(new
    {
        vehiculo.Id,
        vehiculo.Placa,
        vehiculo.TipoVehiculo,
        usuarioId = usuario?.Id,
        usuarioNombre = usuario?.Nombre
    });
}

    [HttpPost]
    public async Task<ActionResult<Vehiculo>> Crear([FromBody] Vehiculo vehiculo)
    {
        if (User.IsInRole("Cliente"))
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            vehiculo.UsuarioId = int.Parse(userIdString);
        }


        try
        {
            var vehiculoCreado = await _vehiculoServicio.Insertar(vehiculo);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = vehiculoCreado.Id }, vehiculoCreado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, [FromBody] Vehiculo vehiculo)
    {
        if (id != vehiculo.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _vehiculoServicio.Actualizar(vehiculo);
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
            if (await _vehiculoServicio.Eliminar(id))
                return NoContent();
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
