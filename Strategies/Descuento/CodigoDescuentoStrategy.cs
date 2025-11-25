namespace Parqueadero.Strategies.Descuento;

public class CodigoDescuentoStrategy : IDescuentoStrategy
{
    public decimal Aplicar(Models.Descuento descuento, decimal subtotal)
    {
        if (!EsValido(descuento))
            return subtotal;
        return subtotal * (1 - descuento.Porcentaje / 100m);
    }
    public bool EsValido(Models.Descuento descuento) => !string.IsNullOrEmpty(descuento.Codigo)
                   && descuento.FechaExpiracion >= DateTime.UtcNow;
}
