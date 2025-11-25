namespace Parqueadero.Models;

public class Descuento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Porcentaje { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaExpiracion { get; set; }
}