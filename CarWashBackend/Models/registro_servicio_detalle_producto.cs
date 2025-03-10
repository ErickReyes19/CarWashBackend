using CarWashBackend.Models;

public class registro_servicio_detalle_producto
{
    public string RegistroServicioDetalleId { get; set; }
    public registro_servicio_detalle RegistroServicioDetalle { get; set; }

    public string ProductoId { get; set; }
    public Producto Producto { get; set; }

    public int Cantidad { get; set; }
}
