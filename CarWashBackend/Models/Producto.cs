using CarWashBackend.Models;

public class Producto
{
    public string id { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public decimal precio { get; set; }
    public bool? activo { get; set; }
    public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    public ICollection<registro_servicio_detalle_producto> RegistroServicioDetalleProductos { get; set; }
}

public class ProductoDtoEdit
{
    public string ProductoId { get; set; }
    public int Cantidad { get; set; }
}
