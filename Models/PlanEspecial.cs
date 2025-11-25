namespace Parqueadero.Models;

public class PlanEspecial
{
    public int Id { get; set; }
    public required string Nombre { get; set; } 
    public required string Descripcion { get; set; } 
    public decimal Costo { get; set; }
}