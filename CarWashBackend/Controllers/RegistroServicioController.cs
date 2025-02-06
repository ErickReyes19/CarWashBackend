using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Asegura que solo usuarios autenticados puedan acceder
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
                fecha = DateTime.UtcNow
            };

            _context.registro_servicios.Add(registroServicio);

            // 2. Agregar empleados a la relación muchos a muchos
            if (dto.Empleados != null && dto.Empleados.Any())
            {
                foreach (var empleadoDto in dto.Empleados)
                {
                    var empleado = await _context.Empleados.FindAsync(empleadoDto.EmpleadoId);
                    if (empleado != null)
                    {
                        // Agregar el empleado a la colección de empleados en registro_servicio
                        registroServicio.empleados.Add(empleado);
                    }
                }
            }

            // 3. Crear los registros de vehículos y detalles de servicio
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
                }
            }

            // Añadir todo al contexto
            _context.registro_servicio_vehiculos.AddRange(registrosVehiculo);
            _context.registro_servicio_detalles.AddRange(registrosDetalle);

            // Guardar todo de una vez
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Registro de servicio para múltiples vehículos creado correctamente",
                registroServicioId = registroServicio.id
            });
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

            // Filtro por fecha desde (comparando solo la fecha)
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

            if (registros == null || !registros.Any())
                return NotFound("No se encontraron registros de servicios.");

            var registrosDto = registros.Select(r => new RegistroServicioDTO
            {
                id = r.id,
                cliente_id = r.cliente_id,
                vehiculo_id = r.vehiculo_id,
                servicio_id = r.servicio_id,
                usuario_id = r.usuario_id,
                estado_id = r.estado_id,
                observaciones = r.observaciones,
                fecha = r.fecha,
                created_at = r.created_at,
                updated_at = r.updated_at,
                cliente = new ClienteSummaryDTO
                {
                    Id = r.cliente.id,
                    Nombre = r.cliente.nombre
                },
                vehiculo = new VehiculoSummaryDTO
                {
                    Id = r.vehiculo.id,
                    Marca = r.vehiculo.marca
                },
                servicio = new ServicioSummaryDTO
                {
                    Id = r.servicio.id,
                    Nombre = r.servicio.nombre
                },
                usuario = new UsuarioSummaryDTO
                {
                    Id = r.usuario.id,
                    Usuario = r.usuario.usuario1
                },
                estado = new EstadosServicioSummaryDTO
                {
                    Id = r.estado.id,
                    Nombre = r.estado.nombre
                },
                Pagos = r.Pagos.Select(p => new PagoSummaryDTO
                {
                    id = p.id,
                    monto = p.monto,
                    metodo_pago = p.metodo_pago,
                    fecha = p.fecha
                }).ToList() // Mapea los pagos relacionados
            }).ToList();


            return Ok(registrosDto);
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
                return NotFound($"No se encontró un registro de servicio con el ID {id}.");

            var registroDto = new RegistroServicioDTO
            {
                id = registro.id,
                cliente_id = registro.cliente_id,
                vehiculo_id = registro.vehiculo_id,
                servicio_id = registro.servicio_id,
                usuario_id = registro.usuario_id,
                estado_id = registro.estado_id,
                observaciones = registro.observaciones,
                fecha = registro.fecha,
                created_at = registro.created_at,
                updated_at = registro.updated_at,
                cliente = new ClienteSummaryDTO
                {
                    Id = registro.cliente.id,
                    Nombre = registro.cliente.nombre
                },
                vehiculo = new VehiculoSummaryDTO
                {
                    Id = registro.vehiculo.id,
                    Marca = registro.vehiculo.marca
                },
                servicio = new ServicioSummaryDTO
                {
                    Id = registro.servicio.id,
                    Nombre = registro.servicio.nombre
                },
                usuario = new UsuarioSummaryDTO
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

            return Ok(registroDto);
        }

        // POST: api/RegistroServicio
        [HttpPost]
        public async Task<IActionResult> CreateRegistroServicio(RegistroServicio registro)
        {

            Console.WriteLine($"Cliente ID: {registro.cliente_id} (Length: {registro.cliente_id.Length})");



            if (registro == null)
                return BadRequest("Los datos enviados son inválidos.");

            registro.id = Guid.NewGuid().ToString();
            registro.created_at = DateTime.UtcNow;
            registro.updated_at = DateTime.UtcNow;

            _context.RegistroServicios.Add(registro);
            await _context.SaveChangesAsync();

            var registroDto = new RegistroServicioDTO
            {
                id = registro.id,
                cliente_id = registro.cliente_id,
                vehiculo_id = registro.vehiculo_id,
                servicio_id = registro.servicio_id,
                usuario_id = registro.usuario_id,
                estado_id = registro.estado_id,
                observaciones = registro.observaciones,
                fecha = registro.fecha,
                created_at = registro.created_at,
                updated_at = registro.updated_at
            };

            return CreatedAtAction(nameof(GetRegistroServicioById), new { id = registro.id }, registroDto);
        }

        // PUT: api/RegistroServicio/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegistroServicio(string id, RegistroServicio registro)
        {
            if (id != registro.id)
                return BadRequest("El ID no coincide con el del registro de servicio enviado.");

            var existingRegistro = await _context.RegistroServicios.FindAsync(id);

            if (existingRegistro == null)
                return NotFound($"No se encontró un registro de servicio con el ID {id}.");

            existingRegistro.cliente_id = registro.cliente_id;
            existingRegistro.vehiculo_id = registro.vehiculo_id;
            existingRegistro.servicio_id = registro.servicio_id;
            existingRegistro.usuario_id = registro.usuario_id;
            existingRegistro.estado_id = registro.estado_id;
            existingRegistro.observaciones = registro.observaciones;
            existingRegistro.fecha = registro.fecha;
            existingRegistro.updated_at = DateTime.UtcNow;

            _context.RegistroServicios.Update(existingRegistro);
            await _context.SaveChangesAsync();

            var registroDto = new RegistroServicioDTO
            {
                id = existingRegistro.id,
                cliente_id = existingRegistro.cliente_id,
                vehiculo_id = existingRegistro.vehiculo_id,
                servicio_id = existingRegistro.servicio_id,
                usuario_id = existingRegistro.usuario_id,
                estado_id = existingRegistro.estado_id,
                observaciones = existingRegistro.observaciones,
                fecha = existingRegistro.fecha,
                created_at = existingRegistro.created_at,
                updated_at = existingRegistro.updated_at
            };

            return Ok(registroDto);
        }


    }
}
