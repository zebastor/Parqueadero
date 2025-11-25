using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface IZonaServicio : IGenericoServicio<Zona>
{
    Task<IEnumerable<Zona>> ObtenerTodosPorPiso(int pisoId);
    Task<IEnumerable<Zona>> ObtenerTodosPorTipoVehiculoYPiso(string placa, int pisoId);
}
