using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IZonaRepositorio : IGenericoRepositorio<Zona>
{    
    IQueryable<Zona> ObtenerTodosPorPiso(int pisoId);
    IQueryable<Zona> ObtenerTodosPorTipoVehiculoYPiso(TipoVehiculo tipoVehiculo, int pisoId);
}
