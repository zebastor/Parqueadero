using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class ZonaRepositorio : GenericoRepositorio<Zona>, IZonaRepositorio
{
    public ZonaRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<Zona> ObtenerTodosPorPiso(int pisoId) 
        => Entities.Where(z => z.PisoId == pisoId);

    public IQueryable<Zona> ObtenerTodosPorTipoVehiculoYPiso(TipoVehiculo tipoVehiculo, int pisoId) 
        => Entities.Where(z => z.TipoVehiculo == tipoVehiculo && z.PisoId == pisoId);
}
