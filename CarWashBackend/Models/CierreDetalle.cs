public class CierreDetalle
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string CierreId { get; set; }
    public Cierre Cierre { get; set; }

    public string MetodoPago { get; set; }

    public decimal Monto { get; set; }
}
