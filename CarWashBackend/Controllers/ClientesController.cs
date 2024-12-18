using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Esto asegura que solo usuarios autenticados puedan acceder
public class ClienteController : ControllerBase
{
    private readonly CarwashContext _context;

    public ClienteController(CarwashContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClientes()
    {
        try
        {
            var clientes = await _context.Clientes
                                        .Select(c => new ClienteDTO
                                        {
                                            Id = c.id,
                                            Nombre = c.nombre,
                                            Correo = c.correo,
                                            Telefono = c.telefono,
                                            Genero = c.genero,
                                            Activo = c.activo
                                        })
                                        .ToListAsync();

            if (clientes == null || !clientes.Any())
                return NotFound("No se encontraron clientes.");

            return Ok(clientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener los clientes: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCliente(Cliente cliente)
    {
        try
        {
            if (cliente == null)
                return BadRequest("Datos inválidos.");

            cliente.id = Guid.NewGuid().ToString();
            cliente.created_at = DateTime.UtcNow;
            cliente.updated_at = DateTime.UtcNow;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClienteById), new { id = cliente.id }, cliente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear el cliente: {ex.Message}");
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveClientes()
    {
        try
        {
            var clientesActivos = await _context.Clientes
                                                .Where(c => c.activo == true)
                                                .Select(c => new ClienteDTO
                                                {
                                                    Id = c.id,
                                                    Nombre = c.nombre,
                                                    Correo = c.correo,
                                                    Telefono = c.telefono,
                                                    Genero = c.genero,
                                                    Activo = c.activo
                                                })
                                                .ToListAsync();

            if (clientesActivos == null || !clientesActivos.Any())
                return NotFound("No se encontraron clientes activos.");

            return Ok(clientesActivos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener los clientes activos: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClienteById(string id)
    {
        try
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound($"Cliente con ID {id} no encontrado.");

            var clienteDTO = new ClienteDTO
            {
                Id = cliente.id,
                Nombre = cliente.nombre,
                Correo = cliente.correo,
                Telefono = cliente.telefono,
                Genero = cliente.genero,
                Activo = cliente.activo
            };

            return Ok(clienteDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener el cliente: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCliente(string id, Cliente cliente)
    {
        try
        {
            if (id != cliente.id)
                return BadRequest("El ID no coincide.");

            var existingCliente = await _context.Clientes.FindAsync(id);
            if (existingCliente == null)
                return NotFound($"Cliente con ID {id} no encontrado.");

            existingCliente.nombre = cliente.nombre;
            existingCliente.correo = cliente.correo;
            existingCliente.telefono = cliente.telefono;
            existingCliente.genero = cliente.genero;
            existingCliente.activo = cliente.activo;
            existingCliente.updated_at = DateTime.UtcNow;

            _context.Clientes.Update(existingCliente);
            await _context.SaveChangesAsync();

            return Ok(existingCliente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al actualizar el cliente: {ex.Message}");
        }
    }
}
