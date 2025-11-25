using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class ReservaRepositorio : GenericoRepositorio<Reserva>, IReservaRepositorio
{
    public ReservaRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Reserva?> ObtenerPorPlacaConEstadoActivo(string placa)
        => await Entities
            .Include(r => r.Vehiculo)
            .Where(r => r.Vehiculo != null && r.Vehiculo.Placa == placa && r.Estado == EstadoReserva.Activa)
            .FirstOrDefaultAsync();

    public override IQueryable<Reserva> ObtenerTodo()
    => Entities
        .Include(r => r.Vehiculo)
        .Include(r => r.Zona)
        .Include(r => r.Usuario)
            .ThenInclude(u => u.UsuarioEmpresas)
            .ThenInclude(ue => ue.PlanEspecial)
        .Include(r => r.Cobro) 
            .ThenInclude(c => c.Recibo);

    // --- CORREGIDO ---
    // Cambiamos la forma de buscar el PlanEspecial
    public override async Task<Reserva?> ObtenerPorId(int id)
      => await Entities
          .Include(r => r.Vehiculo)
          .Include(r => r.Zona)
              .ThenInclude(z => z.Piso) // <-- AÑADE ESTA LÍNEA (O ASEGÚRATE DE QUE ESTÉ)
          .Include(r => r.Usuario)
              .ThenInclude(u => u.UsuarioEmpresas)
              .ThenInclude(ue => ue.PlanEspecial)
          .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<bool> CancelarReserva(int id)
    {
        var reserva = await ObtenerPorId(id) ?? throw new Exception("Reserva no encontrada");
        reserva.Estado = EstadoReserva.Cancelada;

        if (reserva.Zona != null)
        {
            reserva.Zona.Estado = EstadoPlaza.Libre;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> FinalizarReserva(int id)
    {
        var reserva = await ObtenerPorId(id) ?? throw new Exception("Reserva no encontrada");
        reserva.Estado = EstadoReserva.Finalizada;
        reserva.HoraSalida = DateTime.UtcNow;

        if (reserva.Zona != null)
        {
            reserva.Zona.Estado = EstadoPlaza.Libre;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public IQueryable<Reserva> ObtenerPorUsuario(int usuarioId)
    {
        return Entities
            .Where(r => r.UsuarioId == usuarioId)
            .Include(r => r.Usuario)
            .Include(r => r.Zona)
            .Include(r => r.Vehiculo)
            .Include(r => r.Cobro) 
                .ThenInclude(c => c.Recibo) 
            .OrderByDescending(r => r.HoraEntrada);
    }
}