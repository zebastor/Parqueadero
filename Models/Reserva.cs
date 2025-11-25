namespace Parqueadero.Models;

public enum EstadoReserva { Activa, Finalizada, Cancelada }

public class Reserva
{
    public int Id { get; set; }
    public int ZonaId { get; set; }
    public int UsuarioId { get; set; }
    public int VehiculoId { get; set; }
    public DateTime HoraEntrada { get; set; }
    public DateTime? HoraSalida { get; set; }
    public EstadoReserva Estado { get; set; } = EstadoReserva.Activa;

    public byte[]? ImagenPlaca { get; set; }
    public virtual Zona? Zona { get; set; }
    public virtual Usuario? Usuario { get; set; }
    public virtual Vehiculo? Vehiculo { get; set; }
    public virtual Cobro? Cobro { get; set; }
}