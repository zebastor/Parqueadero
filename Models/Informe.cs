using System;
using System.ComponentModel.DataAnnotations;

namespace Parqueadero.Models
{
    public class Informe
    {
        public int Id { get; set; }
        
        [Required]
        public string Tipo { get; set; } = string.Empty;
        
        public string Titulo { get; set; } = string.Empty;
        
        public string Descripcion { get; set; } = string.Empty;
        
        public DateTime FechaInicio { get; set; }
        
        public DateTime FechaFin { get; set; }
        
        public string Formato { get; set; } = string.Empty;
        
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        
        public string Datos { get; set; } = string.Empty;
        
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    }
}
