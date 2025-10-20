using Microsoft.EntityFrameworkCore;

namespace ApiTienda2025.Models
{
    //DECLARA LA CLASE TIENDACONTEXT QUE HEREDA DE DBCONTEXT
    public class TiendaContext : DbContext
    {
        //CONSTRUCTOR 
        /*
         Constructor que recibe DbContextOptions<TiendaContext> (configuración del contexto, 
         p. ej. connection string, proveedor, opciones).
         Llama a base(options) para que la implementación base (DbContext) reciba esas opciones.
         Esto permite inyectar el contexto desde Startup / Program.cs con AddDbContext<TiendaContext>
        */
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options) { }
        /*
         Define un DbSet para la entidad Fabricante. DbSet representa la colección de esa entidad en
         la base de datos: EF lo mapea a la tabla Fabricantes (o fabricante según convenciones/config).
         Te permite hacer context.Fabricantes.Add(...), context.Fabricantes.ToList(), etc.
        */
        public DbSet<Fabricante> Fabricante { get; set; }  

        //Igual para Producto. Permite consultar y persistir Producto desde/in DB.
        public DbSet<Producto> Producto { get; set; }

        /*
       OnModelCreating es un método especial de Entity Framework Core que se ejecuta una sola vez cuando el contexto 
       (TiendaContext) se inicializa por primera vez. Su propósito es configurar el modelo de datos que EF Core va a
       mapear hacia la base de datos. En otras palabras: aquí defines las reglas, relaciones y configuraciones 
       especiales de tus entidades (Producto, Fabricante, etc.)
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fabricante>().ToTable("fabricante");
            modelBuilder.Entity<Producto>().ToTable("producto");

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Fabricantes)
                .WithMany(f => f.Productos)
                .HasForeignKey(p => p.CodigoFabricante);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Codigo)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Fabricante>()
                .Property(p => p.Codigo)
                .ValueGeneratedOnAdd();
        }

    }
}
