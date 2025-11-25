namespace Parqueadero.Models;

public class Piso
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public int ParqueaderoId { get; set; }
    public ICollection<Zona> Zonas { get; set; } = [];
}