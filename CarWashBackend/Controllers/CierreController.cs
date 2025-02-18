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
        // Crear el cierre para el día
        var cierre = new Cierre
        {
            Fecha = DateTime.Now,
            Total = 0
        };
        _context.Cierres.Add(cierre);
        await _context.SaveChangesAsync();

        // Obtener los registros de servicio del día
        var registros = await _context.registro_servicios
                                      .Where(rs => rs.fecha.Date == DateTime.Today)
                                      .Include(rs => rs.pagos)  // Incluir pagos
                                      .Include(rs => rs.registro_servicio_vehiculos)
                                          .ThenInclude(rs_veh => rs_veh.registro_servicio_detalles) // Incluir detalles de servicios
                                      .ToListAsync();

        decimal totalCierre = 0;

        foreach (var registro in registros)
        {
            // Crear un CierreDetalle por cada pago
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
        }
        // Actualizar el total del Cierre
        cierre.Total = totalCierre;
        await _context.SaveChangesAsync();

        return Ok("Cierre ");
    }
}
