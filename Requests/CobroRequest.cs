namespace Parqueadero.Requests;

public class CobroRequest
{
    public string? CodigoDescuento { get; set; }
    public bool EnviarCorreo { get; set; }
}
