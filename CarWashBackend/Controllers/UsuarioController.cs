using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Asegura que solo usuarios autenticados puedan acceder
public class UsuarioController : ControllerBase
{
    private readonly CarwashContext _context;

    public UsuarioController(CarwashContext context)
    {
        _context = context;
    }

    // Obtener todos los usuarios
    [HttpGet]
    public async Task<IActionResult> GetAllUsuarios()
    {
        var usuarios = await _context.Usuarios
                                      .Include(u => u.empleado)  // Incluye la relación con empleado
                                      .Include(u => u.role)      // Incluye la relación con role
                                      .ToListAsync();

        if (usuarios == null || usuarios.Count == 0)
            return NotFound();

        // Mapear los usuarios a DTO para evitar ciclo de referencia
        var usuariosDto = usuarios.Select(u => new UsuarioDTO
        {
            id = u.id,
            usuario = u.usuario1,
            activo = u.activo,
            empleadoNombre = u.empleado?.nombre,  // Nombre del empleado asociado
            roleNombre = u.role?.nombre,          // Nombre del rol asociado
            created_at = u.created_at,
            updated_at = u.updated_at
        }).ToList();

        return Ok(usuariosDto);
    }


    [HttpGet("active")]
    public async Task<IActionResult> GetUsuariosActivos()
    {
        try
        {
            var usuariosActivos = await _context.Usuarios
                .Where(u => u.activo == true) // Condición para usuarios activos
                .Select(u => new
                {
                    u.id,
                    u.usuario1,
                    u.activo,
                    Empleado = u.empleado == null ? "Sin asignar" : u.empleado.nombre,
                    Rol = u.role == null ? "Sin asignar" : u.role.nombre,
                    u.created_at,
                    u.updated_at
                })
                .ToListAsync();

            return Ok(usuariosActivos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener usuarios activos.", details = ex.Message });
        }
    }

    // Crear un nuevo usuario
    [HttpPost]
    public async Task<IActionResult> CreateUsuario(Usuario usuario)
    {
        if (usuario == null)
            return BadRequest("Datos inválidos.");

        // Verificar si el nombre de usuario ya está registrado
        var existingUsuario = await _context.Usuarios
                                             .FirstOrDefaultAsync(u => u.usuario1 == usuario.usuario1);

        if (existingUsuario != null)
        {
            return Conflict(new { message = "El nombre de usuario ya está registrado." });
        }

        // Verificar si el empleado y el rol existen
        var empleadoExistente = await _context.Empleados
                                              .FirstOrDefaultAsync(e => e.id == usuario.empleado_id);
        var roleExistente = await _context.Roles
                                          .FirstOrDefaultAsync(r => r.id == usuario.role_id);

        if (empleadoExistente == null || roleExistente == null)
        {
            return NotFound(new { message = "Empleado o rol no encontrados." });
        }

        // Crear el usuario
        usuario.id = Guid.NewGuid().ToString();
        usuario.created_at = DateTime.UtcNow;
        usuario.updated_at = DateTime.UtcNow;

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Retornar el usuario creado
        var usuarioCreatedDto = new UsuarioDTO
        {
            id = usuario.id,
            usuario = usuario.usuario1,
            activo = usuario.activo,
            empleadoNombre = empleadoExistente?.nombre,
            roleNombre = roleExistente?.nombre,
            created_at = usuario.created_at,
            updated_at = usuario.updated_at
        };

        return CreatedAtAction(nameof(GetUsuarioById), new { id = usuario.id }, usuarioCreatedDto);
    }

    // Obtener un usuario por ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsuarioById(string id)
    {
        var usuario = await _context.Usuarios
                                     .Include(u => u.empleado)  // Incluye la relación con empleado
                                     .Include(u => u.role)      // Incluye la relación con role
                                     .FirstOrDefaultAsync(u => u.id == id);

        if (usuario == null)
            return NotFound();

        // Crear el DTO con los datos del usuario
        var usuarioDto = new UsuarioDTO
        {
            id = usuario.id,
            usuario = usuario.usuario1,
            empleadoNombre = usuario.empleado?.nombre,
            roleNombre = usuario.role?.nombre,
            activo = usuario.activo,
            created_at = usuario.created_at,
            updated_at = usuario.updated_at
        };

        return Ok(usuarioDto);
    }

    // Actualizar un usuario
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUsuario(string id, Usuario usuario)
    {
        if (id != usuario.id)
            return BadRequest("El ID no coincide.");

        var existingUsuario = await _context.Usuarios.FindAsync(id);
        if (existingUsuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        // Verificar si el nombre de usuario ya está registrado
        var vehicleWithSameUsuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.usuario1 == usuario.usuario1 && u.id != id);

        if (vehicleWithSameUsuario != null)
        {
            return Conflict(new { message = "Ya existe un usuario con el mismo nombre." });
        }

        // Verificar si el empleado y el rol existen
        var empleadoExistente = await _context.Empleados
                                              .FirstOrDefaultAsync(e => e.id == usuario.empleado_id);
        var roleExistente = await _context.Roles
                                          .FirstOrDefaultAsync(r => r.id == usuario.role_id);

        if (empleadoExistente == null || roleExistente == null)
        {
            return NotFound(new { message = "Empleado o rol no encontrados." });
        }

        // Actualizar los datos del usuario
        existingUsuario.usuario1 = usuario.usuario1;
        existingUsuario.contrasena = usuario.contrasena;
        existingUsuario.empleado_id = usuario.empleado_id;
        existingUsuario.activo = usuario.activo;
        existingUsuario.role_id = usuario.role_id;
        existingUsuario.updated_at = DateTime.UtcNow;

        // Guardar los cambios
        _context.Usuarios.Update(existingUsuario);
        await _context.SaveChangesAsync();

        // Crear el DTO con los datos actualizados
        var usuarioUpdatedDto = new UsuarioDTO
        {
            id = existingUsuario.id,
            usuario = existingUsuario.usuario1,
            empleadoNombre = empleadoExistente?.nombre,
            roleNombre = roleExistente?.nombre,
            activo = roleExistente?.activo,
            created_at = existingUsuario.created_at,
            updated_at = existingUsuario.updated_at
        };

        return Ok(usuarioUpdatedDto);
    }


}
