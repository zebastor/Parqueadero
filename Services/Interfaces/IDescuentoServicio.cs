using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface IDescuentoServicio : IGenericoServicio<Descuento>
{
    Task<Descuento?> BuscarPorCodigo(string codigo);
}
