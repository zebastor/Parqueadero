using Parqueadero.Models;

namespace Parqueadero.Strategies.Tarifa;

public class TarifaPorFinDeSemanaStrategy : ITarifaStrategy
{
    public decimal Calcular(Models.Tarifa tarifa, Reserva reserva)
    {
        if (!reserva.HoraSalida.HasValue)
            throw new InvalidOperationException("La reserva aún no tiene hora de salida.");

        var inicio = reserva.HoraEntrada;
        var fin    = reserva.HoraSalida.Value;
        decimal total = 0m;

        var hora = inicio;
        while (hora < fin)
        {
            var siguiente = hora.AddHours(1);
            var factor = (hora.DayOfWeek == DayOfWeek.Saturday || hora.DayOfWeek == DayOfWeek.Sunday)
                            ? 1.5m
                            : 1m;
            total += tarifa.ValorPorHora * factor;
            hora = siguiente;
        }
        return total;
    }
}
