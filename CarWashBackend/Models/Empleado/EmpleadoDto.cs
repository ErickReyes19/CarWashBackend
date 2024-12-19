public class EmpleadoDTO
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public int? Edad { get; set; }
    public string Genero { get; set; }
    public bool? Activo { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Solo incluir el nombre del usuario
    public string UsuarioNombre { get; set; }
}
