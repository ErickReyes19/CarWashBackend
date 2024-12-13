using CarWashBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly CarwashContext _context;

        public EmpleadoController(CarwashContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmpleados()
        {
            var empleados = await _context.Empleados
                .Include(e => e.Usuarios) // Incluimos el usuario relacionado
                .ToListAsync();

            // Mapeamos a DTOs
            var empleadosDTO = empleados.Select(e => new EmpleadoDTO
            {
                Id = e.id,
                Nombre = e.nombre,
                Apellido = e.apellido,
                Correo = e.correo,
                Edad = e.edad,
                Genero = e.genero,
                Activo = e.activo,
                CreatedAt = e.created_at,
                UpdatedAt = e.updated_at,
                // Accedemos directamente al primer (y único) usuario
                UsuarioNombre = e.Usuarios.FirstOrDefault()?.usuario // El primer usuario asociado
            }).ToList();

            return Ok(empleadosDTO);
        }


        // GET: api/Empleado/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveEmpleados()
        {
            var empleadosActivos = await _context.Empleados
                                                  .Where(e => e.activo == true)
                                                  .ToListAsync();
            return Ok(empleadosActivos);
        }

        // GET: api/Empleado/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmpleadoById(string id)
        {
            var empleado = await _context.Empleados
                                         .FirstOrDefaultAsync(e => e.id == id);
            if (empleado == null)
            {
                return NotFound($"Empleado con ID {id} no encontrado.");
            }
            return Ok(empleado);
        }

        // PUT: api/Empleado/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmpleado(string id, [FromBody] Empleado empleado)
        {
            if (id != empleado.id)
            {
                return BadRequest("El ID proporcionado no coincide.");
            }

            var existingEmpleado = await _context.Empleados.FindAsync(id);
            if (existingEmpleado == null)
            {
                return NotFound($"Empleado con ID {id} no encontrado.");
            }

            existingEmpleado.nombre = empleado.nombre;
            existingEmpleado.apellido = empleado.apellido;
            existingEmpleado.correo = empleado.correo;
            existingEmpleado.edad = empleado.edad;
            existingEmpleado.genero = empleado.genero;
            existingEmpleado.activo = empleado.activo;
            existingEmpleado.updated_at = DateTime.UtcNow;

            _context.Empleados.Update(existingEmpleado);
            await _context.SaveChangesAsync();

            return Ok(existingEmpleado);
        }
    }
}
