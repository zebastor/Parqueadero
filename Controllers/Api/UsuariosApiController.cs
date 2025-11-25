using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;
using System.Security.Claims;

namespace Parqueadero.Controllers.Api;

[Authorize(Roles = "SuperUsuario,Administrador,Trabajador,Cliente")]
[Route("api/Usuarios")]
[ApiController]
public class UsuariosApiController : ControllerBase
{
    private readonly IUsuarioServicio _usuarioServicio;
    private readonly IUsuarioEmpresaServicio _usuarioEmpresaServicio;
    private readonly IAutenticacionServicio _autenticacionServicio;

    public UsuariosApiController(
        IUsuarioServicio usuarioServicio,
        IUsuarioEmpresaServicio usuarioEmpresaServicio,
        IAutenticacionServicio autenticacionServicio)
    {
        _usuarioServicio = usuarioServicio;
        _usuarioEmpresaServicio = usuarioEmpresaServicio;
        _autenticacionServicio = autenticacionServicio;
    }

    // ============================================================
    // ENDPOINTS GENERALES (Gestión de Usuarios)
    // ============================================================

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> ObtenerTodos()
    {
        var todos = await _usuarioServicio.ObtenerTodo();

        if (User.IsInRole("SuperUsuario"))
        {
            return Ok(todos);
        }

        // Si es Admin/Trabajador, filtrar por su empresa
        var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
        if (empresaIdClaim != null)
        {
            int empresaId = int.Parse(empresaIdClaim);

            // Usuarios staff de la misma empresa
            var usuariosMismaEmpresa = todos.Where(u => u.EmpresaId == empresaId && u.Rol != Rol.Cliente);

            // Clientes (globales o filtrados, según regla de negocio. Aquí mostramos todos los clientes para que puedan asignar vehículos)
            var clientes = todos.Where(u => u.Rol == Rol.Cliente);

            var resultado = usuariosMismaEmpresa.Concat(clientes).ToList();
            return Ok(resultado);
        }

        // Si es Cliente, no debería ver el listado de todos los usuarios por seguridad
        if (User.IsInRole("Cliente"))
        {
            return Ok(new List<Usuario>());
        }

        return Unauthorized("No tiene permisos para listar usuarios.");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> ObtenerPorId(int id)
    {
        var usuario = await _usuarioServicio.ObtenerPorId(id);
        if (usuario == null) return NotFound();
        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> Crear([FromBody] Usuario usuario)
    {
        try
        {
            if (!Enum.IsDefined(usuario.Rol)) return BadRequest(new { mensaje = "Valor de rol inválido" });

            // Si quien crea es un trabajador/admin, asignamos su empresa al nuevo usuario (si no es cliente global)
            var empresaIdClaim = User.FindFirst("EmpresaId")?.Value;
            int? empresaIdUsuarioActual = string.IsNullOrEmpty(empresaIdClaim) ? (int?)null : int.Parse(empresaIdClaim);

            if (usuario.Rol == Rol.Cliente && empresaIdUsuarioActual.HasValue)
            {
                // Opcional: Asignar empresa principal al cliente si se desea
                usuario.EmpresaId = empresaIdUsuarioActual;
            }

            var usuarioCreado = await _autenticacionServicio.RegistrarUsuario(usuario);

            // Si se creó un cliente dentro de una empresa, crear la relación explícita en UsuarioEmpresa
            if (usuarioCreado.Rol == Rol.Cliente && usuarioCreado.EmpresaId.HasValue)
            {
                var usuarioEmpresa = new UsuarioEmpresa
                {
                    UsuarioId = usuarioCreado.Id,
                    EmpresaId = usuarioCreado.EmpresaId.Value,
                    PlanEspecialId = null
                };
                await _usuarioEmpresaServicio.Insertar(usuarioEmpresa);
            }

            return CreatedAtAction(nameof(ObtenerPorId), new { id = usuarioCreado.Id }, usuarioCreado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, Usuario usuario)
    {
        if (id != usuario.Id) return BadRequest();

        try
        {
            var usuarioExistente = await _usuarioServicio.ObtenerPorId(id);
            if (usuarioExistente == null) return NotFound(new { mensaje = "Usuario no encontrado." });

            // Preservar la contraseña si no se envía una nueva (logica en servicio o aquí)
            // Preservar la empresa si es un cambio de rol delicado
            if (usuarioExistente.Rol != Rol.Cliente && usuario.Rol == Rol.Cliente)
            {
                usuario.EmpresaId = usuarioExistente.EmpresaId;
            }

            await _usuarioServicio.ActualizarAsync(usuario);
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
            if (await _usuarioServicio.Eliminar(id)) return NoContent();
            return NotFound(new { mensaje = "No se encontró el usuario." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // ============================================================
    // ENDPOINTS ESPECÍFICOS PARA EL ROL CLIENTE (Gestión de Mis Empresas)
    // ============================================================

    // 1. Obtener mis empresas (las del usuario logueado)
    [HttpGet("Cliente/MisEmpresas")]
    [Authorize(Roles = "Cliente")]
    public async Task<ActionResult<IEnumerable<UsuarioEmpresa>>> MisEmpresas()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        int userId = int.Parse(userIdString);

        // Obtiene las relaciones e incluye los datos de la Empresa y el Plan
        var relaciones = await _usuarioEmpresaServicio.ObtenerPorUsuario(userId);
        return Ok(relaciones);
    }

    // 2. Asociarme a una empresa (con o sin plan)
    [HttpPost("Cliente/Asociarme/{empresaId}")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Asociarme(int empresaId, [FromBody] AsociarEmpresaRequest request)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        int userId = int.Parse(userIdString);

        try
        {
            // Verificar si ya existe la relación
            var existente = await _usuarioEmpresaServicio.ObtenerPorUsuarioYEmpresa(userId, empresaId);
            if (existente != null)
            {
                return BadRequest(new { mensaje = "Ya estás registrado en esta empresa." });
            }

            var nuevaRelacion = new UsuarioEmpresa
            {
                UsuarioId = userId,
                EmpresaId = empresaId,
                PlanEspecialId = request.PlanEspecialId // Aquí guardamos el plan seleccionado (puede ser null)
            };

            await _usuarioEmpresaServicio.Insertar(nuevaRelacion);
            return Ok(new { mensaje = "Te has unido correctamente a la empresa." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // 3. Salir de una empresa
    [HttpDelete("Cliente/Quitar/{empresaId}")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Quitar(int empresaId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        int userId = int.Parse(userIdString);

        try
        {
            var relacion = await _usuarioEmpresaServicio.ObtenerPorUsuarioYEmpresa(userId, empresaId);
            if (relacion == null) return NotFound(new { mensaje = "No se encontró la relación con esa empresa." });

            await _usuarioEmpresaServicio.Eliminar(relacion.Id);
            return Ok(new { mensaje = "Has salido de la empresa correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}

// DTO para recibir el plan en el body del POST
public class AsociarEmpresaRequest
{
    public int? PlanEspecialId { get; set; }
}