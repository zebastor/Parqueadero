using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface IReciboServicio : IGenericoServicio<Recibo>
{
    Task<Recibo> Insertar(Cobro cobro, bool enviarCorreo);
    Task<(byte[], string, string)> GenerarComprobante(int id, string tipo);
}