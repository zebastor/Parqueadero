using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class PisosRepositorio : GenericoRepositorio<Piso>, IPisosRepositorio
{
    public PisosRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Piso?> ObtenerUltimoPisoPorParqueadero(int parqueaderoId)
    {
        return await Entities
            .Where(p => p.ParqueaderoId == parqueaderoId)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync();
    }

    public IQueryable<Piso> ObtenerTodosPorParqueadero(int parqueaderoId)
    {
        return Entities
            .Include(p => p.Zonas)
            .Where(p => p.ParqueaderoId == parqueaderoId);
    }
}
