using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Asegura que solo usuarios autenticados puedan acceder
    public class RegistroServicioController : ControllerBase
    {
        private readonly CarwashContext _context;

        public RegistroServicioController(CarwashContext context)
        {
            _context = context;
        }

        // GET: api/RegistroServicio
        [HttpGet]
        public async Task<IActionResult> GetAllRegistroServicios()
        {
            var registros = await _context.RegistroServicios
                .Include(r => r.cliente)
                .Include(r => r.vehiculo)
                .Include(r => r.servicio)
                .Include(r => r.usuario)
                .Include(r => r.estado)
                .Include(r => r.Pagos)
                .ToListAsync();

            if (registros == null || !registros.Any())
                return NotFound("No se encontraron registros de servicios.");

            var registrosDto = registros.Select(r => new RegistroServicioDTO
            {
                id = r.id,
                cliente_id = r.cliente_id,
                vehiculo_id = r.vehiculo_id,
                servicio_id = r.servicio_id,
                usuario_id = r.usuario_id,
                estado_id = r.estado_id,
                observaciones = r.observaciones,
                fecha = r.fecha,
                created_at = r.created_at,
                updated_at = r.updated_at,
                cliente = new ClienteSummaryDTO
                {
                    Id = r.cliente.id,
                    Nombre = r.cliente.nombre
                },
                vehiculo = new VehiculoSummaryDTO
                {
                    Id = r.vehiculo.id,
                    Marca = r.vehiculo.marca
                },
                servicio = new ServicioSummaryDTO
                {
                    Id = r.servicio.id,
                    Nombre = r.servicio.nombre
                },
                usuario = new UsuarioSummaryDTO
                {
                    Id = r.usuario.id,
                    Usuario = r.usuario.usuario1
                },
                estado = new EstadosServicioSummaryDTO
                {
                    Id = r.estado.id,
                    Nombre = r.estado.nombre
                },
                Pagos = r.Pagos.Select(p => new PagoSummaryDTO
                {
                    id = p.id,
                    monto = p.monto,
                    metodo_pago = p.metodo_pago,
                    fecha = p.fecha
                }).ToList() // Mapea los pagos relacionados
            }).ToList();


            return Ok(registrosDto);
        }



        // GET: api/RegistroServicio/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegistroServicioById(string id)
        {
            var registro = await _context.RegistroServicios
                .Include(r => r.cliente)
                .Include(r => r.vehiculo)
                .Include(r => r.servicio)
                .Include(r => r.usuario)
                .Include(r => r.estado)
                .Include(r => r.Pagos)
                .FirstOrDefaultAsync(r => r.id == id);

            if (registro == null)
                return NotFound($"No se encontró un registro de servicio con el ID {id}.");

            var registroDto = new RegistroServicioDTO
            {
                id = registro.id,
                cliente_id = registro.cliente_id,
                vehiculo_id = registro.vehiculo_id,
                servicio_id = registro.servicio_id,
                usuario_id = registro.usuario_id,
                estado_id = registro.estado_id,
                observaciones = registro.observaciones,
                fecha = registro.fecha,
                created_at = registro.created_at,
                updated_at = registro.updated_at,
                cliente = new ClienteSummaryDTO
                {
                    Id = registro.cliente.id,
                    Nombre = registro.cliente.nombre
                },
                vehiculo = new VehiculoSummaryDTO
                {
                    Id = registro.vehiculo.id,
                    Marca = registro.vehiculo.marca
                },
                servicio = new ServicioSummaryDTO
                {
                    Id = registro.servicio.id,
                    Nombre = registro.servicio.nombre
                },
                usuario = new UsuarioSummaryDTO
                {
                    Id = registro.usuario.id,
                    Usuario = registro.usuario.usuario1
                },
                estado = new EstadosServicioSummaryDTO
                {
                    Id = registro.estado.id,
                    Nombre = registro.estado.nombre
                },
                Pagos = registro.Pagos.Select(p => new PagoSummaryDTO
                {
                    id = p.id,
                    monto = p.monto,
                    metodo_pago = p.metodo_pago,
                    fecha = p.fecha
                }).ToList() // Mapea los pagos relacionados

            };

            return Ok(registroDto);
        }

        // POST: api/RegistroServicio
        [HttpPost]
        public async Task<IActionResult> CreateRegistroServicio(RegistroServicio registro)
        {

            Console.WriteLine($"Cliente ID: {registro.cliente_id} (Length: {registro.cliente_id.Length})");



            if (registro == null)
                return BadRequest("Los datos enviados son inválidos.");

            registro.id = Guid.NewGuid().ToString();
            registro.created_at = DateTime.UtcNow;
            registro.updated_at = DateTime.UtcNow;

            _context.RegistroServicios.Add(registro);
            await _context.SaveChangesAsync();

            var registroDto = new RegistroServicioDTO
            {
                id = registro.id,
                cliente_id = registro.cliente_id,
                vehiculo_id = registro.vehiculo_id,
                servicio_id = registro.servicio_id,
                usuario_id = registro.usuario_id,
                estado_id = registro.estado_id,
                observaciones = registro.observaciones,
                fecha = registro.fecha,
                created_at = registro.created_at,
                updated_at = registro.updated_at
            };

            return CreatedAtAction(nameof(GetRegistroServicioById), new { id = registro.id }, registroDto);
        }

        // PUT: api/RegistroServicio/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegistroServicio(string id, RegistroServicio registro)
        {
            if (id != registro.id)
                return BadRequest("El ID no coincide con el del registro de servicio enviado.");

            var existingRegistro = await _context.RegistroServicios.FindAsync(id);

            if (existingRegistro == null)
                return NotFound($"No se encontró un registro de servicio con el ID {id}.");

            existingRegistro.cliente_id = registro.cliente_id;
            existingRegistro.vehiculo_id = registro.vehiculo_id;
            existingRegistro.servicio_id = registro.servicio_id;
            existingRegistro.usuario_id = registro.usuario_id;
            existingRegistro.estado_id = registro.estado_id;
            existingRegistro.observaciones = registro.observaciones;
            existingRegistro.fecha = registro.fecha;
            existingRegistro.updated_at = DateTime.UtcNow;

            _context.RegistroServicios.Update(existingRegistro);
            await _context.SaveChangesAsync();

            var registroDto = new RegistroServicioDTO
            {
                id = existingRegistro.id,
                cliente_id = existingRegistro.cliente_id,
                vehiculo_id = existingRegistro.vehiculo_id,
                servicio_id = existingRegistro.servicio_id,
                usuario_id = existingRegistro.usuario_id,
                estado_id = existingRegistro.estado_id,
                observaciones = existingRegistro.observaciones,
                fecha = existingRegistro.fecha,
                created_at = existingRegistro.created_at,
                updated_at = existingRegistro.updated_at
            };

            return Ok(registroDto);
        }
    }
}
