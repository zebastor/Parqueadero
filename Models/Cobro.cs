namespace Parqueadero.Models;

public class Cobro
{
    public int Id { get; set; }
    public int ReservaId { get; set; }
    public int TarifaId { get; set; }
    public int? DescuentoId { get; set; }
    public DateTime FechaCobro { get; set; } = DateTime.Now;
    public decimal Total { get; set; }

    public Reserva? Reserva { get; set; }
    public Tarifa? Tarifa { get; set; }
    public Descuento? Descuento { get; set; }

    public virtual Recibo? Recibo { get; set; }
}