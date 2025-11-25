using Microsoft.EntityFrameworkCore;

namespace Parqueadero.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Empresa> Empresas { get; set; }
    public DbSet<Parqueadero> Parqueaderos { get; set; }
    public DbSet<Piso> Pisos { get; set; }
    public DbSet<Zona> Zonas { get; set; }
    public DbSet<Informe> Informes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Tarifa> Tarifas { get; set; }
    public DbSet<Vehiculo> Vehiculos { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Descuento> Descuentos { get; set; }
    public DbSet<Cobro> Cobros { get; set; }
    public DbSet<Recibo> Recibos { get; set; }
    public DbSet<PlanEspecial> PlanesEspeciales { get; set; }
    public DbSet<UsuarioEmpresa> UsuarioEmpresas { get; set; } // Nueva entidad

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario {
                Id = 1,
                Nombre = "Super Usuario",
                Correo = "superadmin@gmail.com",
                Rol = Rol.SuperUsuario,
                Clave = "$2a$11$.Mr.yp21fY0MTHFnYY2GFeULkgKichiKCxnn6IG/ehHOhPG88Zuga"
            }
        );
        modelBuilder.Entity<Reserva>()
    .HasOne(reserva => reserva.Cobro)
    .WithOne(cobro => cobro.Reserva)
    .HasForeignKey<Cobro>(cobro => cobro.ReservaId);

        modelBuilder.Entity<Cobro>()
            .HasOne(cobro => cobro.Recibo)
            .WithOne(recibo => recibo.Cobro)
            .HasForeignKey<Recibo>("CobroId"); 

        // Configuración de la tabla intermedia UsuarioEmpresa
        modelBuilder.Entity<UsuarioEmpresa>(entity =>
        {
            entity.HasKey(ue => ue.Id);
            
            entity.HasOne(ue => ue.Usuario)
                .WithMany(u => u.UsuarioEmpresas)
                .HasForeignKey(ue => ue.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(ue => ue.Empresa)
                .WithMany() // No necesitas una colección en Empresa si no la usas
                .HasForeignKey(ue => ue.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(ue => ue.PlanEspecial)
                .WithMany()
                .HasForeignKey(ue => ue.PlanEspecialId)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Opcional: Índice compuesto para evitar duplicados de Usuario-Empresa
            entity.HasIndex(ue => new { ue.UsuarioId, ue.EmpresaId })
                .IsUnique();
        });
    }
}