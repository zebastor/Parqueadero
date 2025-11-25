using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface IUsuarioServicio : IGenericoServicio<Usuario>
{
    Task<IEnumerable<Usuario>> ObtenerTodosPorEmpresa(int empresaId);
    Task ActualizarAsync(Usuario entidad);
    Task AsignarPlanEspecial(int usuarioId, int? planEspecialId);
    Task<Usuario?> ObtenerUsuarioPorCorreoAsync(string correo);
}
