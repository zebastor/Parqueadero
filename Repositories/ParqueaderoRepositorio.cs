using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class ParqueaderoRepositorio : GenericoRepositorio<Models.Parqueadero>, IParqueaderoRepositorio
{
    private readonly IPisosRepositorio _pisosRepositorio;
    public ParqueaderoRepositorio(ApplicationDbContext context, IPisosRepositorio pisosRepositorio) : base(context)
    {
        _pisosRepositorio = pisosRepositorio;
    }

    public override async Task<Models.Parqueadero?> ObtenerPorId(int id)
    {
        return await _context.Parqueaderos
            .Include(p => p.Empresa)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override IQueryable<Models.Parqueadero> ObtenerTodo()
    {
        return _context.Parqueaderos
            .Include(p => p.Empresa);
    }

    public IQueryable<Models.Parqueadero> ObtenerTodoPorEmpresa(int empresaId)
    {
        return _context.Parqueaderos
            .Include(p => p.Empresa)
            .Where(p => p.EmpresaId == empresaId);
    }
    
}