using Parqueadero.Models;

namespace Parqueadero.Strategies.Tarifa;

public interface ITarifaStrategy
{
    decimal Calcular(Models.Tarifa tarifa, Reserva reserva);
}
