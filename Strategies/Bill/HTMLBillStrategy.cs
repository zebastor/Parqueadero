namespace Parqueadero.Strategies.Bill;

public class HTMLBillStrategy : IBillStrategy
{
    public byte[] GenerarFactura(string detalle)
    {
        var html = $"<html><body>{detalle}</body></html>";
        return System.Text.Encoding.UTF8.GetBytes(html);
    }

    public string TipoContenido() => "text/html";
    public string ObtenerExtension() => "html";
}
