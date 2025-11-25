using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class PlanEspecialRepositorio : GenericoRepositorio<PlanEspecial>, IPlanEspecialRepositorio
{
    public PlanEspecialRepositorio(ApplicationDbContext context) : base(context)
    {
    }
}