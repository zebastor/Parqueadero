using Parqueadero.Models;

namespace Parqueadero.Factories.FacturaFactory;

public class FacturaFisicaCreator : FacturaCreator
{
    protected override IFactura CrearFactura(Cobro cobro) => new FacturaFisica(cobro);
}
