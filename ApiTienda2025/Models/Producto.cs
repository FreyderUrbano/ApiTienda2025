using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

//CREACION DEL MODELO FABRICANTE QUE ES UNA TABLA EN LA BASE DE DATOS 
namespace ApiTienda2025.Models
{
    public class Producto
    {
        [Key]
        public int Codigo { get; set; }
        public required string Nombre { get; set; }
        public double Precio { get; set; }
        public int CodigoFabricante { get; set; }
        [JsonIgnore]
        public Fabricante? Fabricantes { get; set; }
    }
}
