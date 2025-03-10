using CarWashBackend.Models;

public partial class registro_servicio
{
    public string id { get; set; }
    public string cliente_id { get; set; }
    public string usuario_id { get; set; }
    public DateTime fecha { get; set; }
    public string estado_servicio_id { get; set; }
    public string descripcion { get; set; }
    public decimal total { get; set; }

    // Nueva propiedad CierreId
    public string? CierreId { get; set; }  // Ahora es nullable

    // Nueva propiedad de navegación
    public virtual Cierre Cierre { get; set; }

    public virtual Cliente cliente { get; set; }
    public virtual EstadosServicio estado_servicio { get; set; }
    public virtual ICollection<pago> pagos { get; set; } = new List<pago>();
    public virtual ICollection<registro_servicio_vehiculo> registro_servicio_vehiculos { get; set; } = new List<registro_servicio_vehiculo>();
    public virtual ICollection<Empleado> empleados { get; set; } = new List<Empleado>();
}
