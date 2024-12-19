public class UsuarioDTO
{
    public string id { get; set; }
    public string usuario { get; set; }
    public string empleadoNombre { get; set; }  // Nombre del empleado asociado
    public string roleNombre { get; set; }      // Nombre del rol asociado
    public bool? activo { get; set; }      // Nombre del rol asociado
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
}
