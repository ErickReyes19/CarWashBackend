
public class PagoDTO
{
    public string id { get; set; }
    public string registro_servicio_id { get; set; }
    public decimal monto { get; set; }
    public string metodo_pago { get; set; }
    public DateTime? fecha { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}

public class PagoSummaryDTO
{
    public string id { get; set; }
    public decimal monto { get; set; }
    public string metodo_pago { get; set; }
    public DateTime? fecha { get; set; }
}








