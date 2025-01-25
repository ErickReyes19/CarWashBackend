using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;

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
            // Obtiene todos los vehículos, incluyendo los clientes asociados a través de la relación muchos a muchos
            var vehiculos = await _context.Vehiculos
                .Include(v => v.clientes) // Incluye los clientes asociados a cada vehículo
                .ToListAsync();

            if (vehiculos == null || vehiculos.Count == 0)
                return NotFound(new { message = "No se encontraron vehículos" });

            // Mapeamos los vehículos a un DTO para evitar el ciclo de referencia
            var vehiculosDto = vehiculos.Select(v => new VehiculoClienteDTO
            {
                id = v.id,
                placa = v.placa,
                modelo = v.modelo,
                marca = v.marca,
                color = v.color,
                activo = v.activo,
                created_at = v.created_at,
                updated_at = v.updated_at,
                // Para cada cliente asociado, incluimos su nombre y ID
                Clientes = v.clientes.Select(c => new ClienteSummaryDTO
                {
                    Id = c.id,
                    Nombre = c.nombre
                }).ToList()
            }).ToList();

            return Ok(vehiculosDto);
        }
        catch (Exception ex)
        {
            // Manejo de excepción
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }



    [HttpGet("active")]
    public async Task<IActionResult> GetActiveVehiculos()
    {
        try
        {
            // Obtener los vehículos activos junto con la información de los clientes asociados
            var vehiculosActivos = await _context.Vehiculos
                                                 .Where(v => v.activo == true)  // Filtra solo los vehículos activos
                                                 .Include(v => v.clientes)  // Incluye los clientes asociados a cada vehículo
                                                 .ToListAsync();

            if (vehiculosActivos == null || vehiculosActivos.Count == 0)
                return NotFound(new { message = "No se encontraron vehículos activos" });

            // Mapear los vehículos activos a un DTO para evitar el ciclo de referencia
            var vehiculosDTO = vehiculosActivos.Select(v => new VehiculoClienteDTO
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

            return Ok(vehiculosDTO);  // Retorna la lista de DTOs con los vehículos y los clientes asociados
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }


    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> GetVehiculosByClienteId(string clienteId)
    {
        try
        {
            // Obtener el cliente con sus vehículos asociados usando la relación muchos a muchos
            var cliente = await _context.Clientes
                                        .Where(c => c.id == clienteId) // Filtrar por el clienteId
                                        .Include(c => c.vehiculos) // Incluir los vehículos asociados a este cliente
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
                // Si el vehículo ya está asignado a un cliente, se devuelve la información del cliente
                if (existingVehiculo.clientes.Any())
                {
                    var clienteAsignado = existingVehiculo.clientes.FirstOrDefault();
                    return Conflict(new
                    {
                        message = "El vehículo ya está asignado a un cliente.",
                        clienteId = clienteAsignado.id,
                        clienteNombre = clienteAsignado.nombre
                    });
                }

                return Conflict(new { message = "Ya existe un vehículo con la misma placa." });
            }

            // Verifica si el cliente existe
            var clienteExistente = await _context.Clientes
                                                 .FirstOrDefaultAsync(c => c.id == vehiculo.cliente_id);

            if (clienteExistente == null)
            {
                return NotFound(new { message = "El cliente asociado no existe." });
            }

            // Si el cliente existe, creamos el nuevo vehículo
            vehiculo.id = Guid.NewGuid().ToString();
            vehiculo.created_at = DateTime.UtcNow;
            vehiculo.updated_at = DateTime.UtcNow;

            // Asocia al cliente con el vehículo
            vehiculo.clientes.Add(clienteExistente);

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            // Retorna el nuevo vehículo creado en el DTO
            var vehiculoCreatedDto = new VehiculoDTO
            {
                id = vehiculo.id,
                placa = vehiculo.placa,
                modelo = vehiculo.modelo,
                marca = vehiculo.marca,
                color = vehiculo.color,
                activo = vehiculo.activo,
                created_at = vehiculo.created_at,
                updated_at = vehiculo.updated_at,
                ClienteNombre = vehiculo.clientes?.FirstOrDefault()?.nombre
            };

            return CreatedAtAction(nameof(GetVehiculoById), new { id = vehiculo.id }, vehiculoCreatedDto);
        }
        catch (Exception ex)
        {
            // Manejo de excepción
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }



    [HttpPut("{vehiculoId}/agregar-cliente/{clienteId}")]
    public async Task<IActionResult> AddClienteToVehiculo(string vehiculoId, string clienteId)
    {
        try
        {
            // Buscar el vehículo y el cliente
            var vehiculo = await _context.Vehiculos.Include(v => v.clientes).FirstOrDefaultAsync(v => v.id == vehiculoId);
            var cliente = await _context.Clientes.FindAsync(clienteId);

            // Verificar si el vehículo y el cliente existen
            if (vehiculo == null)
                return NotFound(new { message = "Vehículo no encontrado" });

            if (cliente == null)
                return NotFound(new { message = "Cliente no encontrado" });

            // Verificar si el cliente ya está asociado al vehículo
            if (vehiculo.clientes.Any(c => c.id == clienteId))
            {
                return Conflict(new { message = "Este cliente ya está asociado con el vehículo." });
            }

            // Agregar la relación (cliente al vehículo)
            vehiculo.clientes.Add(cliente);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cliente agregado al vehículo correctamente" });
        }
        catch (Exception ex)
        {
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

            if (vehiculo == null)
                return NotFound(new { message = "Vehículo no encontrado." });

            // Si el vehículo tiene clientes asociados, tomamos el nombre del primero (o el más relevante)
            var clienteNombre = vehiculo.clientes?.FirstOrDefault()?.nombre;

            var vehiculoDto = new VehiculoDTO
            {
                id = vehiculo.id,
                placa = vehiculo.placa,
                modelo = vehiculo.modelo,
                marca = vehiculo.marca,
                color = vehiculo.color,
                activo = vehiculo.activo,
                created_at = vehiculo.created_at,
                updated_at = vehiculo.updated_at,
                ClienteNombre = clienteNombre // Nombre del primer cliente asociado
            };

            return Ok(vehiculoDto);
        }
        catch (Exception ex)
        {
            // Manejo de excepción
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehiculo(string id, Vehiculo vehiculo)
    {
        try
        {
            if (vehiculo == null || id != vehiculo.id)
                return BadRequest("Datos inválidos.");

            // Verifica si el vehículo existe
            var existingVehiculo = await _context.Vehiculos
                                                 .Include(v => v.clientes)  // Incluye los clientes asociados
                                                 .FirstOrDefaultAsync(v => v.id == id);

            if (existingVehiculo == null)
            {
                return NotFound(new { message = "Vehículo no encontrado." });
            }

            // Si el vehículo ya tiene un cliente asignado, devolvemos la información del cliente asignado
            if (existingVehiculo.clientes.Any())
            {
                var clienteAsignado = existingVehiculo.clientes.FirstOrDefault();
                return Conflict(new
                {
                    message = "El vehículo ya está asignado a un cliente.",
                    clienteId = clienteAsignado.id,
                    clienteNombre = clienteAsignado.nombre
                });
            }

            // Si el vehículo no tiene cliente asignado, actualizamos el vehículo
            existingVehiculo.placa = vehiculo.placa;
            existingVehiculo.modelo = vehiculo.modelo;
            existingVehiculo.marca = vehiculo.marca;
            existingVehiculo.color = vehiculo.color;
            existingVehiculo.activo = vehiculo.activo;
            existingVehiculo.updated_at = DateTime.UtcNow;

            // Si se proporciona un cliente_id, asociamos al vehículo con ese cliente
            if (!string.IsNullOrEmpty(vehiculo.cliente_id))
            {
                var clienteExistente = await _context.Clientes
                                                     .FirstOrDefaultAsync(c => c.id == vehiculo.cliente_id);

                if (clienteExistente == null)
                {
                    return NotFound(new { message = "El cliente asociado no existe." });
                }

                existingVehiculo.clientes.Clear(); // Limpiar clientes previos si los hay
                existingVehiculo.clientes.Add(clienteExistente); // Asociar el nuevo cliente
            }

            await _context.SaveChangesAsync();

            // Crear el DTO de respuesta
            var vehiculoUpdatedDto = new VehiculoDTO
            {
                id = existingVehiculo.id,
                placa = existingVehiculo.placa,
                modelo = existingVehiculo.modelo,
                marca = existingVehiculo.marca,
                color = existingVehiculo.color,
                activo = existingVehiculo.activo,
                created_at = existingVehiculo.created_at,
                updated_at = existingVehiculo.updated_at,
                ClienteNombre = string.Join(", ", existingVehiculo.clientes.Select(c => c.nombre))
            };

            return Ok(vehiculoUpdatedDto);  // Retorna el vehículo actualizado en formato DTO
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
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
