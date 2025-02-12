public class ServicioDto
{
    public string ServicioId { get; set; }
    public decimal Precio { get; set; }
}

public class RegistroServicioVehiculoDto
{
    public string VehiculoId { get; set; }
    public List<ServicioDto> Servicios { get; set; }
}

public class RegistroServicioMultipleDto
{
    public string ClienteId { get; set; }
    public string UsuarioId { get; set; }
    public string EstadoServicioId { get; set; }
    public List<RegistroServicioVehiculoDto> Vehiculos { get; set; }
    public List<string> Empleados { get; set; }
}


public class EmpleadoIdDto
{
    public string EmpleadoId { get; set; }
}


public class RegistroServicioSummaryDto
{
    public string Id { get; set; }
    public string ClienteNombre { get; set; }
    public string ClienteCorreo { get; set; }
    public string UsuarioNombre { get; set; }
    public string EstadoServicioNombre { get; set; }
    public DateTime Fecha { get; set; }
}

public class ClienteDto
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }
}

public class UsuarioDto
{
    public string Id { get; set; }
    //public string UsuarioNombre { get; set; }
    // Puedes agregar otros campos relevantes
}

public class EstadosServicioDto
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
}

public class VehiculoDto
{
    public string Id { get; set; }
    public string Placa { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public string Color { get; set; }
}

public class ServicioDetailDto
{
    public string Id { get; set; }  // Id del registro detalle, si se requiere
    public string ServicioNombre { get; set; }
    public decimal Precio { get; set; }
}

public class RegistroServicioVehiculoDetailDto
{
    public string Id { get; set; } // Id del registro_servicio_vehiculo
    public VehiculoDto Vehiculo { get; set; }
    public List<ServicioDetailDto> Servicios { get; set; }
}

public class RegistroServicioDetailDto
{
    public string Id { get; set; }
    public ClienteDto Cliente { get; set; }
    public UsuarioDto Usuario { get; set; }
    public EstadosServicioDto EstadoServicio { get; set; }
    public DateTime Fecha { get; set; }
    public List<RegistroServicioVehiculoDetailDto> Vehiculos { get; set; }
    public List<EmpleadoDto> Empleados { get; set; } // Nueva propiedad para empleados
}


public class EmpleadoDto
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Correo { get; set; }
}



public class RegistroServicioMultipleUpdateDto : RegistroServicioMultipleDto
{
    public string RegistroServicioId { get; set; }
}
