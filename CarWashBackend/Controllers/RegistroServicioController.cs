using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TuProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Esto asegura que solo usuarios autenticados puedan acceder
    public class RegistroServicioController : ControllerBase
    {
        private readonly CarwashContext _context;

        public RegistroServicioController(CarwashContext context)
        {
            _context = context;
        }
        [HttpPost("multiple")]
        public async Task<IActionResult> CreateRegistroServicioMultiple([FromBody] RegistroServicioMultipleDto dto)
        {
            // Validación básica
            if (dto == null ||
                string.IsNullOrEmpty(dto.ClienteId) ||
                string.IsNullOrEmpty(dto.UsuarioId) ||
                string.IsNullOrEmpty(dto.EstadoServicioId) ||
                dto.Vehiculos == null || !dto.Vehiculos.Any())
            {
                return BadRequest("Faltan datos requeridos.");
            }

            // 1. Crear el registro de servicio principal (tabla: registro_servicio)
            var registroServicio = new registro_servicio
            {
                id = Guid.NewGuid().ToString(),
                cliente_id = dto.ClienteId,
                estado_servicio_id = dto.EstadoServicioId,
                usuario_id = dto.UsuarioId,
                fecha = DateTime.UtcNow
            };

            _context.registro_servicios.Add(registroServicio);
            await _context.SaveChangesAsync();

            // 2. Agregar empleados a la tabla intermedia "empleado_registro_servicio"
            if (dto.Empleados != null && dto.Empleados.Any())
            {
                foreach (var empleadoDto in dto.Empleados)
                {
                    // Busca el empleado
                    var empleado = await _context.Empleados.FindAsync(empleadoDto.EmpleadoId);
                    if (empleado != null)
                    {
                        // Crear la relación en la tabla intermedia directamente
                        _context.Add(new
                        {
                            empleado_id = empleadoDto.EmpleadoId,
                            registro_servicio_id = registroServicio.id
                        });
                    }
                }

                // Guardar los cambios para la relación empleados-registro_servicio
                await _context.SaveChangesAsync();
            }

            // 3. Para cada vehículo, se crea el registro en registro_servicio_vehiculo y sus detalles
            foreach (var vehiculoDto in dto.Vehiculos)
            {
                if (string.IsNullOrEmpty(vehiculoDto.VehiculoId) || vehiculoDto.Servicios == null || !vehiculoDto.Servicios.Any())
                {
                    continue;
                }

                var registroServicioVehiculo = new registro_servicio_vehiculo
                {
                    id = Guid.NewGuid().ToString(),
                    registro_servicio_id = registroServicio.id,
                    vehiculo_id = vehiculoDto.VehiculoId
                };

                _context.registro_servicio_vehiculos.Add(registroServicioVehiculo);
                await _context.SaveChangesAsync();

                foreach (var servicio in vehiculoDto.Servicios)
                {
                    if (string.IsNullOrEmpty(servicio.ServicioId))
                        continue;

                    var registroServicioDetalle = new registro_servicio_detalle
                    {
                        id = Guid.NewGuid().ToString(),
                        registro_servicio_vehiculo_id = registroServicioVehiculo.id,
                        servicio_id = servicio.ServicioId,
                        precio = servicio.Precio
                    };

                    _context.registro_servicio_detalles.Add(registroServicioDetalle);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new
            {
                mensaje = "Registro de servicio para múltiples vehículos creado correctamente",
                registroServicioId = registroServicio.id
            });
        }


        // GET: api/RegistroServicio/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetAllResumen()
        {
            var registros = await _context.registro_servicios
                .Include(rs => rs.cliente)
                .Include(rs => rs.estado_servicio)
                .Select(rs => new RegistroServicioSummaryDto
                {
                    Id = rs.id,
                    ClienteNombre = rs.cliente.nombre,
                    ClienteCorreo = rs.cliente.correo,
                    EstadoServicioNombre = rs.estado_servicio.nombre,
                    Fecha = rs.fecha
                })
                .ToListAsync();

            return Ok(registros);
        }

        // GET: api/RegistroServicio/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegistroServicioById(string id)
        {
            var registro = await _context.registro_servicios
                .Include(rs => rs.cliente)
                .Include(rs => rs.estado_servicio)
                .Include(rs => rs.registro_servicio_vehiculos)
                    .ThenInclude(rsv => rsv.vehiculo)
                .Include(rs => rs.registro_servicio_vehiculos)
                    .ThenInclude(rsv => rsv.registro_servicio_detalles)
                        .ThenInclude(rsd => rsd.servicio)
                .Include(rs => rs.empleados) // Aquí accedes a la relación empleados
                .FirstOrDefaultAsync(rs => rs.id == id);

            if (registro == null)
            {
                return NotFound("Registro de servicio no encontrado.");
            }

            // Mapeo a DTO
            var dto = new RegistroServicioDetailDto
            {
                Id = registro.id,
                Fecha = registro.fecha,
                Cliente = new ClienteDto
                {
                    Id = registro.cliente.id,
                    Nombre = registro.cliente.nombre,
                    Correo = registro.cliente.correo,
                    Telefono = registro.cliente.telefono
                },
                Usuario = new UsuarioDto
                {
                    Id = registro.usuario_id
                },
                EstadoServicio = new EstadosServicioDto
                {
                    Id = registro.estado_servicio.id,
                    Nombre = registro.estado_servicio.nombre,
                    Descripcion = registro.estado_servicio.descripcion
                },
                Vehiculos = registro.registro_servicio_vehiculos.Select(rsv => new RegistroServicioVehiculoDetailDto
                {
                    Id = rsv.id,
                    Vehiculo = new VehiculoDto
                    {
                        Id = rsv.vehiculo.id,
                        Placa = rsv.vehiculo.placa,
                        Marca = rsv.vehiculo.marca,
                        Modelo = rsv.vehiculo.modelo,
                        Color = rsv.vehiculo.color
                    },
                    Servicios = rsv.registro_servicio_detalles.Select(rsd => new ServicioDetailDto
                    {
                        Id = rsd.id,
                        ServicioNombre = rsd.servicio.nombre,
                        Precio = rsd.precio
                    }).ToList()
                }).ToList(),
                Empleados = registro.empleados.Select(rse => new EmpleadoDto
                {
                    Id = rse.id,
                    Nombre = rse.nombre,
                    Apellido = rse.apellido,
                    Correo = rse.correo
                }).ToList() // Aquí mapeas los empleados
            };

            return Ok(dto);
        }


    }
}
