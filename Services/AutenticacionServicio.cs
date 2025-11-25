using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class AutenticacionServicio : IAutenticacionServicio
{
    private readonly IEncriptacionService _encriptacionService;
    private readonly IUsuarioRepositorio _usuarioRepositorio;

    public AutenticacionServicio(IEncriptacionService encriptacionService, IUsuarioRepositorio usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _encriptacionService = encriptacionService;
    }

    public async Task<Usuario?> AutenticarUsuario(string email, string password)
    {
        var usuario = await _usuarioRepositorio.ObtenerPorEmail(email);
        if (usuario == null)
            return await Task.FromResult<Usuario?>(null);

        if (!_encriptacionService.Verificar(password, usuario.Clave))
            return await Task.FromResult<Usuario?>(null);
        
        return usuario;
    }

    public async Task<Usuario?> ObtenerUsuarioPorEmail(string email)
    {
        var usuario = await _usuarioRepositorio.ObtenerPorEmail(email);
        return usuario;
    }

    public async Task<Usuario> RegistrarUsuario(Usuario usuario)
    {
        usuario.Clave = _encriptacionService.Encriptar(usuario.Clave);
        var usuarioCreado = await _usuarioRepositorio.Insertar(usuario);
        return usuarioCreado;
    }

}