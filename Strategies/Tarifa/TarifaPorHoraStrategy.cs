using Parqueadero.Models;

namespace Parqueadero.Strategies.Tarifa;

public class TarifaPorHoraStrategy : ITarifaStrategy
{
    public decimal Calcular(Models.Tarifa tarifa, Reserva reserva)
    {
        if (!reserva.HoraSalida.HasValue)
            throw new ArgumentException("La reserva debe tener una hora de salida.");
        
        var horas = (decimal)(reserva.HoraSalida.Value - reserva.HoraEntrada).TotalHours;
        horas = Math.Ceiling(horas);

        return horas * tarifa.ValorPorHora;
    }
}
