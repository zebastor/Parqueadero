using Parqueadero.Models;

namespace Parqueadero.Observers.FacturaObserver;

public class FacturaNotifier : IFacturaNotifier
{
    private readonly IEnumerable<IFacturaObserver> _observadores;

    public FacturaNotifier(IEnumerable<IFacturaObserver> observadores)
    {
        _observadores = observadores;
    }

    public async Task NotificarFactura(Recibo recibo, string detalle)
    {
        foreach (var observador in _observadores)
        {
            await observador.Actualizar(recibo, detalle);
        }
    }
}