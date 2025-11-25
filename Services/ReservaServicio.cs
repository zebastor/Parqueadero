using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Parqueadero.Services;

public class ReservaServicio : GenericoServicio<Reserva>, IReservaServicio
{
    public ReservaServicio(IReservaRepositorio repositorio) : base(repositorio)
    {
    }

    public async Task<Reserva?> ObtenerPorPlacaConEstadoActivo(string placa)
        => await ((IReservaRepositorio)_repositorio).ObtenerPorPlacaConEstadoActivo(placa);

    public async Task<bool> CancelarReserva(int id)
        => await ((IReservaRepositorio)_repositorio).CancelarReserva(id);

    public async Task<bool> FinalizarReserva(int id)
        => await ((IReservaRepositorio)_repositorio).FinalizarReserva(id);
    public async Task<IEnumerable<Reserva>> ObtenerPorUsuario(int usuarioId)
    {
        return await ((IReservaRepositorio)_repositorio).ObtenerPorUsuario(usuarioId).ToListAsync();
    }
    public async Task ActualizarHoraSalida(int id)
    {
        var reserva = await _repositorio.ObtenerPorId(id);
        if (reserva is not null)
        {
            reserva.HoraSalida = DateTime.UtcNow;
            _repositorio.Actualizar(reserva);
        }
    }
}