using Parqueadero.Models;

namespace Parqueadero.Factories.FacturaFactory;

public class FacturaFisica : IFactura
{
    public Cobro Cobro { get; }

    public FacturaFisica(Cobro cobro)
    {
        Cobro = cobro;
    }

    public string GenerarDetalle() =>  $"""
        <html>
        <body>
            <h1>Factura Fisica</h1>
            <p>Total: {Cobro.Total:C}</p>
            <p>Fecha: {Cobro.FechaCobro}</p>
        </body>
        </html>
        """;
}