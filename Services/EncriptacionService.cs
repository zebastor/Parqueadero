using BCrypt.Net;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class EncriptacionService : IEncriptacionService
{
    public string Encriptar(string texto)
    {
        return BCrypt.Net.BCrypt.HashPassword(texto);
    }

    public bool Verificar(string texto, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(texto, hash);
    }
} 