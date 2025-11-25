using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class ReciboRepositorio : GenericoRepositorio<Recibo>, IReciboRepositorio
{
    public ReciboRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public override IQueryable<Recibo> ObtenerTodo()
        => Entities
            .Include(r => r.Cobro);

    public override async Task<Recibo?> ObtenerPorId(int id)
        => await Entities
            .Include(r => r.Cobro)
                .ThenInclude(c => c.Reserva)
                    .ThenInclude(r => r!.Usuario)
            .FirstOrDefaultAsync(r => r.Id == id);
}