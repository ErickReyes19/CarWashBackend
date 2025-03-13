using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace TuProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegistroServicioController : ControllerBase
    {
        private readonly CarwashContext _context;
        private readonly IHubContext<OrderHub> _hubContext;

        public RegistroServicioController(CarwashContext context, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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

            // Crear el registro del servicio
            var registroServicio = new registro_servicio
            {
                id = Guid.NewGuid().ToString(),
                cliente_id = dto.ClienteId,
                estado_servicio_id = dto.EstadoServicioId,
                usuario_id = dto.UsuarioId,
                fecha = DateTime.Now,
                descripcion = dto.Descripcion
            };

            _context.registro_servicios.Add(registroServicio);

            // Asociar empleados si están presentes
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

            decimal totalServicio = 0;
            var registrosVehiculo = new List<registro_servicio_vehiculo>();
            var registrosDetalle = new List<registro_servicio_detalle>();

            // Recorrer vehículos y servicios asociados
            foreach (var vehiculoDto in dto.Vehiculos)
            {
                if (string.IsNullOrEmpty(vehiculoDto.VehiculoId) || vehiculoDto.Servicios == null || !vehiculoDto.Servicios.Any())
                {
                    continue;
                }

                // Crear el registro de servicio de vehículo
                var registroServicioVehiculo = new registro_servicio_vehiculo
                {
                    id = Guid.NewGuid().ToString(),
                    registro_servicio_id = registroServicio.id,
                    vehiculo_id = vehiculoDto.VehiculoId
                };

                registrosVehiculo.Add(registroServicioVehiculo);

                // Recorrer los servicios asociados a cada vehículo
                foreach (var servicio in vehiculoDto.Servicios)
                {
                    if (string.IsNullOrEmpty(servicio.ServicioId))
                        continue;

                    // Crear el detalle del servicio
                    var registroServicioDetalle = new registro_servicio_detalle
                    {
                        id = Guid.NewGuid().ToString(),
                        registro_servicio_vehiculo_id = registroServicioVehiculo.id,
                        servicio_id = servicio.ServicioId,
                        precio = servicio.Precio
                    };

                    registrosDetalle.Add(registroServicioDetalle);
                    totalServicio += servicio.Precio;

                    // Procesar productos asociados y calcular su total
                    if (servicio.Productos != null && servicio.Productos.Any())
                    {
                        foreach (var productoUsage in servicio.Productos)
                        {
                            var producto = await _context.Productos.FindAsync(productoUsage.ProductoId);
                            if (producto != null)
                            {
                                var detalleProducto = new registro_servicio_detalle_producto
                                {
                                    RegistroServicioDetalleId = registroServicioDetalle.id,
                                    ProductoId = producto.id,
                                    Cantidad = productoUsage.Cantidad
                                };

                                _context.registro_servicio_detalle_productos.Add(detalleProducto);

                                // Buscar el precio del producto, multiplicar por la cantidad y sumar al total
                                totalServicio += producto.precio * productoUsage.Cantidad;
                            }
                        }
                    }
                }
            }

            // Asignar el total al servicio
            registroServicio.total = totalServicio;

            _context.registro_servicio_vehiculos.AddRange(registrosVehiculo);
            _context.registro_servicio_detalles.AddRange(registrosDetalle);

            // Registrar pagos si están presentes
            if (dto.Pagos != null && dto.Pagos.Any())
            {
                foreach (var pagoDto in dto.Pagos)
                {
                    var pago = new pago
                    {
                        id = Guid.NewGuid().ToString(),
                        registro_servicio_id = registroServicio.id,
                        metodo_pago = pagoDto.metodo_pago,
                        monto = pagoDto.monto
                    };

                    _context.pagos.Add(pago);
                }
            }

            await _context.SaveChangesAsync();

            // Enviar la nueva orden a los empleados a través de SignalR
            if (dto.Empleados != null && dto.Empleados.Any())
            {
                foreach (var empleadoId in dto.Empleados)
                {
                    await _hubContext.Clients.Group($"employee-{empleadoId}")
                        .SendAsync("newOrder", new
                        {
                            registroServicioId = registroServicio.id,
                            totalServicio = totalServicio,
                            mensaje = "Se te asignó una nueva orden."
                        });
                }
            }

            return Ok(new
            {
                mensaje = "Registro de servicio para múltiples vehículos creado correctamente",
                registroServicioId = registroServicio.id,
                totalServicio = totalServicio
            });
        }


        [HttpPut("multiple")]
        public async Task<IActionResult> UpdateRegistroServicioMultiple([FromBody] RegistroServicioMultipleUpdateDto dto)
        {
            if (dto == null ||
                string.IsNullOrEmpty(dto.RegistroServicioId) ||
                string.IsNullOrEmpty(dto.ClienteId) ||
                string.IsNullOrEmpty(dto.UsuarioId) ||
                string.IsNullOrEmpty(dto.EstadoServicioId) ||
                dto.Vehiculos == null || !dto.Vehiculos.Any())
            {
                return BadRequest("Faltan datos requeridos.");
            }

            var registroServicio = await _context.registro_servicios
                .Include(rs => rs.empleados)
                .Include(rs => rs.registro_servicio_vehiculos)
                    .ThenInclude(rsv => rsv.registro_servicio_detalles)
                .FirstOrDefaultAsync(rs => rs.id == dto.RegistroServicioId);

            if (registroServicio == null)
            {
                return NotFound("Registro de servicio no encontrado.");
            }

            // Actualizar propiedades básicas
            registroServicio.cliente_id = dto.ClienteId;
            registroServicio.estado_servicio_id = dto.EstadoServicioId;
            registroServicio.usuario_id = dto.UsuarioId;
            registroServicio.descripcion = dto.Descripcion;

            // Actualizar empleados
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

            // Eliminar registros existentes de vehículos y sus detalles (incluyendo productos asociados)
            var vehiculosExistentes = registroServicio.registro_servicio_vehiculos.ToList();
            foreach (var vehiculo in vehiculosExistentes)
            {
                var detalles = vehiculo.registro_servicio_detalles.ToList();
                foreach (var detalle in detalles)
                {
                    // Eliminar productos asociados a cada detalle
                    var productosDetalle = _context.registro_servicio_detalle_productos
                        .Where(rdp => rdp.RegistroServicioDetalleId == detalle.id)
                        .ToList();
                    _context.registro_servicio_detalle_productos.RemoveRange(productosDetalle);

                    _context.registro_servicio_detalles.Remove(detalle);
                }
                _context.registro_servicio_vehiculos.Remove(vehiculo);
            }

            decimal totalServicio = 0;

            // Recorrer los vehículos nuevos y crear los registros correspondientes
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
                    totalServicio += servicio.Precio;

                    // Procesar productos asociados al servicio, si existen
                    if (servicio.Productos != null && servicio.Productos.Any())
                    {
                        foreach (var productoUsage in servicio.Productos)
                        {
                            var producto = await _context.Productos.FindAsync(productoUsage.ProductoId);
                            if (producto != null)
                            {
                                var detalleProducto = new registro_servicio_detalle_producto
                                {
                                    RegistroServicioDetalleId = nuevoDetalle.id,
                                    ProductoId = producto.id,
                                    Cantidad = productoUsage.Cantidad
                                };

                                _context.registro_servicio_detalle_productos.Add(detalleProducto);

                                // **Aquí se suma el precio del producto al total del servicio**
                                totalServicio += producto.precio * productoUsage.Cantidad;
                            }
                        }
                    }
                }
            }

            // Actualizar el total del servicio
            registroServicio.total = totalServicio;

            // Actualizar pagos: eliminar los existentes y agregar los nuevos
            var pagosExistentes = _context.pagos.Where(p => p.registro_servicio_id == registroServicio.id).ToList();
            _context.pagos.RemoveRange(pagosExistentes);

            if (dto.Pagos != null && dto.Pagos.Any())
            {
                foreach (var pagoDto in dto.Pagos)
                {
                    var nuevoPago = new pago
                    {
                        id = Guid.NewGuid().ToString(),
                        registro_servicio_id = registroServicio.id,
                        metodo_pago = pagoDto.metodo_pago,
                        monto = pagoDto.monto
                    };
                    _context.pagos.Add(nuevoPago);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Registro de servicio actualizado correctamente",
                registroServicioId = registroServicio.id,
                totalServicio = totalServicio
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

            if (fechaDesde.HasValue)
            {
                var fechaInicio = fechaDesde.Value.Date;
                registrosQuery = registrosQuery.Where(rs => rs.Fecha.Date >= fechaInicio);
            }

            if (fechaHasta.HasValue)
            {
                var fechaFin = fechaHasta.Value.Date;
                registrosQuery = registrosQuery.Where(rs => rs.Fecha.Date <= fechaFin);
            }

            var registros = await registrosQuery.ToListAsync();

            return Ok(registros);
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> GetRegistroServicioForEdit(string id)
        {
            var registroServicio = await _context.registro_servicios
                .Include(rs => rs.empleados)
                .Include(rs => rs.pagos)
                .Include(rs => rs.registro_servicio_vehiculos)
                    .ThenInclude(rsv => rsv.registro_servicio_detalles)
                        .ThenInclude(d => d.RegistroServicioDetalleProductos) // Incluir productos
                            .ThenInclude(dp => dp.Producto) // Incluir información del producto
                .FirstOrDefaultAsync(rs => rs.id == id);




            if (registroServicio == null)
            {
                return NotFound("Registro de servicio no encontrado.");
            }

            var dto = new RegistroServicioMultipleUpdateDto
            {
                RegistroServicioId = registroServicio.id,
                ClienteId = registroServicio.cliente_id,
                Descripcion = registroServicio.descripcion,
                EstadoServicioId = registroServicio.estado_servicio_id,
                UsuarioId = registroServicio.usuario_id,
                Empleados = registroServicio.empleados.Select(e => e.id).ToList(),
                Pagos = registroServicio.pagos.Select(p => new PagoDto
                {
                    metodo_pago = p.metodo_pago,
                    monto = p.monto
                }).ToList(),
                Vehiculos = registroServicio.registro_servicio_vehiculos.Select(v => new RegistroServicioVehiculoDto
                {
                    VehiculoId = v.vehiculo_id,
                    Servicios = v.registro_servicio_detalles.Select(d => new ServicioDto
                    {
                        ServicioId = d.servicio_id,
                        Precio = d.precio,
                        // Mapear los productos asociados: id, cantidad y total
                        Productos = d.RegistroServicioDetalleProductos
                            .Select(pd => new ProductoUsageDto
                            {
                                ProductoId = pd.ProductoId,
                                Cantidad = pd.Cantidad,
                                Precio = _context.Productos
                                    .Where(p => p.id == pd.ProductoId)
                                    .Select(p => p.precio * pd.Cantidad)
                                    .FirstOrDefault() // Obtener el total multiplicando precio * cantidad
                            }).ToList()
                    }).ToList()
                }).ToList() // Aquí cerramos correctamente
            };


            return Ok(dto);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegistroServicioById(string id)
        {
            var registro = await _context.registro_servicios
                .Include(rs => rs.cliente)
                .Include(rs => rs.estado_servicio)
                .Include(rs => rs.pagos)
                .Include(rs => rs.registro_servicio_vehiculos)
                    .ThenInclude(rsv => rsv.vehiculo)
                .Include(rs => rs.registro_servicio_vehiculos)
                    .ThenInclude(rsv => rsv.registro_servicio_detalles)
                        .ThenInclude(rsd => rsd.servicio)
                .Include(rs => rs.registro_servicio_vehiculos)
                    .ThenInclude(rsv => rsv.registro_servicio_detalles)
                        .ThenInclude(rsd => rsd.RegistroServicioDetalleProductos)
                            .ThenInclude(rp => rp.Producto)
                .Include(rs => rs.empleados)
                .FirstOrDefaultAsync(rs => rs.id == id);

            if (registro == null)
            {
                return NotFound("Registro de servicio no encontrado.");
            }

            var dto = new RegistroServicioDetailDto
            {
                Id = registro.id,
                Fecha = registro.fecha,
                Descripcion = registro.descripcion,
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
                        Precio = rsd.precio,
                        Producto = rsd.RegistroServicioDetalleProductos.Select(rp => new ProductoUsageViewDto
                        {
                            ProductoId = rp.ProductoId,
                            Nombre = rp.Producto.nombre,
                            Cantidad = rp.Cantidad,
                            Precio =  rp.Producto.precio
                        }).ToList()

                    }).ToList()
                }).ToList(),
                Empleados = registro.empleados.Select(rse => new EmpleadoDto
                {
                    Id = rse.id,
                    Nombre = rse.nombre,
                    Apellido = rse.apellido,
                    Correo = rse.correo
                }).ToList(),
                Pagos = registro.pagos.Select(p => new PagoDto
                {
                    metodo_pago = p.metodo_pago,
                    monto = p.monto
                }).ToList()
            };

            return Ok(dto);
        }



    }
}
