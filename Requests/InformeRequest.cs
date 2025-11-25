using System;
using System.ComponentModel.DataAnnotations;

namespace Parqueadero.Models
{
    public class InformeRequest
    {
        [Required]
        public string Tipo { get; set; } = string.Empty;
        
        [Required]
        public DateTime FechaInicio { get; set; }
        
        [Required]
        public DateTime FechaFin { get; set; }
        
        [Required]
        public string Formato { get; set; } = string.Empty;
        
        public int? ParqueaderoId { get; set; }
    }
}
