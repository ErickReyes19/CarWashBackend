public class CierreDetalle
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Llave foránea al cierre general
    public string CierreId { get; set; }
    public Cierre Cierre { get; set; }

    // Método de pago (por ejemplo: "efectivo", "tarjeta", "transferencia")
    public string MetodoPago { get; set; }

    // Monto total para este método en el cierre
    public decimal Monto { get; set; }
}
