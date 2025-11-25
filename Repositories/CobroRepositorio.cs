using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class CobroRepositorio : GenericoRepositorio<Cobro>, ICobroRepositorio
{
    public CobroRepositorio(ApplicationDbContext context) : base(context)
    {
    }
}