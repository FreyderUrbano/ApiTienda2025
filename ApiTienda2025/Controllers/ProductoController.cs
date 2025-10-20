using ApiTienda2025.DTO.Producto;
using ApiTienda2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiTienda2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly TiendaContext _context;

        public ProductoController(TiendaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductos()
        {
            return await _context.Producto
                .Include(p => p.Fabricantes)
                .Select(p => new ProductoDTO
                {
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Nombre_Fabricante = p.Fabricantes.Nombre
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProductos(int id)
        {
            var producto = await _context.Producto
                .Include(p => p.Fabricantes)
                .Select(p => new ProductoDTO
                {
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Codigo_Fabricante = p.CodigoFabricante,
                    Nombre_Fabricante = p.Fabricantes.Nombre
                })
                .FirstOrDefaultAsync(p => p.Codigo == id);

            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }

        [HttpGet("Fabricante/{fabricanteName}")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductosByFabricante(string fabricanteName)
        {
            var listaProductos = await _context.Producto
                .Include(p => p.Fabricantes)
                .Where(p => p.Fabricantes.Nombre.Contains(fabricanteName))
                .Select(p => new ProductoDTO
                {
                    Codigo = p.Codigo,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Codigo_Fabricante = p.CodigoFabricante,
                    Nombre_Fabricante = p.Fabricantes.Nombre,
                })
                .ToListAsync();

            if (listaProductos == null || listaProductos.Count == 0)
            {
                return NotFound();
            }
            return Ok(listaProductos);
        }

        /*uso del procedimiento almacenado creado en postgresss*/
        [HttpPost("insertar")]
        public async Task<ActionResult> InsertarProducto([FromBody] Producto producto)
        {
            try
            {
                var procedimientoAlmacenadoSql = "CALL insertar_producto(@p_nombre, @p_precio, @p_codigofabricante)";
                var parameters = new[]
                {
                    new Npgsql.NpgsqlParameter("@p_nombre", producto.Nombre),
                    new Npgsql.NpgsqlParameter("@p_precio", producto.Precio),
                    new Npgsql.NpgsqlParameter("@p_codigofabricante", producto.CodigoFabricante)
                };

                await _context.Database.ExecuteSqlRawAsync(procedimientoAlmacenadoSql, parameters);
                return Ok(new { mensaje = "PROCEDIMIENTO ENVIADO CON EXITO" });

            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

            [HttpPost]
            public async Task<ActionResult<Producto>> CreateProduct(ProductoCreateDTO createProducto)
            {
                var producto = new Producto
                {
                    Nombre = createProducto.Nombre,
                    Precio = createProducto.Precio,
                    CodigoFabricante = createProducto.Codigo_Fabricante,
                };
                _context.Producto.Add(producto);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProductos), new { id = producto.Codigo }, producto);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProducto(int id, ProductoCreateDTO productoCreateDTO)
            {
                var producto = await _context.Producto.FindAsync(id);
                if (producto == null)
                {
                    return NotFound();
                }
                producto.Nombre = productoCreateDTO.Nombre;
                _context.Entry(producto).State = EntityState.Modified;

                try
                {
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteProducto(int id)
            {
                var producto = await _context.Producto.FindAsync();
                if (producto == null)
                {
                    return NotFound();
                }
                _context.Producto.Remove(producto);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            private bool ProductoExists(int id)
            {
                return _context.Producto.Any(e => e.Codigo == id);
            }
        }
    }
