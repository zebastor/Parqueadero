using Parqueadero.Models;

namespace Parqueadero.Strategies.Descuento;

public interface IDescuentoStrategy
{
    decimal Aplicar(Models.Descuento descuento, decimal subtotal);
    bool EsValido(Models.Descuento descuento);
}
