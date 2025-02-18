using CarWashBackend.Models;

public class Cierre
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }

    // Relación con el detalle de cierre
    public ICollection<CierreDetalle> CierreDetalles { get; set; } = new List<CierreDetalle>();

    // Relación con registro_servicio
    public ICollection<registro_servicio> RegistroServicios { get; set; } = new List<registro_servicio>();
}
