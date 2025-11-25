using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class TarifaRepositorio : GenericoRepositorio<Tarifa>, ITarifaRepositorio
{
    public TarifaRepositorio(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<Tarifa?> ObtenerPorParqueaderoYTipo(int parqueaderoId, TipoVehiculo tipoVehiculo)
    {
        return await Entities.FirstOrDefaultAsync(t =>
            t.ParqueaderoId == parqueaderoId &&
            t.TipoVehiculo == tipoVehiculo
        );
    }
    public IQueryable<Tarifa> ObtenerTarifasPorParqueadero(int parqueaderoId)
        => Entities.Where(t => t.ParqueaderoId == parqueaderoId);

    public async Task<Tarifa?> ObtenerPorTipoDeVehiculo(TipoVehiculo tipoVehiculo)
    {
        if (!Enum.IsDefined(typeof(TipoVehiculo), tipoVehiculo))
        {
            throw new ArgumentException("El tipo de vehículo no es válido");
        }
        return await Entities.FirstOrDefaultAsync(t => t.TipoVehiculo == tipoVehiculo);
    }
}
