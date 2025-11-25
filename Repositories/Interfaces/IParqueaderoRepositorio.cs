using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IParqueaderoRepositorio : IGenericoRepositorio<Models.Parqueadero>
{
    IQueryable<Models.Parqueadero> ObtenerTodoPorEmpresa(int empresaId);
}