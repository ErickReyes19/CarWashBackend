using CarWashBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
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
        // Obtener la fecha y hora actual en Honduras
        DateTime ahoraHonduras = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _timeZoneHonduras);
        DateTime hoy = ahoraHonduras.Date; // Solo la fecha, sin la hora

        // Obtener pagos del día en Honduras
        var pagos = await _context.pagos
            .Where(p => TimeZoneInfo.ConvertTime(p.registro_servicio.fecha, _timeZoneHonduras).Date == hoy)
            .ToListAsync();

        if (!pagos.Any())
        {
            return BadRequest("No hay pagos registrados para realizar el cierre.");
        }

        // Agrupar pagos por método de pago
        var pagosAgrupados = pagos
            .GroupBy(p => p.metodo_pago)
            .Select(g => new
            {
                MetodoPago = g.Key,
                MontoTotal = g.Sum(p => p.monto)
            }).ToList();

        // Crear el cierre general
        var cierre = new Cierre
        {
            Total = pagosAgrupados.Sum(p => p.MontoTotal),
            Fecha = ahoraHonduras
        };

        _context.Cierres.Add(cierre);
        await _context.SaveChangesAsync(); // Guardamos para obtener el ID del cierre

        // Crear los detalles del cierre
        var detalles = pagosAgrupados.Select(p => new CierreDetalle
        {
            CierreId = cierre.Id,
            MetodoPago = p.MetodoPago,
            Monto = p.MontoTotal
        }).ToList();

        _context.CierreDetalles.AddRange(detalles);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Cierre registrado con éxito", cierre, detalles });
    }
}
