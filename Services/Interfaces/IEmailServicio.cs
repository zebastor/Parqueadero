namespace Parqueadero.Services.Interfaces;

public interface IEmailServicio
{
    Task Enviar(string correo, string asunto, string cuerpo);
}
