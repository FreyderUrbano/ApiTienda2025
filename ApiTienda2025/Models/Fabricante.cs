using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiTienda2025.Models
{
    [Table("fabricante")]
    public class Fabricante
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Producto>? Productos { get; set; }
    }
}
