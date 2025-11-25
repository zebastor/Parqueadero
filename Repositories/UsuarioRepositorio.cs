using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;

namespace Parqueadero.Repositories;

public class UsuarioRepositorio : GenericoRepositorio<Usuario>, IUsuarioRepositorio
{
    public UsuarioRepositorio(ApplicationDbContext context) : base(context)
    {
    }

public async Task<Usuario?> ObtenerPorEmail(string email) => await Entities
    .Include(u => u.Empresa)
         .Include(u => u.PlanEspecial) // PlanEspecial principal (si aplica)
        .Include(u => u.UsuarioEmpresas) // Relaciones con otras empresas
            .ThenInclude(ue => ue.Empresa) // Empresa en la relación
        .Include(u => u.UsuarioEmpresas)
            .ThenInclude(ue => ue.PlanEspecial) // PlanEspecial en la relación
        .FirstOrDefaultAsync(u => u.Correo == email);

    // Modificar ObtenerTodo para incluir UsuarioEmpresas y sus relaciones
    public override IQueryable<Usuario> ObtenerTodo() => Entities
        .Include(u => u.Empresa)
        .Include(u => u.PlanEspecial)
        .Include(u => u.UsuarioEmpresas) // Añadir esta línea
            .ThenInclude(ue => ue.Empresa) // Y estas
        .Include(u => u.UsuarioEmpresas)
            .ThenInclude(ue => ue.PlanEspecial); // Y esta

    public override async Task<Usuario?> ObtenerPorId(int id) => await Entities
        .Include(u => u.Empresa)
        .Include(u => u.PlanEspecial)
        .FirstOrDefaultAsync(u => u.Id == id);

    public IQueryable<Usuario> ObtenerTodosPorEmpresa(int empresaId) => Entities
        .Where(u => u.EmpresaId == empresaId) // Usuarios con EmpresaId principal
        // Si quieres también Clientes asociados a esta empresa vía UsuarioEmpresa:
        // .Where(u => u.EmpresaId == empresaId || u.UsuarioEmpresas.Any(ue => ue.EmpresaId == empresaId))
        .Include(u => u.Empresa)
        .Include(u => u.PlanEspecial)
        .Include(u => u.UsuarioEmpresas) // Añadir esta línea
            .ThenInclude(ue => ue.Empresa) // Y estas
        .Include(u => u.UsuarioEmpresas)
            .ThenInclude(ue => ue.PlanEspecial); // Y esta

    public async Task AsignarPlanEspecial(int usuarioId, int? planEspecialId)
    {
        var usuario = await Entities.FirstOrDefaultAsync(u => u.Id == usuarioId);
        if (usuario == null)
            throw new Exception("Usuario no encontrado");

        usuario.PlanEspecialId = planEspecialId;
        await _context.SaveChangesAsync();
    }   

}