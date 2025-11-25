using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IDescuentoRepositorio : IGenericoRepositorio<Descuento> 
{
    Task<Descuento?> BuscarPorCodigo(string codigo);
}
