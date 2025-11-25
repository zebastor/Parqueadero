using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface ICobroServicio
{
    Task<Cobro> GenerarCobro(int idReserva, string? codigoDescuento);
}
