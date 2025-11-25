using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class EmpresaServicio : GenericoServicio<Empresa>, IEmpresaServicio
{
    public EmpresaServicio(IEmpresaRepositorio empresaRepository) : base(empresaRepository)
    {
    }
} 