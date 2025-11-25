namespace Parqueadero.Services.Interfaces;

public interface IParqueaderoServicio : IGenericoServicio<Models.Parqueadero>
{
    Task<IEnumerable<Models.Parqueadero>> ObtenerTodoPorEmpresa(int empresaId);
}