using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly CarwashContext _context;

        public RoleController(CarwashContext context)
        {
            _context = context;
        }

        // GET: api/Role
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _context.Roles
                    .Include(r => r.permisos)
                    .ToListAsync();

                var rolesDTO = roles.Select(r => new RoleDTO
                {
                    id = r.id,
                    nombre = r.nombre,
                    descripcion = r.descripcion,
                    activo = r.activo,
                    created_at = r.created_at,
                    updated_at = r.updated_at,
                    permisosRol = r.permisos.Select(p => new PermisoDTORol
                    {
                        id = p.id,
                        nombre = p.nombre
                    }).ToList()
                }).ToList();

                return Ok(rolesDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los roles: {ex.Message}");
            }
        }

        // GET: api/Role/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveRoles()
        {
            try
            {
                var activeRoles = await _context.Roles
                    .Where(r => r.activo == true)
                    .Include(r => r.permisos)
                    .ToListAsync();

                var rolesDTO = activeRoles.Select(r => new RoleDTO
                {
                    id = r.id,
                    nombre = r.nombre,
                    descripcion = r.descripcion,
                    activo = r.activo,
                    created_at = r.created_at,
                    updated_at = r.updated_at,
                    permisosRol = r.permisos.Select(p => new PermisoDTORol
                    {
                        id = p.id,
                        nombre = p.nombre
                    }).ToList()
                }).ToList();

                return Ok(rolesDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los roles activos: {ex.Message}");
            }
        }

        // GET: api/Role/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.permisos)
                    .FirstOrDefaultAsync(r => r.id == id);

                if (role == null)
                    return NotFound($"Rol con ID {id} no encontrado.");

                var roleDTO = new RoleDTO
                {
                    id = role.id,
                    nombre = role.nombre,
                    descripcion = role.descripcion,
                    activo = role.activo,
                    created_at = role.created_at,
                    updated_at = role.updated_at,
                    permisosRol = role.permisos.Select(p => new PermisoDTORol
                    {
                        id = p.id,
                        nombre = p.nombre
                    }).ToList()
                };

                return Ok(roleDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el rol: {ex.Message}");
            }
        }

        // POST: api/Role
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDTO roleCreateDTO)
        {
            if (roleCreateDTO == null)
                return BadRequest("Datos inválidos.");

            try
            {
                var newRole = new Role
                {
                    id = Guid.NewGuid().ToString(),
                    nombre = roleCreateDTO.nombre,
                    descripcion = roleCreateDTO.descripcion,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };

                if (roleCreateDTO.permisosRol != null && roleCreateDTO.permisosRol.Any())
                {
                    var permisos = await _context.Permisos
                        .Where(p => roleCreateDTO.permisosRol.Contains(p.id))
                        .ToListAsync();

                    newRole.permisos = permisos;
                }

                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();

                var roleDTO = new RoleDTO
                {
                    id = newRole.id,
                    nombre = newRole.nombre,
                    descripcion = newRole.descripcion,
                    activo = newRole.activo,
                    created_at = newRole.created_at,
                    updated_at = newRole.updated_at,
                    permisosRol = newRole.permisos.Select(p => new PermisoDTORol
                    {
                        id = p.id,
                        nombre = p.nombre
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetRoleById), new { id = newRole.id }, roleDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el rol: {ex.Message}");
            }
        }

        // PUT: api/Role/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleUpdateDTO roleDTO)
        {
            if (id != roleDTO.id)
                return BadRequest("El ID proporcionado no coincide.");

            try
            {
                var existingRole = await _context.Roles
                    .Include(r => r.permisos) // Incluimos los permisos del rol actual
                    .FirstOrDefaultAsync(r => r.id == id);

                if (existingRole == null)
                    return NotFound($"Rol con ID {id} no encontrado.");

                // Actualizar la información básica del rol
                existingRole.nombre = roleDTO.nombre;
                existingRole.descripcion = roleDTO.descripcion;
                existingRole.activo = roleDTO.activo;
                existingRole.updated_at = DateTime.UtcNow;

                // Si los permisos fueron enviados
                if (roleDTO.permisosRol != null && roleDTO.permisosRol.Any())
                {
                    // Encontrar los permisos usando los IDs proporcionados
                    var permisos = await _context.Permisos
                        .Where(p => roleDTO.permisosRol.Contains(p.id)) // Filtrar por los IDs proporcionados
                        .ToListAsync();

                    // Asignamos los permisos encontrados al rol
                    existingRole.permisos = permisos;
                }

                // Actualizar el rol en la base de datos
                _context.Roles.Update(existingRole);
                await _context.SaveChangesAsync();

                // Devolver el DTO actualizado con los objetos de permisos (no solo los IDs)
                var updatedRoleDTO = new RoleDTO
                {
                    id = existingRole.id,
                    nombre = existingRole.nombre,
                    descripcion = existingRole.descripcion,
                    activo = existingRole.activo,
                    created_at = existingRole.created_at,
                    updated_at = existingRole.updated_at,
                    permisosRol = existingRole.permisos.Select(p => new PermisoDTORol
                    {
                        id = p.id,
                        nombre = p.nombre
                    }).ToList() // Convertimos cada permiso a PermisoDTORol
                };

                return Ok(updatedRoleDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el rol: {ex.Message}");
            }
        }


    }
}
