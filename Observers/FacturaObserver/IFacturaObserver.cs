using Parqueadero.Models;

namespace Parqueadero.Observers.FacturaObserver;

public interface IFacturaObserver
{
    Task Actualizar(Recibo recibo, string detalle);
}