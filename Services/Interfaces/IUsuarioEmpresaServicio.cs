using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;
public interface IUsuarioEmpresaServicio : IGenericoServicio<UsuarioEmpresa>
{
    Task<UsuarioEmpresa?> ObtenerPorUsuarioYEmpresa(int usuarioId, int empresaId);
    Task<IEnumerable<UsuarioEmpresa>> ObtenerPorUsuario(int usuarioId);
    Task<IEnumerable<UsuarioEmpresa>> ObtenerPorEmpresa(int empresaId);
    Task AsignarPlanEspecial(int usuarioId, int empresaId, int? planEspecialId);
}