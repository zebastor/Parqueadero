using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IUsuarioRepositorio : IGenericoRepositorio<Usuario>
{
    Task<Usuario?> ObtenerPorEmail(string email);
    IQueryable<Usuario> ObtenerTodosPorEmpresa(int empresaId);
    Task AsignarPlanEspecial(int usuarioId, int? planEspecialId);
}