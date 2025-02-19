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





public class ClienteDtoClienteVista
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }
    public string Genero { get; set; }
    public bool? Activo { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<VehiculoDtoClienteVista> Vehiculos { get; set; } = new List<VehiculoDtoClienteVista>();
    public List<RegistroServicioDtoClienteVista> RegistroServicios { get; set; } = new List<RegistroServicioDtoClienteVista>();
}

public class VehiculoDtoClienteVista
{
    public string Id { get; set; }
    public string Placa { get; set; }
    public string Modelo { get; set; }
    public string Marca { get; set; }
    public string Color { get; set; }
    public bool? Activo { get; set; }
}

public class RegistroServicioDtoClienteVista
{
    public string Id { get; set; }
    public DateTime Fecha { get; set; }

    public EstadoServicioDtoClienteVista EstadoServicio { get; set; }

    public List<EmpleadoDtoClienteVista> Empleados { get; set; } = new List<EmpleadoDtoClienteVista>();

    public List<RegistroServicioVehiculoDtoClienteVista> RegistroServicioVehiculos { get; set; } = new List<RegistroServicioVehiculoDtoClienteVista>();
}

public class EstadoServicioDtoClienteVista
{
    public string Nombre { get; set; }
}

public class EmpleadoDtoClienteVista
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
}

public class RegistroServicioVehiculoDtoClienteVista
{
    public string Id { get; set; }

    public string VehiculoConcatenado { get; set; }

    public List<RegistroServicioDetalleDtoClienteVista> Detalles { get; set; } = new List<RegistroServicioDetalleDtoClienteVista>();
}

public class RegistroServicioDetalleDtoClienteVista
{
    public decimal Precio { get; set; }
    public ServicioDtoClienteVista Servicio { get; set; }
}

public class ServicioDtoClienteVista
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
}
