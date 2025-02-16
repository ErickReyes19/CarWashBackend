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
    [Authorize]
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
            if (dto == null || string.IsNullOrEmpty(dto.ClienteId) ||
                string.IsNullOrEmpty(dto.UsuarioId) || string.IsNullOrEmpty(dto.EstadoServicioId) ||
                dto.Vehiculos == null || !dto.Vehiculos.Any())
            {
                return BadRequest("Faltan datos requeridos.");
            }

            // 1. Crear el registro de servicio principal
            var registroServicio = new registro_servicio
            {
                id = Guid.NewGuid().ToString(),
                cliente_id = dto.ClienteId,
                estado_servicio_id = dto.EstadoServicioId,
                usuario_id = dto.UsuarioId,
                fecha = DateTime.Now
            };

            _context.registro_servicios.Add(registroServicio);

            // 2. Agregar empleados a la relación muchos a muchos
            if (dto.Empleados != null && dto.Empleados.Any())
            {
                foreach (var empleadoDto in dto.Empleados)
                {
                    var empleado = await _context.Empleados.FindAsync(empleadoDto);
                    if (empleado != null)
                    {
                        // Agregar el empleado a la colección de empleados en registro_servicio
                        registroServicio.empleados.Add(empleado);
                    }
                }
            }

            // 3. Crear los registros de vehículos y detalles de servicio, y calcular el total
            decimal totalServicio = 0;
            var registrosVehiculo = new List<registro_servicio_vehiculo>();
            var registrosDetalle = new List<registro_servicio_detalle>();

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

                registrosVehiculo.Add(registroServicioVehiculo);

                foreach (var servicio in vehiculoDto.Servicios)
                {
                    if (string.IsNullOrEmpty(servicio.ServicioId))
                        continue; // o retorna error

                    var registroServicioDetalle = new registro_servicio_detalle
                    {
                        id = Guid.NewGuid().ToString(),
                        registro_servicio_vehiculo_id = registroServicioVehiculo.id,
                        servicio_id = servicio.ServicioId,
                        precio = servicio.Precio
                    };

                    registrosDetalle.Add(registroServicioDetalle);

                    // Acumulamos el precio total
                    totalServicio += servicio.Precio;
                }
            }

            // Establecer el total en el registro de servicio
            registroServicio.total = totalServicio;

            // Añadir todo al contexto
            _context.registro_servicio_vehiculos.AddRange(registrosVehiculo);
            _context.registro_servicio_detalles.AddRange(registrosDetalle);

            // Guardar todo de una vez
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Registro de servicio para múltiples vehículos creado correctamente",
                registroServicioId = registroServicio.id,
                totalServicio = totalServicio // Devolver el total
            });
        }


        [HttpPut("multiple")]
        public async Task<IActionResult> UpdateRegistroServicioMultiple([FromBody] RegistroServicioMultipleUpdateDto dto)
        {
            // Validamos que el DTO contenga los datos requeridos, incluyendo el id del registro a actualizar.
            if (dto == null ||
                string.IsNullOrEmpty(dto.RegistroServicioId) ||
                string.IsNullOrEmpty(dto.ClienteId) ||
                string.IsNullOrEmpty(dto.UsuarioId) ||
                string.IsNullOrEmpty(dto.EstadoServicioId) ||
                dto.Vehiculos == null || !dto.Vehiculos.Any())
            {
                return BadRequest("Faltan datos requeridos.");
            }

            // Buscamos el registro de servicio a actualizar, incluyendo sus relaciones
            var registroServicio = await _context.registro_servicios
                .Include(rs => rs.empleados)
                .Include(rs => rs.registro_servicio_vehiculos)
                    .ThenInclude(rsv => rsv.registro_servicio_detalles)
                .FirstOrDefaultAsync(rs => rs.id == dto.RegistroServicioId);

            if (registroServicio == null)
            {
                return NotFound("Registro de servicio no encontrado.");
            }

            // Actualizamos los campos principales
            registroServicio.cliente_id = dto.ClienteId;
            registroServicio.estado_servicio_id = dto.EstadoServicioId;
            registroServicio.usuario_id = dto.UsuarioId;

            // Actualizamos la relación muchos a muchos de empleados:
            // Limpiamos la relación actual y agregamos los nuevos empleados
            registroServicio.empleados.Clear();
            if (dto.Empleados != null && dto.Empleados.Any())
            {
                foreach (var empleadoId in dto.Empleados)
                {
                    var empleado = await _context.Empleados.FindAsync(empleadoId);
                    if (empleado != null)
                    {
                        registroServicio.empleados.Add(empleado);
                    }
                }
            }

            // Actualizamos los vehículos y sus detalles:
            // Primero eliminamos los registros de vehículos y sus detalles actuales
            var vehiculosExistentes = registroServicio.registro_servicio_vehiculos.ToList();
            foreach (var vehiculo in vehiculosExistentes)
            {
                // Eliminamos los detalles asociados al vehículo
                var detalles = vehiculo.registro_servicio_detalles.ToList();
                foreach (var detalle in detalles)
                {
                    _context.registro_servicio_detalles.Remove(detalle);
                }
                // Eliminamos el registro de vehículo
                _context.registro_servicio_vehiculos.Remove(vehiculo);
            }

            // Ahora, agregamos los nuevos registros a partir del DTO
            foreach (var vehiculoDto in dto.Vehiculos)
            {
                if (string.IsNullOrEmpty(vehiculoDto.VehiculoId) ||
                    vehiculoDto.Servicios == null || !vehiculoDto.Servicios.Any())
                {
                    continue;
                }

                var nuevoRegistroVehiculo = new registro_servicio_vehiculo
                {
                    id = Guid.NewGuid().ToString(),
                    registro_servicio_id = registroServicio.id,
                    vehiculo_id = vehiculoDto.VehiculoId
                };

                _context.registro_servicio_vehiculos.Add(nuevoRegistroVehiculo);

                foreach (var servicio in vehiculoDto.Servicios)
                {
                    if (string.IsNullOrEmpty(servicio.ServicioId))
                        continue;

                    var nuevoDetalle = new registro_servicio_detalle
                    {
                        id = Guid.NewGuid().ToString(),
                        registro_servicio_vehiculo_id = nuevoRegistroVehiculo.id,
                        servicio_id = servicio.ServicioId,
                        precio = servicio.Precio
                    };

                    _context.registro_servicio_detalles.Add(nuevoDetalle);
                }
            }

            // Guardamos los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Registro de servicio actualizado correctamente",
                registroServicioId = registroServicio.id
            });
        }



        [HttpPut("actualizar-estado")]
        public async Task<IActionResult> UpdateEstadoRegistroServicio([FromBody] EstadoServicioUpdateDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.RegistroServicioId) || string.IsNullOrEmpty(dto.EstadoServicioId))
            {
                return BadRequest("Faltan datos requeridos.");
            }

            var registroServicio = await _context.registro_servicios
                .FirstOrDefaultAsync(rs => rs.id == dto.RegistroServicioId);

            if (registroServicio == null)
            {
                return NotFound("Registro de servicio no encontrado.");
            }

            registroServicio.estado_servicio_id = dto.EstadoServicioId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Estado del registro de servicio actualizado correctamente",
                registroServicioId = registroServicio.id
            });
        }






        [HttpGet("summary")]
        public async Task<IActionResult> GetAllResumen(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var registrosQuery = _context.registro_servicios
                .Include(rs => rs.cliente)
                .Include(rs => rs.estado_servicio)
                .Select(rs => new RegistroServicioSummaryDto
                {
                    Id = rs.id,
                    ClienteNombre = rs.cliente.nombre,
                    ClienteCorreo = rs.cliente.correo,
                    EstadoServicioNombre = rs.estado_servicio.nombre,
                    Fecha = rs.fecha
                });

            // Filtro por fecha desde (comparando solo la fecha)RegistroServicioSummaryDto
            if (fechaDesde.HasValue)
            {
                var fechaInicio = fechaDesde.Value.Date; // Se usa Date para quitar la hora
                registrosQuery = registrosQuery.Where(rs => rs.Fecha.Date >= fechaInicio);
            }

            // Filtro por fecha hasta (comparando solo la fecha)
            if (fechaHasta.HasValue)
            {
                var fechaFin = fechaHasta.Value.Date; // Se usa Date para quitar la hora
                registrosQuery = registrosQuery.Where(rs => rs.Fecha.Date <= fechaFin);
            }

            var registros = await registrosQuery.ToListAsync();

            return Ok(registros);
        }
         

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> GetRegistroServicioForEdit(string id)
        {
            // Buscar el registro de servicio con sus relaciones
            var registroServicio = await _context.registro_servicios
                .Include(rs => rs.empleados) // Relación con empleados
                .Include(rs => rs.registro_servicio_vehiculos) // Relación con vehículos
                    .ThenInclude(rsv => rsv.registro_servicio_detalles) // Relación con detalles de servicio
                .FirstOrDefaultAsync(rs => rs.id == id);

            // Validar si no se encontró el registro
            if (registroServicio == null)
            {
                return NotFound("Registro de servicio no encontrado.");
            }

            // Mapear la entidad a un DTO de actualización para el fronten
            var dto = new RegistroServicioMultipleUpdateDto
            {
                RegistroServicioId = registroServicio.id,
                ClienteId = registroServicio.cliente_id,
                EstadoServicioId = registroServicio.estado_servicio_id,
                UsuarioId = registroServicio.usuario_id,
                Empleados = registroServicio.empleados.Select(e => e.id).ToList(),
                Vehiculos = registroServicio.registro_servicio_vehiculos.Select(v => new RegistroServicioVehiculoDto
                {
                    VehiculoId = v.vehiculo_id,
                    Servicios = v.registro_servicio_detalles.Select(d => new ServicioDto
                    {
                        ServicioId = d.servicio_id,
                        Precio = d.precio
                    }).ToList()
                }).ToList()
            };

            return Ok(dto);
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
