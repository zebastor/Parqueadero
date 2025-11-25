using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class PlanEspecialServicio : GenericoServicio<PlanEspecial>, IPlanEspecialServicio
{
    public PlanEspecialServicio(IPlanEspecialRepositorio repositorio) : base(repositorio)
    {
    }
}
