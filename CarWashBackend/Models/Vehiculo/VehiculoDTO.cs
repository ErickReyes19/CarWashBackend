using CarWashBackend.Models;

public class VehiculoDTO
{
    public string id { get; set; }
    public string placa { get; set; }
    public string modelo { get; set; }
    public string marca { get; set; }
    public string color { get; set; }
    public bool activo { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }

    // Cambia esta propiedad a una lista de ClienteSummaryDTO
    public List<ClienteSummaryDTO> clientes { get; set; }
}


public class VehiculoClienteDTO
{
    public string id { get; set; }
    public string placa { get; set; }
    public string modelo { get; set; }
    public string marca { get; set; }
    public string color { get; set; }
    public bool? activo { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }

    // Lista de clientes asociados con el vehículo
    public List<ClienteSummaryDTO> Clientes { get; set; } // Cambié el tipo a List<ClienteDTO>
}

public class VehiculoSummaryDTO
{
    public string Id { get; set; }
    public string Marca { get; set; }
}

