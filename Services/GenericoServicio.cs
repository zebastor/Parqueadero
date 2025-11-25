using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public abstract class GenericoServicio<T> : IGenericoServicio<T>
{
    protected readonly IGenericoRepositorio<T> _repositorio;

    protected GenericoServicio(IGenericoRepositorio<T> repositorio)
    {
        _repositorio = repositorio;
    }
    

    public virtual async Task Actualizar(T valor)
        {
            await _repositorio.Actualizar(valor); // <-- Ahora sí es await
        }

    public virtual Task<bool> Eliminar(int id)
    {
        var resultado = _repositorio.Eliminar(id);
        return resultado;
    }

    public virtual Task<T> Insertar(T valor)
    {
        var nuevoValor = _repositorio.Insertar(valor);
        return nuevoValor;
    }

    public virtual async Task<T?> ObtenerPorId(int id) => await _repositorio.ObtenerPorId(id);

    public virtual async Task<IEnumerable<T>> ObtenerTodo() => await _repositorio.ObtenerTodo().ToListAsync();
}