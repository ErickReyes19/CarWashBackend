using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
[Authorize] // Esto asegura que solo usuarios autenticados puedan acceder
public class ReporteController : ControllerBase
{
    private readonly CarwashContext _context;

    public ReporteController(CarwashContext context)
    {
        _context = context;
    }

    // Ganancias del día
    [HttpGet("ganancias-dia")]
    public async Task<ActionResult> GetGananciasDia()
    {
        var today = DateTime.UtcNow.AddHours(-6); // Ajuste a la zona horaria de Honduras (UTC -6)
        var totalGanancias = await _context.registro_servicios
            .Where(r => r.fecha.Date == today.Date)
            .SumAsync(r => r.total);

        return Ok(new
        {
            message = "Ganancias del día",
            total = totalGanancias
        });
    }

    // Ganancias por rango de fecha
    [HttpGet("ganancias-rango")]
    public async Task<ActionResult> GetGananciasRango([FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
    {
        // Convertir a DateOnly para ignorar la hora
        var fechaInicio = fechaDesde.HasValue ? DateOnly.FromDateTime(fechaDesde.Value) : DateOnly.FromDateTime(DateTime.Now);
        var fechaFin = fechaHasta.HasValue ? DateOnly.FromDateTime(fechaHasta.Value) : DateOnly.FromDateTime(DateTime.Now);

        var totalGanancias = await _context.registro_servicios
            .Where(r => DateOnly.FromDateTime(r.fecha) >= fechaInicio && DateOnly.FromDateTime(r.fecha) <= fechaFin)
            .SumAsync(r => r.total);

        return Ok(new
        {
            message = "Ganancias por rango de fecha",
            total = totalGanancias
        });
    }


    //// Ganancias por tipo de servicio
    //[HttpGet("ganancias-tipo-servicio")]
    //public async Task<ActionResult> GetGananciasPorTipoDeServicio([FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
    //{
    //    // Si no se pasa fechaDesde o fechaHasta, se usan las fechas del día actual
    //    fechaDesde ??= DateTime.UtcNow.AddHours(-6).Date;
    //    fechaHasta ??= DateTime.UtcNow.AddHours(-6).Date;

    //    var gananciasPorTipo = await _context.registro_servicios
    //        .Where(r => r.fecha >= fechaDesde && r.fecha <= fechaHasta)
    //        .GroupBy(r => r.tipo_servicio) // Asumiendo que tienes un campo "tipo_servicio" en tu modelo
    //        .Select(g => new
    //        {
    //            tipo_servicio = g.Key,
    //            total = g.Sum(r => r.total)
    //        })
    //        .ToListAsync();

    //    return Ok(new
    //    {
    //        message = "Ganancias por tipo de servicio",
    //        data = gananciasPorTipo
    //    });
    //}

    // Número de servicios realizados por día
    [HttpGet("numero-servicios-dia")]
    public async Task<ActionResult> GetNumeroServiciosPorDia([FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
    {
        try
        {
            // Si no se pasan los parámetros, se usan las fechas actuales ajustadas a UTC-6
            var fechaDesdeSolo = (fechaDesde ?? DateTime.UtcNow.AddHours(-6)).Date;
            // Ajustar fechaHasta para incluir todo el día (hasta las 23:59:59.999)
            var fechaHastaSolo = ((fechaHasta ?? DateTime.UtcNow.AddHours(-6)).Date)
                                  .AddDays(1)
                                  .AddTicks(-1);

            // Consulta para obtener la lista de servicios realizados, su cantidad y las ganancias
            var serviciosConDetalles = await _context.registro_servicios
                .Where(r => r.fecha >= fechaDesdeSolo && r.fecha <= fechaHastaSolo)
                .Join(
                    _context.registro_servicio_detalles, // Unir con los detalles del servicio
                    r => r.id, // Relacionamos por el ID del registro de servicio
                    rsd => rsd.registro_servicio_vehiculo_id, // Relacionamos por el ID del vehículo en el detalle
                    (r, rsd) => new
                    {
                        r.fecha,
                        rsd.servicio_id,
                        rsd.precio
                    })
                .Join(
                    _context.Servicios, // Unir con la tabla de Servicios para obtener el nombre
                    rsd => rsd.servicio_id, // Relacionamos por servicio_id
                    s => s.id, // Relacionamos por id del servicio
                    (rsd, s) => new
                    {
                        nombreServicio = s.nombre, // Obtener nombre del servicio
                        precio = rsd.precio // Obtener el precio del detalle
                    })
                .GroupBy(s => s.nombreServicio) // Agrupar por nombre de servicio
                .Select(g => new
                {
                    nombreServicio = g.Key,
                    cantidad = g.Count(), // Contamos las veces que se ha realizado el servicio
                    ganancias = g.Sum(x => x.precio) // Sumamos las ganancias por servicio
                })
                .ToListAsync();

            return Ok(new
            {
                message = "Número de servicios realizados con ganancias",
                data = serviciosConDetalles
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al obtener el número de servicios: {ex.Message}" });
        }
    }




    // Promedio de ganancias diarias
    [HttpGet("promedio-ganancias-diarias")]
    public async Task<ActionResult> GetPromedioGananciasDiarias([FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
    {
        // Si no se pasa fechaDesde o fechaHasta, se usan las fechas del día actual ajustado a UTC-6
        var fechaDesdeSolo = (fechaDesde ?? DateTime.UtcNow.AddHours(-6)).Date;
        // Ajustar fechaHasta para incluir todo el día (hasta las 23:59:59.999)
        var fechaHastaSolo = ((fechaHasta ?? DateTime.UtcNow.AddHours(-6)).Date).AddDays(1).AddTicks(-1);

        // Calcular el total de ganancias en el rango de fechas
        var totalGanancias = await _context.registro_servicios
            .Where(r => r.fecha >= fechaDesdeSolo && r.fecha <= fechaHastaSolo)
            .SumAsync(r => r.total);

        // Calcular el número de días distintos con registros en el rango
        var dias = await _context.registro_servicios
            .Where(r => r.fecha >= fechaDesdeSolo && r.fecha <= fechaHastaSolo)
            .Select(r => r.fecha.Date)
            .Distinct()
            .CountAsync();

        var promedio = dias > 0 ? totalGanancias / dias : 0;

        return Ok(new
        {
            message = "Promedio de ganancias diarias",
            fechaDesde = fechaDesdeSolo.ToString("yyyy-MM-dd"),
            fechaHasta = fechaHastaSolo.Date.ToString("yyyy-MM-dd"),
            totalGanancias,
            promedio
        });
    }



    // Ganancias por cliente
    [HttpGet("ganancias-por-cliente")]
    public async Task<ActionResult> GetGananciasPorCliente([FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
    {
        try
        {
            // Si no se pasa fechaDesde o fechaHasta, se usan las fechas del día actual (ajustado a UTC-6)
            var fechaDesdeSolo = (fechaDesde ?? DateTime.UtcNow.AddHours(-6)).Date;
            // Ajuste para incluir todo el día de fechaHasta: hasta las 23:59:59.999
            var fechaHastaSolo = ((fechaHasta ?? DateTime.UtcNow.AddHours(-6)).Date).AddDays(1).AddTicks(-1);

            // Consultar todos los clientes y, para cada uno, sumar los totales de registros en el rango de fechas
            var gananciasPorCliente = await _context.Clientes
                .Select(c => new
                {
                    cliente_id = c.id,
                    nombre = c.nombre,
                    total = _context.registro_servicios
                              .Where(r => r.cliente_id == c.id &&
                                          r.fecha >= fechaDesdeSolo &&
                                          r.fecha <= fechaHastaSolo)
                              .Sum(r => (decimal?)r.total) ?? 0
                })
                .ToListAsync();

            return Ok(new
            {
                message = "Ganancias por cliente",
                data = gananciasPorCliente
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al obtener las ganancias por cliente: {ex.Message}" });
        }
    }




    [HttpGet("ganancias_rango")]
    public async Task<IActionResult> ObtenerGananciasPorRango([FromQuery] DateTime fechaDesde, [FromQuery] DateTime fechaHasta)
    {
        try
        {
            // Convertir las fechas a solo fecha (sin hora) para la comparación
            var fechaDesdeSolo = fechaDesde.Date;
            // Ajustar fechaHasta para incluir todo el día (hasta las 23:59:59.9999999)
            var fechaHastaSolo = fechaHasta.Date.AddDays(1).AddTicks(-1);

            // Obtener los registros agrupados por día que tengan datos
            var registros = await _context.registro_servicios
                .Where(r => r.fecha >= fechaDesdeSolo && r.fecha <= fechaHastaSolo)
                .GroupBy(r => r.fecha.Date)
                .Select(g => new
                {
                    fecha = g.Key,
                    ganancias = g.Sum(r => r.total)
                })
                .ToListAsync();

            // Generar la lista de fechas en el rango y asignar 0 si no hay registro para ese día
            var resultado = new List<object>();
            for (var d = fechaDesdeSolo; d <= fechaHastaSolo.Date; d = d.AddDays(1))
            {
                var registro = registros.FirstOrDefault(r => r.fecha == d);
                resultado.Add(new
                {
                    fecha = d.ToString("yyyy-MM-dd"),
                    ganancias = registro != null ? registro.ganancias : 0
                });
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al obtener las ganancias: {ex.Message}" });
        }
    }




}
