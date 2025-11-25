namespace Parqueadero.Strategies.Bill;

public interface IBillStrategy
{
    byte[] GenerarFactura(string detalle);
    string TipoContenido();
    string ObtenerExtension();
}