using Parqueadero.Models;

namespace Parqueadero.Observers.FacturaObserver;

public interface IFacturaNotifier
{
    Task NotificarFactura(Recibo recibo, string detalle);
}
