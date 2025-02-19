public class UsuarioDTO
{
    public string id { get; set; }
    public string usuario { get; set; }
    public string empleadoNombre { get; set; }  
    public string roleNombre { get; set; }      
    public bool? activo { get; set; }      
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}
public class UsuarioCreateDTO
{
    public string id { get; set; }
    public string usuario { get; set; }
    public string empleado_id { get; set; }  
    public string role_id { get; set; }      
    public bool? activo { get; set; }      
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}


public class UsuarioSummaryDTO
{
    public string Id { get; set; }
    public string Usuario { get; set; }
}
