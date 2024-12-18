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
            // Obtiene todos los vehículos, incluyendo el cliente asociado
            var vehiculos = await _context.Vehiculos
                                          .Include(v => v.cliente)
                                          .ToListAsync();

            if (vehiculos == null || vehiculos.Count == 0)
                return NotFound();

            // Mapeamos los vehículos a un DTO para evitar el ciclo de referencia
            var vehiculosDto = vehiculos.Select(v => new VehiculoDTO
            {
                id = v.id,
                placa = v.placa,
                modelo = v.modelo,
                marca = v.marca,
                color = v.color,
                activo = v.activo,
                created_at = v.created_at,
                updated_at = v.updated_at,
                ClienteNombre = v.cliente?.nombre
            }).ToList();

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
                                                 .FirstOrDefaultAsync(v => v.placa == vehiculo.placa);

            if (existingVehiculo != null)
            {
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
                ClienteNombre = vehiculo.cliente?.nombre
            };

            return CreatedAtAction(nameof(GetVehiculoById), new { id = vehiculo.id }, vehiculoCreatedDto);
        }
        catch (Exception ex)
        {
            // Manejo de excepción
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVehiculoById(string id)
    {
        try
        {
            var vehiculo = await _context.Vehiculos
                                         .Include(v => v.cliente)
                                         .FirstOrDefaultAsync(v => v.id == id);

            if (vehiculo == null)
                return NotFound();

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
                ClienteNombre = vehiculo.cliente?.nombre
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
            if (id != vehiculo.id)
                return BadRequest("El ID no coincide.");

            var existingVehiculo = await _context.Vehiculos.FindAsync(id);
            if (existingVehiculo == null)
                return NotFound(new { message = "Vehículo no encontrado." });

            var vehicleWithSamePlaca = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.placa == vehiculo.placa && v.id != id);

            if (vehicleWithSamePlaca != null)
            {
                return Conflict(new { message = "Ya existe un vehículo con la misma placa." });
            }

            var clienteExistente = await _context.Clientes
                                                  .FirstOrDefaultAsync(c => c.id == vehiculo.cliente_id);

            if (clienteExistente == null)
            {
                return NotFound(new { message = "Cliente no encontrado." });
            }

            existingVehiculo.placa = vehiculo.placa;
            existingVehiculo.modelo = vehiculo.modelo;
            existingVehiculo.marca = vehiculo.marca;
            existingVehiculo.color = vehiculo.color;
            existingVehiculo.activo = vehiculo.activo;
            existingVehiculo.cliente_id = vehiculo.cliente_id;
            existingVehiculo.updated_at = DateTime.UtcNow;

            _context.Vehiculos.Update(existingVehiculo);
            await _context.SaveChangesAsync();

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
                ClienteNombre = clienteExistente?.nombre
            };

            return Ok(vehiculoUpdatedDto);
        }
        catch (Exception ex)
        {
            // Manejo de excepción
            return StatusCode(500, new { message = "Error en el servidor", details = ex.Message });
        }
    }
}
