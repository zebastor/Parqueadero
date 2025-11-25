using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IPisosRepositorio : IGenericoRepositorio<Piso>
{    
    Task<Piso?> ObtenerUltimoPisoPorParqueadero(int parqueaderoId);
    IQueryable<Piso> ObtenerTodosPorParqueadero(int parqueaderoId);
}
