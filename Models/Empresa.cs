using System.Text.Json.Serialization;

namespace Parqueadero.Models;

public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Nit { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<Parqueadero> Parqueaderos { get; set; } = [];
}