using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class ZonaServicio : GenericoServicio<Zona>, IZonaServicio
{
    private readonly IVehiculoServicio _vehiculoServicio;
    public ZonaServicio(IZonaRepositorio repositorio, IVehiculoServicio vehiculoServicio) : base(repositorio)
    {
        _vehiculoServicio = vehiculoServicio;
    }

    public async Task<IEnumerable<Zona>> ObtenerTodosPorPiso(int pisoId) 
        => await ((IZonaRepositorio)_repositorio).ObtenerTodosPorPiso(pisoId).ToListAsync();

    public async Task<IEnumerable<Zona>> ObtenerTodosPorTipoVehiculoYPiso(string placa, int pisoId) 
    {
        var vehiculo = await _vehiculoServicio.ObtenerPorPlaca(placa) ?? throw new Exception("No se encontró el vehículo.");

        return await ((IZonaRepositorio)_repositorio).ObtenerTodosPorTipoVehiculoYPiso(vehiculo.TipoVehiculo, pisoId).ToListAsync();
    }
}