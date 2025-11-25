using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class EmpresaRepositorio : GenericoRepositorio<Empresa>, IEmpresaRepositorio
{
    public EmpresaRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Empresa?> ObtenerPorNit(string nit)
    {
        return await _context.Empresas
            .FirstOrDefaultAsync(e => e.Nit == nit);
    }
} 