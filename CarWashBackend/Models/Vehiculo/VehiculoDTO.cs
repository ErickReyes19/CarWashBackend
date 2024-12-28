public class VehiculoDTO
{
    public string id { get; set; }
    public string placa { get; set; }
    public string modelo { get; set; }
    public string marca { get; set; }
    public string color { get; set; }
    public bool? activo { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }

    // Nombre del cliente, no la entidad completa
    public string ClienteNombre { get; set; }
}

public class VehiculoSummaryDTO
{
    public string Id { get; set; }
    public string Marca { get; set; }
}

