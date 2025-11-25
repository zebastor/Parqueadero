namespace Parqueadero.Repositories.Interfaces;

public interface IGenericoRepositorio<T>
{
    Task<T> Insertar(T valor);
    Task<T?> ObtenerPorId(int id);
    IQueryable<T> ObtenerTodo();
    Task Actualizar(T valor);
    Task<bool> Eliminar(int id);
}