namespace Parqueadero.Strategies.Descuento;

public class NoDescuentoStrategy : IDescuentoStrategy
{
    public decimal Aplicar(Models.Descuento descuento, decimal subtotal) => subtotal;
    public bool EsValido(Models.Descuento descuento) => true;
}