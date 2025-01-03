﻿public class RoleDTO
{
    public string id { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public bool? activo { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public List<PermisoDTORol> permisos { get; set; } // Cambiado de permisosIds a permisos
}
public class RoleCreateDTO
{
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public bool? activo { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public List<string> permisosIds { get; set; }
}
