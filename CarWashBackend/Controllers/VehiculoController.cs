using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Esto asegura que solo usuarios autenticados puedan acceder
public class VehiculoController : ControllerBase
{
    private readonly CarwashContext _context;

    public VehiculoController(CarwashContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllVehiculos()
    {
        try
        {
            var vehiculosDto = await _context.Vehiculos
                .AsNoTracking()
                .Include(v => v.clientes)
                .Select(v => new VehiculoClienteDTO
                {
                    id = v.id,
                    placa = v.placa,
                    modelo = v.modelo,
                    marca = v.marca,
                    color = v.color,
                    activo = v.activo,
                    created_at = v.created_at,
                    updated_at = v.updated_at,
                    Clientes = v.clientes.Select(c => new ClienteSummaryDTO
                    {
                        Id = c.id,
                        Nombre = c.nombre
                    }).ToList()
                })
                .ToListAsync();

            if (vehiculosDto.Count == 0)
                return NotFound(new { message = "No se encontraron vehículos" });

            return Ok(vehiculosDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error interno en el servidor",
                error = ex.Message
            });
        }
    }



    [HttpGet("active")]
    public async Task<IActionResult> GetActiveVehiculos()
    {
        try
        {
            var vehiculosDTO = await _context.Vehiculos
                .AsNoTracking()
                .Where(v => v.activo.HasValue && v.activo.Value)  // Verifica si 'activo' no es null y es true
                .Include(v => v.clientes)  // Incluye los clientes asociados a cada vehículo
                .Select(v => new VehiculoClienteDTO
                {
                    id = v.id,
                    placa = v.placa,
                    modelo = v.modelo,
                    marca = v.marca,
                    color = v.color,
                    activo = v.activo,
                    created_at = v.created_at,
                    updated_at = v.updated_at,
                    Clientes = v.clientes.Select(c => new ClienteSummaryDTO
                    {
                        Id = c.id,
                        Nombre = c.nombre
                    }).ToList()
                })
                .ToListAsync();

            if (vehiculosDTO.Count == 0)
                return NotFound(new { message = "No se encontraron vehículos activos" });

            return Ok(vehiculosDTO);
        }
        catch (Exception ex)
        {
            // Manejo de excepciones con más detalle
            return StatusCode(500, new
            {
                message = "Error interno en el servidor",
                error = ex.Message
            });
        }
    }



    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetVehiculosByClienteId(string clienteId)
    {
        try
        {
            // Obtener el cliente con sus vehículos asociados usando la relación muchos a muchos
            var cliente = await _context.Clientes
                .AsNoTracking()  // Evita el seguimiento de los cambios para mejorar el rendimiento
                .Where(c => c.id == clienteId)  // Filtra por el clienteId
                .Include(c => c.vehiculos)  // Incluye los vehículos asociados a este cliente
                .FirstOrDefaultAsync();

            // Si no se encuentra el cliente o no tiene vehículos, retorna un mensaje indicando que no existen
            if (cliente == null || !cliente.vehiculos.Any())
            {
                return NotFound(new { message = "No se encontraron vehículos para el cliente con ID " + clienteId });
            }

            // Crear el DTO para los vehículos encontrados
            var vehiculosDto = cliente.vehiculos.Select(v => new VehiculoClienteDTO
            {
                id = v.id,
                placa = v.placa,
                modelo = v.modelo,
                marca = v.marca,
                color = v.color,
                activo = v.activo,
                created_at = v.created_at,
                updated_at = v.updated_at,
                // Lista de clientes asociados, cada uno mapeado a un ClienteDTO
                Clientes = v.clientes.Select(c => new ClienteSummaryDTO
                {
                    Id = c.id,
                    Nombre = c.nombre
                }).ToList()
            }).ToList();

            // Retornar la lista de vehículos en formato DTO
            return Ok(vehiculosDto);
        }
        catch (Exception ex)
        {
            // Manejo de excepción
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehiculo(Vehiculo vehiculo)
    {
        try
        {
            if (vehiculo == null)
                return BadRequest("Datos inválidos.");

            // Verifica si ya existe un vehículo con la misma placa
            var existingVehiculo = await _context.Vehiculos
                                                 .Include(v => v.clientes)  // Incluye los clientes asociados
                                                 .FirstOrDefaultAsync(v => v.placa == vehiculo.placa);

            if (existingVehiculo != null)
            {
                // Si el vehículo ya está asignado a algún cliente
                if (existingVehiculo.clientes != null && existingVehiculo.clientes.Any())
                {
                    var clientesAsignados = existingVehiculo.clientes.Select(c => new
                    {
                        clienteId = c.id,
                        clienteNombre = c.nombre
                    }).ToList();

                    return Conflict(new
                    {
                        message = "El vehículo ya está asignado a uno o más clientes.",
                        idVehiculo = existingVehiculo.id,
                        clientes = clientesAsignados
                    });
                }

                // Si el vehículo existe pero no está asignado a ningún cliente
                return Conflict(new { message = "Ya existe un vehículo con la misma placa, pero no está asignado a un cliente." });
            }

            // Verificar que los clientes asociados existen
            var clientesExistentes = new List<Cliente>();
            foreach (var cliente in vehiculo.clientes)
            {
                var clienteExistente = await _context.Clientes
                                                      .FirstOrDefaultAsync(c => c.id == cliente.id);
                if (clienteExistente != null)
                {
                    clientesExistentes.Add(clienteExistente);
                }
                else
                {
                    return NotFound(new { message = $"El cliente con ID {cliente.id} no existe." });
                }
            }

            // Si todos los clientes son válidos, asociamos los clientes al vehículo
            vehiculo.clientes = clientesExistentes;

            // Asignamos los valores para el nuevo vehículo
            vehiculo.id = Guid.NewGuid().ToString();
            vehiculo.created_at = DateTime.UtcNow;
            vehiculo.updated_at = DateTime.UtcNow;

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            // Retorna el nuevo vehículo creado
            var vehiculoCreatedDto = new VehiculoDTO
            {
                id = vehiculo.id,
                placa = vehiculo.placa,
                modelo = vehiculo.modelo,
                marca = vehiculo.marca,
                color = vehiculo.color,
                activo = vehiculo.activo ?? false,
                clientes = vehiculo.clientes.Select(c => new ClienteSummaryDTO
                {
                    Id = c.id,
                    Nombre = c.nombre
                }).ToList()
            };

            return CreatedAtAction(nameof(GetVehiculoById), new { id = vehiculo.id }, vehiculoCreatedDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }




    [HttpPut("{vehiculoId}/agregar-clientes")]
    public async Task<IActionResult> AddClientesToVehiculo(string vehiculoId, [FromBody] List<string> clienteIds)
    {
        try
        {
            // Verificar que la lista de IDs no esté vacía
            if (clienteIds == null || !clienteIds.Any())
            {
                return BadRequest(new { message = "Debe proporcionar al menos un ID de cliente." });
            }

            // Buscar el vehículo e incluir los clientes asociados
            var vehiculo = await _context.Vehiculos
                                          .Include(v => v.clientes)
                                          .FirstOrDefaultAsync(v => v.id == vehiculoId);

            if (vehiculo == null)
            {
                return NotFound(new { message = "Vehículo no encontrado" });
            }

            // Buscar todos los clientes por los IDs proporcionados
            var clientes = await _context.Clientes
                                         .Where(c => clienteIds.Contains(c.id))
                                         .ToListAsync();

            // Verificar si todos los clientes existen
            var clientesNoExistentes = clienteIds.Except(clientes.Select(c => c.id)).ToList();
            if (clientesNoExistentes.Any())
            {
                return NotFound(new { message = "Algunos clientes no existen.", clientesInexistentes = clientesNoExistentes });
            }

            // Filtrar los clientes que no están ya asociados al vehículo
            var clientesExistentes = clientes.Where(c => !vehiculo.clientes.Any(vc => vc.id == c.id)).ToList();

            // Agregar los clientes al vehículo
            vehiculo.clientes.AddRange(clientesExistentes);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Retornar una respuesta exitosa con los clientes agregados
            return Ok(new
            {
                message = "Clientes agregados al vehículo correctamente",
                clientesAgregados = clientesExistentes.Select(c => new { c.id, c.nombre })
            });
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetVehiculoById(string id)
    {
        try
        {
            // Obtener el vehículo y su relación con los clientes a través de la tabla intermedia
            var vehiculo = await _context.Vehiculos
                                         .Include(v => v.clientes) // Incluir los clientes relacionados
                                         .FirstOrDefaultAsync(v => v.id == id);

            // Verificar si el vehículo existe
            if (vehiculo == null)
            {
                return NotFound(new { message = "Vehículo no encontrado." });
            }

            // Crear el DTO para el vehículo con sus clientes asociados
            var vehiculoDto = new VehiculoClienteDTO
            {
                id = vehiculo.id,
                placa = vehiculo.placa,
                modelo = vehiculo.modelo,
                marca = vehiculo.marca,
                color = vehiculo.color,
                activo = vehiculo.activo,
                created_at = vehiculo.created_at,
                updated_at = vehiculo.updated_at,
                // Si el vehículo tiene clientes asociados, se los mapea, si no, será una lista vacía
                Clientes = vehiculo.clientes.Select(c => new ClienteSummaryDTO
                {
                    Id = c.id,
                    Nombre = c.nombre
                }).ToList()
            };

            // Retornar la información del vehículo con los clientes
            return Ok(vehiculoDto);
        }
        catch (Exception ex)
        {
            // Manejo de excepción en caso de error en el servidor
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehiculo(string id, Vehiculo vehiculo)
    {
        try
        {
            if (vehiculo == null || id != vehiculo.id)
                return BadRequest(new { message = "Datos inválidos." });

            // Verifica si el vehículo existe
            var existingVehiculo = await _context.Vehiculos
                                                 .Include(v => v.clientes)  // Incluye los clientes asociados
                                                 .FirstOrDefaultAsync(v => v.id == id);

            if (existingVehiculo == null)
            {
                return NotFound(new { message = "Vehículo no encontrado." });
            }

            // Actualiza la información del vehículo
            existingVehiculo.placa = vehiculo.placa;
            existingVehiculo.modelo = vehiculo.modelo;
            existingVehiculo.marca = vehiculo.marca;
            existingVehiculo.color = vehiculo.color;
            existingVehiculo.activo = vehiculo.activo ?? false;
            existingVehiculo.updated_at = DateTime.UtcNow;

            // Si se proporciona un conjunto de clientes, validamos y actualizamos
            if (vehiculo.clientes != null && vehiculo.clientes.Any())
            {
                // Obtener todos los IDs de los clientes del body
                var clienteIds = vehiculo.clientes.Select(c => c.id).ToList();

                // Verificar si los clientes existen en la base de datos
                var clientesExistentes = await _context.Clientes
                                                        .Where(c => clienteIds.Contains(c.id))
                                                        .ToListAsync();

                // Verificar si hay algún cliente que no existe
                var clientesNoExistentes = clienteIds.Except(clientesExistentes.Select(c => c.id)).ToList();

                if (clientesNoExistentes.Any())
                {
                    return NotFound(new { message = "Algunos clientes no existen.", clientesInexistentes = clientesNoExistentes });
                }

                // Limpiamos los clientes actuales asociados al vehículo
                existingVehiculo.clientes.Clear();

                // Asociamos los clientes existentes al vehículo
                existingVehiculo.clientes.AddRange(clientesExistentes);
            }

            // Guardamos los cambios en la base de datos
            await _context.SaveChangesAsync();

            // Creamos el DTO de respuesta
            var vehiculoUpdatedDto = new VehiculoDTO
            {
                id = existingVehiculo.id,
                placa = existingVehiculo.placa,
                modelo = existingVehiculo.modelo,
                marca = existingVehiculo.marca,
                color = existingVehiculo.color,
                activo = existingVehiculo.activo ?? false,
                clientes = existingVehiculo.clientes.Select(c => new ClienteSummaryDTO
                {
                    Id = c.id,
                    Nombre = c.nombre
                }).ToList()
            };

            return Ok(vehiculoUpdatedDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }



    [HttpPut("{vehiculoId}/asignar-clientes")]
    public async Task<IActionResult> AssignClientsToVehiculo(string vehiculoId, [FromBody] List<string> clienteIds)
    {
        try
        {
            // Buscar el vehículo
            var vehiculo = await _context.Vehiculos.Include(v => v.clientes)
                                                    .FirstOrDefaultAsync(v => v.id == vehiculoId);
            if (vehiculo == null)
                return NotFound(new { message = "Vehículo no encontrado." });

            // Limpiar los clientes actuales del vehículo (si quieres reemplazar todos)
            vehiculo.clientes.Clear();

            // Verificar que los clientes existan y agregarlos al vehículo
            foreach (var clienteId in clienteIds)
            {
                var cliente = await _context.Clientes.FindAsync(clienteId);
                if (cliente == null)
                {
                    return NotFound(new { message = $"Cliente con ID {clienteId} no encontrado." });
                }

                // Asignar el cliente al vehículo
                vehiculo.clientes.Add(cliente);
            }

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "Clientes asignados correctamente al vehículo", vehiculoId = vehiculoId });
        }
        catch (Exception ex)
        {
            // Manejo de errores
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }


}
