public class ServicioDTO
{
    public string id { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public decimal precio { get; set; }
    public bool? activo { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}

public class ServicioSummaryDTO
{
    public string Id { get; set; }
    public string Nombre { get; set; }
}
