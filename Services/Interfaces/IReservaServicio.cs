using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface IReservaServicio : IGenericoServicio<Reserva>
{
    Task<Reserva?> ObtenerPorPlacaConEstadoActivo(string placa);
    Task ActualizarHoraSalida(int id);
    Task<bool> CancelarReserva(int id);
    Task<bool> FinalizarReserva(int id);
    Task<IEnumerable<Reserva>> ObtenerPorUsuario(int usuarioId);
}
