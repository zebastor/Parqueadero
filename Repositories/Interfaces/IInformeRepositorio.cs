using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces
{
    public interface IInformeRepositorio : IGenericoRepositorio<Informe>
    {
        Task<IEnumerable<Informe>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Informe>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Cobro>> ObtenerCobrosParaInformeAsync(DateTime fechaInicio, DateTime fechaFin, int? parqueaderoId = null);
        Task<IEnumerable<Reserva>> ObtenerReservasParaInformeAsync(DateTime fechaInicio, DateTime fechaFin, int? parqueaderoId = null);
        Task<decimal> ObtenerIngresosTotalesAsync(DateTime fechaInicio, DateTime fechaFin, int? parqueaderoId = null);
        Task<int> ObtenerTotalVehiculosAsync(DateTime fechaInicio, DateTime fechaFin, int? parqueaderoId = null);
    }
}