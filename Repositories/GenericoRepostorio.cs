using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public abstract class GenericoRepositorio<T> : IGenericoRepositorio<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected DbSet<T> Entities => _context.Set<T>();

    protected GenericoRepositorio(ApplicationDbContext context)
    {
        _context = context;
    }

    public virtual async Task<T> Insertar(T valor)
    {
        await Entities.AddAsync(valor);
        await _context.SaveChangesAsync();
        return valor;
    }

    public virtual async Task<T?> ObtenerPorId(int id)  => await Entities.FindAsync(id);

    public virtual IQueryable<T> ObtenerTodo() => Entities;

public async Task Actualizar(T valor)
{
    _context.Set<T>().Update(valor);
    await _context.SaveChangesAsync();
}



    public virtual async Task<bool> Eliminar(int id)
    {
        var entidad = await ObtenerPorId(id);
        if (entidad == null) return false;
        
        Entities.Remove(entidad);
        await _context.SaveChangesAsync();
        return true;
    }
}