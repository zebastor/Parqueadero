using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface IPisoServicio : IGenericoServicio<Piso>
{
    Task<Piso?> ObtenerUltimoPisoPorParqueadero(int parqueaderoId);
    Task<IEnumerable<Piso>> ObtenerTodosPorParqueadero(int parqueaderoId);
    Task<Piso> InsertarPiso(int parqueaderoId);
}
