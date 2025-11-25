using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers.Api;

[Route("api/Registro")]
[ApiController]
public class RegistroApiController : ControllerBase
{
    private readonly IAutenticacionServicio _autenticacionServicio;

    public RegistroApiController(IAutenticacionServicio autenticacionServicio)
    {
        _autenticacionServicio = autenticacionServicio;
    }

    // Registro público de cliente
    [HttpPost("Cliente")]
    [AllowAnonymous]
    public async Task<IActionResult> RegistrarCliente([FromBody] Usuario usuario)
    {
        try
        {
            if (usuario == null)
                return BadRequest(new { mensaje = "Datos incompletos" });

            if (string.IsNullOrEmpty(usuario.Nombre) ||
                string.IsNullOrEmpty(usuario.Correo) ||
                string.IsNullOrEmpty(usuario.Clave))
            {
                return BadRequest(new { mensaje = "Todos los campos son obligatorios" });
            }

            // Forzar rol Cliente
            usuario.Rol = Rol.Cliente;

            // --- LÍNEAS ELIMINADAS ---
            // // No asignar EmpresaId aquí, se asignará cuando el admin/trabajador lo asocie
            // usuario.EmpresaId = null;
            // --- FIN LÍNEAS ELIMINADAS ---

            var creado = await _autenticacionServicio.RegistrarUsuario(usuario);

            return Ok(new { mensaje = "Cliente registrado correctamente", usuarioId = creado.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}