namespace Parqueadero.Services.Interfaces;

public interface IEncriptacionService
{
    string Encriptar(string texto);
    bool Verificar(string texto, string hash);
} 