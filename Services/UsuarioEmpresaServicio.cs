// En Parqueadero.Services.UsuarioEmpresaServicio.cs
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;
using Parqueadero.Models;
using Microsoft.EntityFrameworkCore;

namespace Parqueadero.Services;

public class UsuarioEmpresaServicio : GenericoServicio<UsuarioEmpresa>, IUsuarioEmpresaServicio
{
    private readonly IUsuarioEmpresaRepositorio _usuarioEmpresaRepositorio;

    public UsuarioEmpresaServicio(IUsuarioEmpresaRepositorio usuarioEmpresaRepositorio) : base(usuarioEmpresaRepositorio)
    {
        _usuarioEmpresaRepositorio = usuarioEmpresaRepositorio;
    }

    public async Task<UsuarioEmpresa?> ObtenerPorUsuarioYEmpresa(int usuarioId, int empresaId) => 
        await _usuarioEmpresaRepositorio.ObtenerPorUsuarioYEmpresa(usuarioId, empresaId);

    public async Task<IEnumerable<UsuarioEmpresa>> ObtenerPorUsuario(int usuarioId) => 
        await _usuarioEmpresaRepositorio.ObtenerPorUsuario(usuarioId).ToListAsync(); // Esto ya llama al repositorio que incluye las relaciones

    public async Task<IEnumerable<UsuarioEmpresa>> ObtenerPorEmpresa(int empresaId) => 
        await _usuarioEmpresaRepositorio.ObtenerPorEmpresa(empresaId).ToListAsync(); // Esto ya llama al repositorio que incluye las relaciones

    public async Task AsignarPlanEspecial(int usuarioId, int empresaId, int? planEspecialId)
    {
        var relacion = await _usuarioEmpresaRepositorio.ObtenerPorUsuarioYEmpresa(usuarioId, empresaId);
        
        if (relacion == null)
        {
            // Crear nueva relación si no existe
            relacion = new UsuarioEmpresa
            {
                UsuarioId = usuarioId,
                EmpresaId = empresaId,
                PlanEspecialId = planEspecialId
            };
            await _usuarioEmpresaRepositorio.Insertar(relacion);
        }
        else
        {
            // Actualizar plan existente
            relacion.PlanEspecialId = planEspecialId;
            _usuarioEmpresaRepositorio.Actualizar(relacion);
        }
    }
}