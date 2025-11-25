using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IVehiculoRepositorio : IGenericoRepositorio<Vehiculo>
{
    Task<Vehiculo?> ObtenerPorPlaca(string placa);
    IQueryable<Vehiculo> ObtenerPorUsuario(int usuarioId);

}