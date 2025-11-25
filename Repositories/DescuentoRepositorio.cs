using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class DescuentoRepositorio : GenericoRepositorio<Descuento>, IDescuentoRepositorio
{
    public DescuentoRepositorio(ApplicationDbContext context) : base(context) { }

    public Task<Descuento?> BuscarPorCodigo(string codigo) => Entities.FirstOrDefaultAsync(d => d.Codigo == codigo);
}
