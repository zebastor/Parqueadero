namespace Parqueadero.Models;

public class UsuarioEmpresa
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int EmpresaId { get; set; }
    public int? PlanEspecialId { get; set; }

    public Usuario? Usuario { get; set; }
    public Empresa? Empresa { get; set; }
    public PlanEspecial? PlanEspecial { get; set; }
}