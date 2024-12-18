using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Esto asegura que solo usuarios autenticados puedan acceder
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
            try
            {
                var empleados = await _context.Empleados
                    .Include(e => e.Usuarios) // Incluimos el usuario relacionado
                    .ToListAsync();

                if (empleados == null || !empleados.Any())
                    return NotFound("No se encontraron empleados.");

                // Mapeamos a DTOs
                var empleadosDTO = empleados.Select(e => new EmpleadoDTO
                {
                    Id = e.id,
                    Nombre = e.nombre,
                    Apellido = e.apellido,
                    Edad = e.edad,
                    Genero = e.genero,
                    Activo = e.activo,
                    CreatedAt = e.created_at,
                    UpdatedAt = e.updated_at,
                    // Accedemos directamente al primer (y único) usuario
                    UsuarioNombre = e.Usuarios.FirstOrDefault()?.usuario1 // El primer usuario asociado
                }).ToList();

                return Ok(empleadosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los empleados: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente(Empleado empleado)
        {
            try
            {
                if (empleado == null)
                    return BadRequest("Datos inválidos.");

                empleado.id = Guid.NewGuid().ToString();
                empleado.created_at = DateTime.UtcNow;
                empleado.updated_at = DateTime.UtcNow;

                _context.Empleados.Add(empleado);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmpleadoById), new { id = empleado.id }, empleado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el cliente: {ex.Message}");
            }
        }

        // GET: api/Empleado/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveEmpleados()
        {
            try
            {
                var empleadosActivos = await _context.Empleados
                    .Where(e => e.activo == true)
                    .Include(e => e.Usuarios) // Incluye la relación con Usuarios
                    .ToListAsync();

                if (empleadosActivos == null || !empleadosActivos.Any())
                    return NotFound("No se encontraron empleados activos.");

                var empleadosDTO = empleadosActivos.Select(e => new EmpleadoDTO
                {
                    Id = e.id,
                    Nombre = e.nombre,
                    Apellido = e.apellido,
                    Edad = e.edad,
                    Genero = e.genero,
                    Activo = e.activo,
                    CreatedAt = e.created_at,
                    UpdatedAt = e.updated_at,
                    UsuarioNombre = e.Usuarios.FirstOrDefault()?.usuario1 // Maneja posibles valores null
                }).ToList();

                return Ok(empleadosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los empleados activos: {ex.Message}");
            }
        }

        // GET: api/Empleado/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmpleadoById(string id)
        {
            try
            {
                var empleado = await _context.Empleados
                    .Include(e => e.Usuarios) // Incluye la relación con Usuarios
                    .FirstOrDefaultAsync(e => e.id == id);

                if (empleado == null)
                {
                    return NotFound($"Empleado con ID {id} no encontrado.");
                }

                var empleadoDTO = new EmpleadoDTO
                {
                    Id = empleado.id,
                    Nombre = empleado.nombre,
                    Apellido = empleado.apellido,
                    Edad = empleado.edad,
                    Genero = empleado.genero,
                    Activo = empleado.activo,
                    CreatedAt = empleado.created_at,
                    UpdatedAt = empleado.updated_at,
                    UsuarioNombre = empleado.Usuarios.FirstOrDefault()?.usuario1 // Maneja posibles valores null
                };

                return Ok(empleadoDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el empleado: {ex.Message}");
            }
        }

        // PUT: api/Empleado/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmpleado(string id, [FromBody] Empleado empleado)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el empleado: {ex.Message}");
            }
        }
    }
}
