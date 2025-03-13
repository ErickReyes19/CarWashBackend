using CarWashBackend.Models;

public class registro_servicio_detalle_producto
{
    public string RegistroServicioDetalleId { get; set; }
    public registro_servicio_detalle RegistroServicioDetalle { get; set; }

    public string ProductoId { get; set; }
    public Producto Producto { get; set; }

    public int Cantidad { get; set; }

    // Precio del producto en el momento de la transacción
    public decimal Precio { get; set; }

    // Total calculado para este producto en el detalle (Precio * Cantidad)
    public decimal Total { get; set; }
}
