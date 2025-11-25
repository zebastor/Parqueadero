using Parqueadero.Models;

namespace Parqueadero.Factories.FacturaFactory;

public class FacturaElectronicaCreator : FacturaCreator
{
    protected override IFactura CrearFactura(Cobro cobro) => new FacturaElectronica(cobro);
}
