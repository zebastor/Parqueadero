// Interfaces
using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IUsuarioEmpresaRepositorio : IGenericoRepositorio<UsuarioEmpresa>
{
    Task<UsuarioEmpresa?> ObtenerPorUsuarioYEmpresa(int usuarioId, int empresaId);
    IQueryable<UsuarioEmpresa> ObtenerPorUsuario(int usuarioId);
    IQueryable<UsuarioEmpresa> ObtenerPorEmpresa(int empresaId);
}