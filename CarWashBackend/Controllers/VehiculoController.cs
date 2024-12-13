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
        // Obtiene todos los vehículos, incluyendo el cliente asociado
        var vehiculos = await _context.Vehiculos
                                      .Include(v => v.cliente)  // Incluye la relación con el cliente
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
            ClienteNombre = v.cliente?.nombre  // Solo el nombre del cliente
        }).ToList();

        return Ok(vehiculosDto);
    }


    [HttpPost]
    public async Task<IActionResult> CreateVehiculo(Vehiculo vehiculo)
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
            ClienteNombre = vehiculo.cliente?.nombre // Nombre del cliente
        };

        return CreatedAtAction(nameof(GetVehiculoById), new { id = vehiculo.id }, vehiculoCreatedDto);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveVehiculos()
    {
        // Obtener los vehículos activos junto con la información del cliente
        var vehiculosActivos = await _context.Vehiculos
                                              .Where(v => v.activo == true)
                                              .Include(v => v.cliente)  // Incluir el cliente
                                              .ToListAsync();

        // Mapear los vehículos activos a un DTO
        var vehiculosDTO = vehiculosActivos.Select(v => new VehiculoDTO
        {
            id = v.id,
            placa = v.placa,
            modelo = v.modelo,
            marca = v.marca,
            color = v.color,
            activo = v.activo,
            created_at = v.created_at,
            updated_at = v.updated_at,
            ClienteNombre = v.cliente?.nombre // Nombre del cliente asociado
        }).ToList();

        return Ok(vehiculosDTO);  // Retorna la lista de DTOs
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetVehiculoById(string id)
    {
        var vehiculo = await _context.Vehiculos
                                     .Include(v => v.cliente)  // Incluye el cliente relacionado
                                     .FirstOrDefaultAsync(v => v.id == id);

        if (vehiculo == null)
            return NotFound();

        // Mapeamos a un DTO para evitar problemas con las relaciones circulares
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
            ClienteNombre = vehiculo.cliente?.nombre // Solo incluir el nombre del cliente
        };

        return Ok(vehiculoDto);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVehiculo(string id, Vehiculo vehiculo)
    {
        if (id != vehiculo.id)
            return BadRequest("El ID no coincide.");

        var existingVehiculo = await _context.Vehiculos.FindAsync(id);
        if (existingVehiculo == null)
            return NotFound();

        // Verificar si ya existe un vehículo con la misma placa, pero que no sea el mismo vehículo
        var vehicleWithSamePlaca = await _context.Vehiculos
            .FirstOrDefaultAsync(v => v.placa == vehiculo.placa && v.id != id);

        if (vehicleWithSamePlaca != null)
        {
            return Conflict(new { message = "Ya existe un vehículo con la misma placa." });
        }

        // Actualizar los datos del vehículo
        existingVehiculo.placa = vehiculo.placa;
        existingVehiculo.modelo = vehiculo.modelo;
        existingVehiculo.marca = vehiculo.marca;
        existingVehiculo.color = vehiculo.color;
        existingVehiculo.activo = vehiculo.activo;
        existingVehiculo.updated_at = DateTime.UtcNow;

        _context.Vehiculos.Update(existingVehiculo);
        await _context.SaveChangesAsync();

        // Crear el DTO con los datos actualizados
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
            ClienteNombre = existingVehiculo.cliente?.nombre  // Obtener nombre del cliente
        };

        // Retornar el DTO con el vehículo actualizado
        return Ok(vehiculoUpdatedDto);
    }


}
