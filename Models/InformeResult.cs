namespace Parqueadero.Models
{
    public class InformeResult
    {
        public string Contenido { get; set; } = string.Empty;
        public byte[] Archivo { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
    }
}
