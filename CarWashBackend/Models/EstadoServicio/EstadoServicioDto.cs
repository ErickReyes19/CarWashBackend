public class EstadosServicioDTO
{
    public string id { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public bool? activo { get; set; }
}


public class EstadosServicioSummaryDTO
{
    public string Id { get; set; }
    public string Nombre { get; set; }
}
