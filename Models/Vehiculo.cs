namespace Parqueadero.Models;

public class Vehiculo{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public TipoVehiculo TipoVehiculo { get; set; }
    
    public Usuario? Usuario { get; set; }
}