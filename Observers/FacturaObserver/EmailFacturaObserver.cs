using Parqueadero.Models;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Observers.FacturaObserver;

public class EmailFacturaObserver : IFacturaObserver
{
    private readonly IEmailServicio _emailServicio;

    public EmailFacturaObserver(IEmailServicio emailServicio)
    {
        _emailServicio = emailServicio;
    }
    
    public async Task Actualizar(Recibo recibo, string detalle)
    {
        var correo = recibo.Cobro.Reserva!.Usuario!.Correo;
        if (!string.IsNullOrEmpty(correo))
        {
            await _emailServicio.Enviar(correo, "Factura Electronica", detalle);
        }
    }
}