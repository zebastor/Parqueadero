using Parqueadero.Models;

namespace Parqueadero.Repositories.Interfaces;

public interface IEmpresaRepositorio : IGenericoRepositorio<Empresa>
{
    Task<Empresa?> ObtenerPorNit(string nit);
} 