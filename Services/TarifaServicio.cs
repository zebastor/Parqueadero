using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class TarifaServicio : GenericoServicio<Tarifa>, ITarifaServicio
{
    public TarifaServicio(ITarifaRepositorio repositorio) : base(repositorio)
    {
    }

    public async Task<IEnumerable<Tarifa>> ObtenerTarifasPorParqueadero(int parqueaderoId)
        => await ((ITarifaRepositorio)_repositorio).ObtenerTarifasPorParqueadero(parqueaderoId).ToListAsync();

    public async Task<Tarifa?> ObtenerPorTipoDeVehiculo(TipoVehiculo tipoVehiculo)
        => await ((ITarifaRepositorio)_repositorio).ObtenerPorTipoDeVehiculo(tipoVehiculo);

    public async Task<Tarifa?> ObtenerPorParqueaderoYTipo(int parqueaderoId, TipoVehiculo tipoVehiculo)
    {
        return await ((ITarifaRepositorio)_repositorio).ObtenerPorParqueaderoYTipo(parqueaderoId, tipoVehiculo);
    }
}