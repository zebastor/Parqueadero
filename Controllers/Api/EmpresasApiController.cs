using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;
using System.Security.Claims;

namespace Parqueadero.Controllers.Api;

[Authorize(Roles = "SuperUsuario,Administrador,Trabajador,Cliente")]
[Route("api/Empresas")]
[ApiController]
public class EmpresasApiController : ControllerBase
{
    private readonly IEmpresaServicio _empresaServicio;

    public EmpresasApiController(IEmpresaServicio empresaServicio)
    {
        _empresaServicio = empresaServicio;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Empresa>>> ObtenerTodas()
    {
        // Obtenemos el rol de forma segura
        var rol = User.FindFirst(ClaimTypes.Role)?.Value;

        // 2. LÓGICA MODIFICADA:
        // Si es SuperUsuario O Cliente, devolvemos TODAS las empresas.
        // El cliente necesita verlas todas para poder elegir a cuál registrarse.
        if (rol == "SuperUsuario" || rol == "Cliente")
        {
            var todas = await _empresaServicio.ObtenerTodo();
            return Ok(todas);
        }

        // 3. Lógica para Admin y Trabajador (solo ven su propia empresa)
        var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;

        if (string.IsNullOrEmpty(empresaIdClaim))
            return Unauthorized("No se pudo identificar la empresa del usuario.");

        var empresaId = int.Parse(empresaIdClaim);
        var empresa = await _empresaServicio.ObtenerPorId(empresaId);

        if (empresa == null)
            return NotFound("Empresa no encontrada.");

        return Ok(new List<Empresa> { empresa }); // Retorna lista con 1 elemento
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Empresa>> ObtenerPorId(int id)
    {
        var empresa = await _empresaServicio.ObtenerPorId(id);
        if (empresa == null)
            return NotFound();

        return Ok(empresa);
    }

    [HttpPost]
    [Authorize(Roles = "SuperUsuario")]
    public async Task<ActionResult<Empresa>> Crear(Empresa empresa)
    {
        try
        {
            var empresaCreada = await _empresaServicio.Insertar(empresa);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = empresaCreada.Id }, empresaCreada);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("MiEmpresa")]
public async Task<ActionResult<Empresa>> ObtenerMiEmpresa()
{
    var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;

    if (empresaIdClaim == null)
        return Unauthorized("No se encontró la empresa del usuario.");

    if (!int.TryParse(empresaIdClaim, out int empresaId))
        return BadRequest("El ID de la empresa es inválido.");

    var empresa = await _empresaServicio.ObtenerPorId(empresaId);

    if (empresa == null)
        return NotFound("No se encontró la empresa asociada al usuario.");

    return Ok(empresa);
}


    [HttpPut("{id}")]
    public IActionResult Actualizar(int id, Empresa empresa)
    {
        if (id != empresa.Id)
            return BadRequest();

        try
        {
            _empresaServicio.Actualizar(empresa);
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
            if (await _empresaServicio.Eliminar(id))
                return NoContent();
            return NotFound(new { mensaje = "No se encontró la empresa." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
} 