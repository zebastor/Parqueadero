using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class PisoServicio : GenericoServicio<Piso>, IPisoServicio
{
    public PisoServicio(IPisosRepositorio repositorio) : base(repositorio)
    {
    }

    public async Task<Piso?> ObtenerUltimoPisoPorParqueadero(int parqueaderoId)
    {
        return await ((IPisosRepositorio)_repositorio).ObtenerUltimoPisoPorParqueadero(parqueaderoId);
    }

    public async Task<IEnumerable<Piso>> ObtenerTodosPorParqueadero(int parqueaderoId)
    {
        return await ((IPisosRepositorio)_repositorio).ObtenerTodosPorParqueadero(parqueaderoId).ToListAsync();
    }

    public async Task<Piso> InsertarPiso(int parqueaderoId)
    {
        var ultimoPiso = await ObtenerUltimoPisoPorParqueadero(parqueaderoId);
        var numero = ultimoPiso != null ? ultimoPiso.Numero + 1 : 1;
        var piso = new Piso
        {
            ParqueaderoId = parqueaderoId,
            Numero = numero
        };
        return await Insertar(piso);
    }
}