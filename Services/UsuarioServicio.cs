using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;
using Parqueadero.Models;
using Microsoft.EntityFrameworkCore;

namespace Parqueadero.Services;

public class UsuarioServicio : GenericoServicio<Usuario>, IUsuarioServicio
{
    public UsuarioServicio(IUsuarioRepositorio usuarioRepositorio) : base(usuarioRepositorio)
    {
    }

    public async Task<IEnumerable<Usuario>> ObtenerTodosPorEmpresa(int empresaId) => await ((IUsuarioRepositorio)_repositorio).ObtenerTodosPorEmpresa(empresaId).ToListAsync();

    public async Task ActualizarAsync(Usuario entidad)
    {
        var usuarioExistente = await _repositorio.ObtenerPorId(entidad.Id);
        if (usuarioExistente == null)
            throw new InvalidOperationException("Usuario no encontrado");

        if (!string.IsNullOrEmpty(entidad.Clave))
            usuarioExistente.Clave = entidad.Clave;

        usuarioExistente.Nombre = entidad.Nombre;
        usuarioExistente.Correo = entidad.Correo;
        usuarioExistente.Rol = entidad.Rol;
        usuarioExistente.EmpresaId = entidad.EmpresaId;

        _repositorio.Actualizar(usuarioExistente);
    }

    public async Task AsignarPlanEspecial(int usuarioId, int? planEspecialId)
    {
        await ((IUsuarioRepositorio)_repositorio).AsignarPlanEspecial(usuarioId, planEspecialId);
    }

    public async Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo)
    {
        return await ((IUsuarioRepositorio)_repositorio).ObtenerPorEmail(correo);
    }
}
