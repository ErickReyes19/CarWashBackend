using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CierreController : ControllerBase
{
    private readonly CarwashContext _context;
    private readonly TimeZoneInfo _timeZoneHonduras;

    public CierreController(CarwashContext context)
    {
        _context = context;
        _timeZoneHonduras = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"); // Honduras (UTC-6)
    }

    // POST: api/cierre
    [HttpPost]
    public async Task<IActionResult> CrearCierre()
    {
        DateTime fechaHoy = DateTime.Now.Date;

        // Verificar si ya existe un cierre para el día
        bool existeCierre = await _context.Cierres.AnyAsync(c => c.Fecha.Date == fechaHoy);
        if (existeCierre)
        {
            return BadRequest(new { mensaje = "Ya existe un cierre para el día de hoy." });
        }

        // Crear el cierre
        var cierre = new Cierre
        {
            Fecha = fechaHoy,
            Total = 0
        };
        _context.Cierres.Add(cierre);
        await _context.SaveChangesAsync();

        // Obtener registros de servicio del día
        var registros = await _context.registro_servicios
                                      .Where(rs => rs.fecha.Date == fechaHoy)
                                      .Include(rs => rs.pagos)
                                      .Include(rs => rs.registro_servicio_vehiculos)
                                          .ThenInclude(rs_veh => rs_veh.registro_servicio_detalles)
                                      .ToListAsync();

        decimal totalCierre = 0;

        foreach (var registro in registros)
        {
            foreach (var pago in registro.pagos)
            {
                var cierreDetalle = new CierreDetalle
                {
                    CierreId = cierre.Id,
                    Monto = pago.monto,
                    MetodoPago = pago.metodo_pago
                };

                _context.CierreDetalles.Add(cierreDetalle);
                totalCierre += pago.monto;
            }

            registro.CierreId = cierre.Id;
            _context.Entry(registro).State = EntityState.Modified;
        }

        cierre.Total = totalCierre;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Cierre realizado con éxito", cierreId = cierre.Id, total = cierre.Total });
    }

    // GET: api/cierre
    [HttpGet]
    public async Task<IActionResult> ObtenerCierres()
    {
        var cierres = await _context.Cierres
            .Include(c => c.CierreDetalles) // Incluir detalles
            .OrderByDescending(c => c.Fecha)
            .Select(c => new
            {
                c.Id,
                c.Fecha,
                c.Total,
                MetodosPago = c.CierreDetalles
                    .GroupBy(d => d.MetodoPago)
                    .Select(g => new
                    {
                        MetodoPago = g.Key,
                        Total = g.Sum(d => d.Monto)
                    })
                    .ToList() // Convertir en lista
            })
            .ToListAsync();

        return Ok(cierres);
    }
}
