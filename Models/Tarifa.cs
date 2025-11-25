namespace Parqueadero.Models;


public class Tarifa
{
    public int Id { get; set; }
    public TipoVehiculo TipoVehiculo { get; set; }
    public decimal ValorPorHora { get; set; }
    public int ParqueaderoId { get; set; }
}