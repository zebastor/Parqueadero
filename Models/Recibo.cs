namespace Parqueadero.Models;

public class Recibo
{
    public int Id { get; set; }
    public required Cobro Cobro { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; } = DateTime.Now;
    public bool Enviado { get; set; } = false;
}