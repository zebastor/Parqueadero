using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface IVehiculoServicio : IGenericoServicio<Vehiculo>
{
    Task<IEnumerable<Vehiculo>> ObtenerPorUsuario(int usuarioId);
    Task<Vehiculo?> ObtenerPorPlaca(string placa);
}