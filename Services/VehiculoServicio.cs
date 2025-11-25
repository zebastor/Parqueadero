using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class VehiculoServicio : GenericoServicio<Vehiculo>, IVehiculoServicio
{
    public VehiculoServicio(IVehiculoRepositorio repositorio) : base(repositorio)
    {
    }

    public async Task<IEnumerable<Vehiculo>> ObtenerPorUsuario(int usuarioId) 
        => await ((IVehiculoRepositorio)_repositorio).ObtenerPorUsuario(usuarioId).ToListAsync();

    public async Task<Vehiculo?> ObtenerPorPlaca(string placa)
    {
        return await ((IVehiculoRepositorio)_repositorio).ObtenerPorPlaca(placa);
    }
}
