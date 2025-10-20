using System.ComponentModel.DataAnnotations;

namespace ApiTienda2025.Models
{
    public class NuevaClase
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
