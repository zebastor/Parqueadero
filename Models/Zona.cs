using System.Text.Json.Serialization;

namespace Parqueadero.Models;

public enum EstadoPlaza { Libre, Ocupada, Reservada }

public class Zona
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int PisoId { get; set; }
    public TipoVehiculo TipoVehiculo { get; set; }
    public EstadoPlaza Estado { get; set; } = EstadoPlaza.Libre;
    
    [JsonIgnore]
    public virtual Piso? Piso { get; set; }
}