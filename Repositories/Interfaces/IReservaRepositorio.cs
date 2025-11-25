using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IReservaRepositorio : IGenericoRepositorio<Reserva>
{
    Task<Reserva?> ObtenerPorPlacaConEstadoActivo(string placa);
    Task<bool> CancelarReserva(int id);
    Task<bool> FinalizarReserva(int id);
    IQueryable<Reserva> ObtenerPorUsuario(int usuarioId);
}
