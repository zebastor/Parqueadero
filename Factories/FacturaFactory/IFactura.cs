using Parqueadero.Models;

namespace Parqueadero.Factories.FacturaFactory;

public interface IFactura
{
    Cobro Cobro { get; }
    string GenerarDetalle();
}