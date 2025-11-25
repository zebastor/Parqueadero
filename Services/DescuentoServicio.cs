using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class DescuentoServicio : GenericoServicio<Descuento>, IDescuentoServicio
{
    public DescuentoServicio(IDescuentoRepositorio repositorio) : base(repositorio) { }

    public async Task<Descuento?> BuscarPorCodigo(string codigo) => await ((IDescuentoRepositorio)_repositorio).BuscarPorCodigo(codigo);
}