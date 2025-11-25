// En Parqueadero.Repositories.UsuarioEmpresaRepositorio.cs
using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class UsuarioEmpresaRepositorio : GenericoRepositorio<UsuarioEmpresa>, IUsuarioEmpresaRepositorio
{
    public UsuarioEmpresaRepositorio(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UsuarioEmpresa?> ObtenerPorUsuarioYEmpresa(int usuarioId, int empresaId) => 
        await Entities
            .Include(ue => ue.Usuario)
            .Include(ue => ue.Empresa)
            .Include(ue => ue.PlanEspecial) // Asegurar que PlanEspecial se incluya
            .FirstOrDefaultAsync(ue => ue.UsuarioId == usuarioId && ue.EmpresaId == empresaId);

    public IQueryable<UsuarioEmpresa> ObtenerPorUsuario(int usuarioId) => 
        Entities
            .Where(ue => ue.UsuarioId == usuarioId)
            .Include(ue => ue.Usuario)
            .Include(ue => ue.Empresa)
            .Include(ue => ue.PlanEspecial); // Asegurar que PlanEspecial se incluya

    public IQueryable<UsuarioEmpresa> ObtenerPorEmpresa(int empresaId) => 
        Entities
            .Where(ue => ue.EmpresaId == empresaId)
            .Include(ue => ue.Usuario)
            .Include(ue => ue.Empresa)
            .Include(ue => ue.PlanEspecial); // Asegurar que PlanEspecial se incluya
}