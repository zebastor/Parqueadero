using Parqueadero.Models;

namespace Parqueadero.Factories.FacturaFactory;

public abstract class FacturaCreator
{
    protected abstract IFactura CrearFactura(Cobro cobro);
    
    public (Recibo recibo, string detalle) Crear(Cobro cobro)
    {
        var factura = CrearFactura(cobro);
        var detalle = factura.GenerarDetalle();
        
        var recibo = new Recibo
        {
            Cobro = cobro,
            Codigo = Guid.NewGuid().ToString().Substring(0, 8),
            FechaEmision = DateTime.UtcNow,
            Enviado = false,
        };
        return (recibo, detalle);
    }
}