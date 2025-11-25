using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces;

public interface IAutenticacionServicio
{
    Task<Usuario> RegistrarUsuario(Usuario usuario);
    Task<Usuario?> AutenticarUsuario(string email, string password);
    Task<Usuario?> ObtenerUsuarioPorEmail(string email);
}
