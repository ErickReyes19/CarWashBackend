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

        return Ok(clientes);
    }


    [HttpPost]
    public async Task<IActionResult> CreateCliente(Cliente cliente)
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

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveClientes()
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

        return Ok(clientesActivos);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetClienteById(string id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            return NotFound();

        return Ok(cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCliente(string id, Cliente cliente)
    {
        if (id != cliente.id)
            return BadRequest("El ID no coincide.");

        var existingCliente = await _context.Clientes.FindAsync(id);
        if (existingCliente == null)
            return NotFound();

        existingCliente.nombre = cliente.nombre;
        existingCliente.correo = cliente.correo;
        existingCliente.telefono = cliente.telefono;
        existingCliente.genero = cliente.genero;
        existingCliente.activo = cliente.activo;
        existingCliente.updated_at = DateTime.UtcNow;

        _context.Clientes.Update(existingCliente);
        await _context.SaveChangesAsync();

        // Retorna el cliente actualizado
        return Ok(existingCliente);
    }

}
