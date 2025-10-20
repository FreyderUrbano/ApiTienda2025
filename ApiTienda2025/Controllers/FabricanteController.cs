using ApiTienda2025.DTO.Fabricante;
using ApiTienda2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiTienda2025.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FabricanteController : ControllerBase
    {
        private readonly TiendaContext _context;

        //Constructor
        public FabricanteController(TiendaContext context)
        {
            _context = context;
        }

        //Funcion GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FabricanteDTO>>> GetFabricante()
        {
            return await _context.Fabricante
                .Include(f => f.Productos)
                .Select(f => new FabricanteDTO
                {
                    Codigo = f.Codigo,
                    Nombre = f.Nombre
                })
                .ToListAsync();

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FabricanteDTO>> GetFabricante(int id)
        {
            var fabricante = await _context.Fabricante
                .Include(f => f.Productos)
                .Select(f => new FabricanteDTO
                {
                    Codigo = f.Codigo,
                    Nombre = f.Nombre
                })
                .FirstOrDefaultAsync(f => f.Codigo == id);

            if (fabricante == null)
            {
                return NotFound();
            }
            return fabricante;
        }

        [HttpPost("insertar")]
        public async Task<ActionResult> InsertarProducto([FromBody] Producto producto)
        {
            try
            {
                var procedimientoAlmacenadoSql = "CALL insertar_producto(@p_nombre, @p_precio, @p_codigoFabricante)";
                var parameters = new[]
                {
            new Npgsql.NpgsqlParameter("@p_nombre", producto.Nombre),
            new Npgsql.NpgsqlParameter("@p_precio", producto.Precio),
            new Npgsql.NpgsqlParameter("@p_codigofabricante", producto.CodigoFabricante)
        };

                await _context.Database.ExecuteSqlRawAsync(procedimientoAlmacenadoSql, parameters);

                return Ok(new { mensaje = "✅ PRODUCTOS INSERTADOS CON EXITO" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<ActionResult<Fabricante>> CreateFabricante(FabricanteCreateDTO fabricanteDTO)
        {
            var fabricante = new Fabricante
            {
                Nombre = fabricanteDTO.Nombre,
            };
            _context.Fabricante.Add(fabricante);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFabricante), new { id = fabricante.Codigo }, fabricante);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFabricante(int id, FabricanteCreateDTO fabricanteDTO)
        {
            var fabricante = await _context.Fabricante.FindAsync(id);

            if (fabricante == null)
            {
                return NotFound();
            }
            fabricante.Nombre = fabricanteDTO.Nombre;
            _context.Entry(fabricante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FabicanteExists(id))
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
        public async Task<ActionResult> DeleteFabricante(int id)
        {
            var fabricante = await _context.Fabricante.FindAsync(id);
            if (fabricante == null)
            {
                return NotFound();
            }
            _context.Fabricante.Remove(fabricante);
            await _context.SaveChangesAsync();
            return NoContent();
        }
            
        private bool FabicanteExists(int id)
        {
            return _context.Fabricante.Any(e => e.Codigo == id);
        }
    }
}






