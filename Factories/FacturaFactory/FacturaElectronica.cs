using Parqueadero.Models;

namespace Parqueadero.Factories.FacturaFactory;

public class FacturaElectronica : IFactura
{
    public Cobro Cobro { get; }

    public FacturaElectronica(Cobro cobro)
    {
        Cobro = cobro;
    }

    public string GenerarDetalle() => $"""
        <html>
        <body>
            <h1>Factura Electrónica</h1>
            <p>Total: {Cobro.Total:C}</p>
            <p>Fecha: {Cobro.FechaCobro}</p>
            <p>Cliente: {Cobro.Reserva.Usuario?.Correo}</p>
        </body>
        </html>
        """;
}
