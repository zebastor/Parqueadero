using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories
{
    public class InformeRepositorio : GenericoRepositorio<Informe>, IInformeRepositorio
    {
        public InformeRepositorio(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Informe>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _context.Informes
                .AsNoTracking()
                .Where(i => i.UsuarioId == usuarioId)
                .OrderByDescending(i => i.FechaGeneracion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Informe>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Informes
                .AsNoTracking()
                .Where(i => i.FechaGeneracion >= fechaInicio && i.FechaGeneracion <= fechaFin)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cobro>> ObtenerCobrosParaInformeAsync(DateTime fechaInicio, DateTime fechaFin, int? parqueaderoId = null)
        {
            var query = _context.Cobros
                .AsNoTracking()
                .Include(c => c.Reserva)
                    .ThenInclude(r => r!.Vehiculo)
                .Include(c => c.Tarifa)
                .Include(c => c.Descuento)
                .Where(c => c.FechaCobro >= fechaInicio && c.FechaCobro <= fechaFin);

            if (parqueaderoId.HasValue)
            {
                query = query.Where(c => c.Reserva != null && c.Reserva.Zona != null && c.Reserva.Zona.Piso != null && c.Reserva.Zona.Piso.ParqueaderoId == parqueaderoId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> ObtenerReservasParaInformeAsync(DateTime fechaInicio, DateTime fechaFin, int? parqueaderoId = null)
        {
            return await _context.Reservas
                .AsNoTracking()
                .Include(r => r.Vehiculo)
                .Include(r => r.Zona)
                .Where(r => r.HoraEntrada >= fechaInicio && r.HoraEntrada <= fechaFin)
                .ToListAsync();
        }

        public async Task<decimal> ObtenerIngresosTotalesAsync(DateTime fechaInicio, DateTime fechaFin, int? parqueaderoId = null)
        {
            return await _context.Cobros
                .Where(c => c.FechaCobro >= fechaInicio && c.FechaCobro <= fechaFin)
                .SumAsync(c => c.Total);
        }

        public async Task<int> ObtenerTotalVehiculosAsync(DateTime fechaInicio, DateTime fechaFin, int? parqueaderoId = null)
        {
            return await _context.Reservas
                .Where(r => r.HoraEntrada >= fechaInicio && r.HoraEntrada <= fechaFin)
                .Select(r => r.VehiculoId)
                .Distinct()
                .CountAsync();
        }
    }
}