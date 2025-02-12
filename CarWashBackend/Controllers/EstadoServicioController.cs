using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Esto asegura que solo usuarios autenticados puedan acceder
    public class EstadosServicioController : ControllerBase
    {
        private readonly CarwashContext _context;

        public EstadosServicioController(CarwashContext context)
        {
            _context = context;
        }

        // GET: api/EstadosServicio
        [HttpGet]
        public async Task<IActionResult> GetAllEstadosServicio()
        {
            var estadosServicio = await _context.EstadosServicios.ToListAsync();

            if (estadosServicio == null || !estadosServicio.Any())
                return NotFound("No se encontraron estados de servicio.");

            var estadosServicioDto = estadosServicio.Select(es => new EstadosServicioDTO
            {
                id = es.id,
                nombre = es.nombre,
                descripcion = es.descripcion,
                activo = es.activo,
                created_at = es.created_at,
                updated_at = es.updated_at
            }).ToList();

            return Ok(estadosServicioDto);
        }

        // GET: api/EstadosServicio/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveEstadosServicio()
        {
            var estadosServicioActivos = await _context.EstadosServicios
                .Where(es => es.activo == true)
                .ToListAsync();

            if (!estadosServicioActivos.Any())
                return NotFound("No se encontraron estados de servicio activos.");

            var estadosServicioDto = estadosServicioActivos.Select(es => new EstadosServicioDTO
            {
                id = es.id,
                nombre = es.nombre,
                descripcion = es.descripcion,
                activo = es.activo,
                created_at = es.created_at,
                updated_at = es.updated_at
            }).ToList();

            return Ok(estadosServicioDto);
        }

        // GET: api/EstadosServicio/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEstadosServicioById(string id)
        {
            var estadoServicio = await _context.EstadosServicios.FindAsync(id);

            if (estadoServicio == null)
                return NotFound($"No se encontró un estado de servicio con el ID {id}.");

            var estadoServicioDto = new EstadosServicioDTO
            {
                id = estadoServicio.id,
                nombre = estadoServicio.nombre,
                descripcion = estadoServicio.descripcion,
                activo = estadoServicio.activo,
                created_at = estadoServicio.created_at,
                updated_at = estadoServicio.updated_at
            };

            return Ok(estadoServicioDto);
        }

        // POST: api/EstadosServicio
        [HttpPost]
        public async Task<IActionResult> CreateEstadosServicio(EstadosServicio estadoServicio)
        {
            if (estadoServicio == null)
                return BadRequest("Los datos enviados son inválidos.");

            estadoServicio.id = Guid.NewGuid().ToString();
            estadoServicio.created_at = DateTime.UtcNow;
            estadoServicio.updated_at = DateTime.UtcNow;

            _context.EstadosServicios.Add(estadoServicio);
            await _context.SaveChangesAsync();

            var estadoServicioDto = new EstadosServicioDTO
            {
                id = estadoServicio.id,
                nombre = estadoServicio.nombre,
                descripcion = estadoServicio.descripcion,
                activo = estadoServicio.activo,
                created_at = estadoServicio.created_at,
                updated_at = estadoServicio.updated_at
            };

            return CreatedAtAction(nameof(GetEstadosServicioById), new { id = estadoServicio.id }, estadoServicioDto);
        }

        // PUT: api/EstadosServicio/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstadosServicio(string id, EstadosServicio estadoServicio)
        {
            if (id != estadoServicio.id)
                return BadRequest("El ID no coincide con el del estado de servicio enviado.");

            var existingEstadoServicio = await _context.EstadosServicios.FindAsync(id);

            if (existingEstadoServicio == null)
                return NotFound($"No se encontró un estado de servicio con el ID {id}.");

            existingEstadoServicio.nombre = estadoServicio.nombre;
            existingEstadoServicio.descripcion = estadoServicio.descripcion;
            existingEstadoServicio.activo = estadoServicio.activo;
            existingEstadoServicio.updated_at = DateTime.UtcNow;

            _context.EstadosServicios.Update(existingEstadoServicio);
            await _context.SaveChangesAsync();

            var estadoServicioDto = new EstadosServicioDTO
            {
                id = existingEstadoServicio.id,
                nombre = existingEstadoServicio.nombre,
                descripcion = existingEstadoServicio.descripcion,
                activo = existingEstadoServicio.activo,
                created_at = existingEstadoServicio.created_at,
                updated_at = existingEstadoServicio.updated_at
            };

            return Ok(estadoServicioDto);
        }
    }
}
