﻿using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Esto asegura que solo usuarios autenticados puedan acceder
    public class PermisoController : ControllerBase
    {
        private readonly CarwashContext _context;

        public PermisoController(CarwashContext context)
        {
            _context = context;
        }

        // GET: api/Permiso
        [HttpGet]
        public async Task<IActionResult> GetPermisos()
        {
            var permisos = await _context.Permisos
                .ToListAsync();

            // Mapeamos los permisos a DTOs sin incluir los roles
            var permisosDTO = permisos.Select(p => new PermisoDTO
            {
                id = p.id,
                nombre = p.nombre,
                descripcion = p.descripcion,
                created_at = p.created_at,
                updated_at = p.updated_at
            }).ToList();

            return Ok(permisosDTO);
        }

        // GET: api/Permiso/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActivePermisos()
        {
            var permisosActivos = await _context.Permisos
                .Where(p => p.updated_at != null) // Aquí puedes agregar la lógica para verificar si el permiso está activo
                .ToListAsync();

            var permisosDTO = permisosActivos.Select(p => new PermisoDTO
            {
                id = p.id,
                nombre = p.nombre,
                descripcion = p.descripcion,
                created_at = p.created_at,
                updated_at = p.updated_at
            }).ToList();

            return Ok(permisosDTO);
        }

        // GET: api/Permiso/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPermisoById(string id)
        {
            var permiso = await _context.Permisos
                .FirstOrDefaultAsync(p => p.id == id);

            if (permiso == null)
            {
                return NotFound($"Permiso con ID {id} no encontrado.");
            }

            var permisoDTO = new PermisoDTO
            {
                id = permiso.id,
                nombre = permiso.nombre,
                descripcion = permiso.descripcion,
                created_at = permiso.created_at,
                updated_at = permiso.updated_at
            };

            return Ok(permisoDTO);
        }

        // POST: api/Permiso
        [HttpPost]
        public async Task<IActionResult> CreatePermiso([FromBody] Permiso permiso)
        {
            if (permiso == null)
                return BadRequest("Datos inválidos.");

            permiso.id = Guid.NewGuid().ToString();
            permiso.created_at = DateTime.UtcNow;
            permiso.updated_at = DateTime.UtcNow;

            _context.Permisos.Add(permiso);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPermisoById), new { id = permiso.id }, permiso);
        }

        // PUT: api/Permiso/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermiso(string id, [FromBody] Permiso permiso)
        {
            if (id != permiso.id)
            {
                return BadRequest("El ID proporcionado no coincide.");
            }

            var existingPermiso = await _context.Permisos.FindAsync(id);
            if (existingPermiso == null)
            {
                return NotFound($"Permiso con ID {id} no encontrado.");
            }

            existingPermiso.nombre = permiso.nombre;
            existingPermiso.descripcion = permiso.descripcion;
            existingPermiso.updated_at = DateTime.UtcNow;

            _context.Permisos.Update(existingPermiso);
            await _context.SaveChangesAsync();

            return Ok(existingPermiso);
        }
    }
}