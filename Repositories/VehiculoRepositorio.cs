using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class VehiculoRepositorio : GenericoRepositorio<Vehiculo>, IVehiculoRepositorio
{
    public VehiculoRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Vehiculo?> ObtenerPorPlaca(string placa)
    {
        return await Entities
            .Include(v => v.Usuario)
            .FirstOrDefaultAsync(v => v.Placa == placa);
    }

    public IQueryable<Vehiculo> ObtenerPorUsuario(int usuarioId) 
        => Entities.Where(v => v.UsuarioId == usuarioId);

    public override IQueryable<Vehiculo> ObtenerTodo() 
        => Entities
        .Include(v => v.Usuario);
}
