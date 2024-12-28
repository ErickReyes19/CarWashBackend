public class ClienteDTO
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }
    public string Genero { get; set; }
    public bool? Activo { get; set; }
}

public class ClienteSummaryDTO
{
    public string Id { get; set; }
    public string Nombre { get; set; }
}

