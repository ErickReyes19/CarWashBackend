﻿using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
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
                    .Include(e => e.Usuarios) 
                    .ToListAsync();

                if (empleados == null || !empleados.Any())
                    return NotFound("No se encontraron empleados.");

                
                var empleadosDTO = empleados.Select(e => new EmpleadoDTO
                {
                    Id = e.id,
                    Nombre = e.nombre,
                    Apellido = e.apellido,
                    Edad = e.edad,
                    Genero = e.genero,
                    Activo = e.activo,
                    correo = e.correo,

                    CreatedAt = e.created_at,
                    UpdatedAt = e.updated_at,
                    
                    UsuarioNombre = e.Usuarios.FirstOrDefault()?.usuario1 
                }).ToList();

                return Ok(empleadosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los empleados: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmpleado(Empleado empleado)
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
                return StatusCode(500, $"Error al crear el empleado: {ex.Message}");
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
                    .Include(e => e.Usuarios) 
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
                    correo = e.correo,
                    CreatedAt = e.created_at,
                    UpdatedAt = e.updated_at,
                    UsuarioNombre = e.Usuarios.FirstOrDefault()?.usuario1 
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
                    .Include(e => e.Usuarios) 
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
                    correo = empleado.correo,
                    CreatedAt = empleado.created_at,
                    UpdatedAt = empleado.updated_at,
                    UsuarioNombre = empleado.Usuarios.FirstOrDefault()?.usuario1 
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

        [HttpGet("disponibles")]
        public async Task<IActionResult> GetEmpleadosSinUsuario()
        {
            try
            {
                var empleadosSinUsuario = await _context.Empleados
                    .Where(e => e.activo == true && !_context.Usuarios.Any(u => u.empleado_id == e.id))
                    .ToListAsync();

                if (empleadosSinUsuario == null || !empleadosSinUsuario.Any())
                    return NotFound("No se encontraron empleados sin usuario asignado.");

                
                var empleadosDTO = empleadosSinUsuario.Select(e => new EmpleadoDTO
                {
                    Id = e.id,
                    Nombre = e.nombre,
                    Apellido = e.apellido,
                    Edad = e.edad,
                    Genero = e.genero,
                    Activo = e.activo,
                    correo = e.correo,
                    CreatedAt = e.created_at,
                    UpdatedAt = e.updated_at
                }).ToList();

                return Ok(empleadosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los empleados sin usuario asignado: {ex.Message}");
            }
        }
    }


}
