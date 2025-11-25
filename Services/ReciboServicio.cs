using Parqueadero.Factories.FacturaFactory;
using Parqueadero.Models;
using Parqueadero.Observers.FacturaObserver;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;
using Parqueadero.Strategies.Bill;
using Parqueadero.Builder;

namespace Parqueadero.Services;

public class ReciboServicio : GenericoServicio<Recibo>, IReciboServicio
{
    private readonly IFacturaNotifier _facturaNotifier;
    private readonly IBillStrategySelector _billStrategySelector;
    private readonly IContentBuilder _contentBuilder;
    
    public ReciboServicio(IReciboRepositorio repositorio, IFacturaNotifier facturaNotifier, IBillStrategySelector billStrategySelector, IContentBuilder contentBuilder) : base(repositorio)
    {
        _facturaNotifier = facturaNotifier;
        _billStrategySelector = billStrategySelector;
        _contentBuilder = contentBuilder;
    }

    public async Task<Recibo> Insertar(Cobro cobro, bool enviarCorreo)
    {

        FacturaCreator factory = enviarCorreo ? new FacturaElectronicaCreator() : new FacturaFisicaCreator();
        var (recibo, detalle) = factory.Crear(cobro);
        await Insertar(recibo);
        recibo = await ObtenerPorId(recibo.Id);
        if (enviarCorreo)
        {
            _facturaNotifier.NotificarFactura(recibo!, detalle);
            recibo!.Enviado = true;
            Actualizar(recibo!);
        }   
        return recibo!;
    }

    public async Task<(byte[], string, string)> GenerarComprobante(int id, string tipo)
    {
        var recibo = await ObtenerPorId(id);
        if (recibo == null)
        {
            throw new ArgumentException("Recibo no encontrado");
        }
        var strategy = _billStrategySelector.GetStrategy(tipo);
        var content = _contentBuilder
                        .AddTitle("Recibo")
                        .AddParagraph("Detalle del recibo")
                        .AddPrice(recibo.Cobro.Total)
                        .AddDate(recibo.FechaEmision)
                        .AddUser(recibo.Cobro.Reserva.Usuario!.Nombre)
                        .AddNumberBill(recibo.Codigo)
                        .BuildContent();
        return (strategy.GenerarFactura(content), strategy.TipoContenido(), strategy.ObtenerExtension());
    }
}