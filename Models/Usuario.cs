namespace Parqueadero.Models;

public enum Rol 
{
    SuperUsuario,
    Administrador,
    Trabajador,
    Cliente
}

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public Rol Rol { get; set; } = Rol.Cliente;
    public string Clave { get; set; } = string.Empty;
    
    // Estas propiedades se mantienen para roles no Cliente
    public int? EmpresaId { get; set; } // Para Trabajador/Administrador
    public int? PlanEspecialId { get; set; } // Puede mantenerse si es necesario para otros roles

    public Empresa? Empresa { get; set; }
    public PlanEspecial? PlanEspecial { get; set; }
    
    // Nueva colección para la relación muchos a muchos
    public virtual ICollection<UsuarioEmpresa> UsuarioEmpresas { get; set; } = new List<UsuarioEmpresa>();
}