using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWashBackend.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Esto asegura que solo usuarios autenticados puedan acceder

    public class ServicioController : ControllerBase
    {
        private readonly CarwashContext _context;

        public ServicioController(CarwashContext context)
        {
            _context = context;
        }

        // GET: api/Servicio
        [HttpGet]
        public async Task<IActionResult> GetAllServicios()
        {
            var servicios = await _context.Servicios.ToListAsync();

            if (servicios == null || !servicios.Any())
                return NotFound("No se encontraron servicios.");

            var serviciosDto = servicios.Select(s => new ServicioDTO
            {
                id = s.id,
                nombre = s.nombre,
                descripcion = s.descripcion,
                precio = s.precio,
                activo = s.activo,
                created_at = s.created_at,
                updated_at = s.updated_at
            }).ToList();

            return Ok(serviciosDto);
        }

        // GET: api/Servicio/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveServicios()
        {
            var serviciosActivos = await _context.Servicios
                .Where(s => s.activo == true)
                .ToListAsync();

            if (!serviciosActivos.Any())
                return NotFound("No se encontraron servicios activos.");

            var serviciosDto = serviciosActivos.Select(s => new ServicioDTO
            {
                id = s.id,
                nombre = s.nombre,
                descripcion = s.descripcion,
                precio = s.precio,
                activo = s.activo,
                created_at = s.created_at,
                updated_at = s.updated_at
            }).ToList();

            return Ok(serviciosDto);
        }

        // GET: api/Servicio/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServicioById(string id)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
                return NotFound($"No se encontró un servicio con el ID {id}.");

            var servicioDto = new ServicioDTO
            {
                id = servicio.id,
                nombre = servicio.nombre,
                descripcion = servicio.descripcion,
                precio = servicio.precio,
                activo = servicio.activo,
                created_at = servicio.created_at,
                updated_at = servicio.updated_at
            };

            return Ok(servicioDto);
        }

        // POST: api/Servicio
        [HttpPost]
        public async Task<IActionResult> CreateServicio(Servicio servicio)
        {
            if (servicio == null)
                return BadRequest("Los datos enviados son inválidos.");

            servicio.id = Guid.NewGuid().ToString();
            servicio.created_at = DateTime.UtcNow;
            servicio.updated_at = DateTime.UtcNow;

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            var servicioDto = new ServicioDTO
            {
                id = servicio.id,
                nombre = servicio.nombre,
                descripcion = servicio.descripcion,
                precio = servicio.precio,
                activo = servicio.activo,
                created_at = servicio.created_at,
                updated_at = servicio.updated_at
            };

            return CreatedAtAction(nameof(GetServicioById), new { id = servicio.id }, servicioDto);
        }

        // PUT: api/Servicio/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServicio(string id, Servicio servicio)
        {
            if (id != servicio.id)
                return BadRequest("El ID no coincide con el del servicio enviado.");

            var existingServicio = await _context.Servicios.FindAsync(id);

            if (existingServicio == null)
                return NotFound($"No se encontró un servicio con el ID {id}.");

            existingServicio.nombre = servicio.nombre;
            existingServicio.descripcion = servicio.descripcion;
            existingServicio.precio = servicio.precio;
            existingServicio.activo = servicio.activo;
            existingServicio.updated_at = DateTime.UtcNow;

            _context.Servicios.Update(existingServicio);
            await _context.SaveChangesAsync();

            var servicioDto = new ServicioDTO
            {
                id = existingServicio.id,
                nombre = existingServicio.nombre,
                descripcion = existingServicio.descripcion,
                precio = existingServicio.precio,
                activo = existingServicio.activo,
                created_at = existingServicio.created_at,
                updated_at = existingServicio.updated_at
            };

            return Ok(servicioDto);
        }
    }
}
