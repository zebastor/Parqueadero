using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;
using System.Security.Claims;

namespace Parqueadero.Controllers.Api;

[Authorize(Roles = "SuperUsuario,Administrador,Trabajador,Cliente")]
[Route("api/Parqueaderos")]
[ApiController]
public class ParqueaderosApiController : ControllerBase
{
    private readonly IParqueaderoServicio _parqueaderoServicio;
    private readonly IUsuarioEmpresaServicio _usuarioEmpresaServicio;

    public ParqueaderosApiController(
        IParqueaderoServicio parqueaderoServicio,
        IUsuarioEmpresaServicio usuarioEmpresaServicio)
    {
        _parqueaderoServicio = parqueaderoServicio;
        _usuarioEmpresaServicio = usuarioEmpresaServicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Models.Parqueadero>>> ObtenerTodos()
    {
        var user = User;

        // === LÓGICA PARA CLIENTES ===
        if (user.IsInRole("Cliente"))
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);

            // 1. Obtener empresas del cliente
            var misEmpresas = await _usuarioEmpresaServicio.ObtenerPorUsuario(userId);

            // Si no tiene empresas, lista vacía
            if (!misEmpresas.Any()) return Ok(new List<Models.Parqueadero>());

            var idsEmpresas = misEmpresas.Select(ue => ue.EmpresaId).ToList();

            // 2. Obtener todos los parqueaderos
            var todos = await _parqueaderoServicio.ObtenerTodo();

            // 3. Filtrar: Dejar solo los que coincidan con mis empresas
            var filtrados = todos.Where(p => idsEmpresas.Contains(p.EmpresaId)).ToList();

            return Ok(filtrados);
        }

        // === LÓGICA PARA SUPERUSUARIO ===
        if (user.IsInRole("SuperUsuario"))
        {
            return Ok(await _parqueaderoServicio.ObtenerTodo());
        }

        // === LÓGICA PARA ADMIN/TRABAJADOR ===
        var empresaIdClaim = user.FindFirst("EmpresaId")?.Value;
        if (empresaIdClaim != null)
        {
            return Ok(await _parqueaderoServicio.ObtenerTodoPorEmpresa(int.Parse(empresaIdClaim)));
        }

        return Unauthorized("No se pudo determinar el contexto del usuario.");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Models.Parqueadero>> ObtenerPorId(int id)
    {
        var parqueadero = await _parqueaderoServicio.ObtenerPorId(id);
        if (parqueadero == null)
            return NotFound();

        return Ok(parqueadero);
    }

    [HttpPost]
    [Authorize(Roles = "SuperUsuario,Administrador")]
    public async Task<ActionResult<Models.Parqueadero>> Crear(Models.Parqueadero parqueadero)
    {
        try
        {
            var parqueaderoCreado = await _parqueaderoServicio.Insertar(parqueadero);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = parqueaderoCreado.Id }, parqueaderoCreado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperUsuario,Administrador")]
    public IActionResult Actualizar(int id, Models.Parqueadero parqueadero)
    {
        if (id != parqueadero.Id)
            return BadRequest();

        try
        {
            _parqueaderoServicio.Actualizar(parqueadero);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperUsuario,Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            if (await _parqueaderoServicio.Eliminar(id))
                return NoContent();
            return NotFound(new { mensaje = "No se encontró el parqueadero." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}