using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWashBackend.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Esto asegura que solo usuarios autenticados puedan acceder
    public class PagoController : ControllerBase
    {
        private readonly CarwashContext _context;

        public PagoController(CarwashContext context)
        {
            _context = context;
        }

        // GET: api/Pago
        [HttpGet]
        public async Task<IActionResult> GetAllPagos()
        {
            var pagos = await _context.Pagos
                .Include(p => p.registro_servicio) // Incluimos el registro de servicio relacionado
                .ToListAsync();

            if (pagos == null || !pagos.Any())
                return NotFound("No se encontraron pagos.");

            var pagosDto = pagos.Select(p => new 
            {
                p.id,
                p.monto,
                p.metodo_pago,
                p.fecha,
                p.created_at,
                p.updated_at,
                registro_servicio_id = p.registro_servicio_id
            }).ToList();

            return Ok(pagosDto);
        }


        // GET: api/Pago/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPagoById(string id)
        {
            var pago = await _context.Pagos
                .Include(p => p.registro_servicio) // Incluimos el registro de servicio relacionado
                .FirstOrDefaultAsync(p => p.id == id);

            if (pago == null)
                return NotFound($"No se encontró un pago con el ID {id}.");

            var pagoDto = new
            {
                pago.id,
                pago.monto,
                pago.metodo_pago,
                pago.fecha,
                pago.created_at,
                pago.updated_at,
                registro_servicio_id = pago.registro_servicio_id
            };

            return Ok(pagoDto);
        }

        // POST: api/Pago
        [HttpPost]
        public async Task<IActionResult> CreatePago(Pago pago)
        {
            if (pago == null)
                return BadRequest("Los datos enviados son inválidos.");

            pago.id = Guid.NewGuid().ToString();
            pago.created_at = DateTime.UtcNow;
            pago.updated_at = DateTime.UtcNow;

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            var pagoDto = new
            {
                pago.id,
                pago.monto,
                pago.metodo_pago,
                pago.fecha,
                pago.created_at,
                pago.updated_at,
                registro_servicio_id = pago.registro_servicio_id
            };

            return CreatedAtAction(nameof(GetPagoById), new { id = pago.id }, pagoDto);
        }

        // PUT: api/Pago/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePago(string id, Pago pago)
        {
            if (id != pago.id)
                return BadRequest("El ID no coincide con el del pago enviado.");

            var existingPago = await _context.Pagos
                .FirstOrDefaultAsync(p => p.id == id);

            if (existingPago == null)
                return NotFound($"No se encontró un pago con el ID {id}.");

            existingPago.monto = pago.monto;
            existingPago.metodo_pago = pago.metodo_pago;
            existingPago.fecha = pago.fecha;
            existingPago.updated_at = DateTime.UtcNow;

            _context.Pagos.Update(existingPago);
            await _context.SaveChangesAsync();

            var pagoDto = new
            {
                existingPago.id,
                existingPago.monto,
                existingPago.metodo_pago,
                existingPago.fecha,
                existingPago.created_at,
                existingPago.updated_at,
                registro_servicio_id = existingPago.registro_servicio_id
            };

            return Ok(pagoDto);
        }
    }
}
