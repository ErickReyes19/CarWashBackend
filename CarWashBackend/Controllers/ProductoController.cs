using CarWashBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly CarwashContext _context;

        public ProductoController(CarwashContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            try
            {
                var productos = await _context.Productos
                    .Select(p => new ProductoDto
                    {
                        id = p.id,
                        nombre = p.nombre,
                        descripcion = p.descripcion,
                        activo = p.activo,
                        precio = p.precio
                    })
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los productos: {ex.Message}");
            }
        }       
        [HttpGet("select")]
        public async Task<IActionResult> GetProductosSelect()
        {
            try
            {
                var productos = await _context.Productos
                    .Select(p => new ProductoDtoSelect
                    {
                        id = p.id,
                        nombre = p.nombre,
                        precio = p.precio
                    })
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los productos: {ex.Message}");
            }
        }


        // GET: api/Productos/active
        [HttpGet("active")]
        public async Task<IActionResult> GetProductosActivos()
        {
            try
            {
                var productosActivos = await _context.Productos
                    .Where(p => p.activo == true)  
                    .Select(p => new ProductoDto  
                    {
                        id = p.id,
                        nombre = p.nombre,
                        descripcion = p.descripcion,
                        activo = p.activo,
                        precio = p.precio
                    })
                    .ToListAsync();

                return Ok(productosActivos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los productos activos: {ex.Message}");
            }
        }

        // GET: api/Producto/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductoById(string id)
        {
            try
            {
                var producto = await _context.Productos
                    .Where(p => p.id == id)  
                    .Select(p => new ProductoDto 
                    {
                        id = p.id,
                        nombre = p.nombre,
                        descripcion = p.descripcion,
                        activo = p.activo,
                        precio = p.precio
                    })
                    .FirstOrDefaultAsync();

                if (producto == null)
                    return NotFound($"Producto con ID {id} no encontrado.");

                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el producto: {ex.Message}");
            }
        }


        // POST: api/Prodcuto
        [HttpPost]
        public async Task<IActionResult> CreateProducto([FromBody] ProductoCreateDto productoDto)
        {
            if (productoDto == null)
                return BadRequest("Datos inválidos.");

            try
            {
                // Convertir ProductoDto a la entidad Producto
                var productoCreate = new Producto
                {
                    id = Guid.NewGuid().ToString(),
                    nombre = productoDto.nombre,
                    descripcion = productoDto.descripcion,
                    activo = productoDto.activo,
                    precio = productoDto.precio
                };

                // Agregar el producto a la base de datos
                _context.Productos.Add(productoCreate);
                await _context.SaveChangesAsync();

                // Devolver la respuesta
                return CreatedAtAction(nameof(GetProductoById), new { id = productoCreate.id }, productoCreate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el producto: {ex.Message}");
            }
        }


        // PUT: api/Role/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(string id, [FromBody] ProductoDto productoDto)
        {
            if (id != productoDto.id)
                return BadRequest("El ID proporcionado no coincide.");

            try
            {
                var existingProducto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.id == id);

                if (existingProducto == null)
                    return NotFound($"Producto con ID {id} no encontrado.");

                existingProducto.nombre = productoDto.nombre;
                existingProducto.descripcion = productoDto.descripcion;
                existingProducto.activo = productoDto.activo;
                existingProducto.precio = productoDto.precio;

                _context.Productos.Update(existingProducto);
                await _context.SaveChangesAsync();

                return Ok(existingProducto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el producto: {ex.Message}");
            }
        }



    }
}
