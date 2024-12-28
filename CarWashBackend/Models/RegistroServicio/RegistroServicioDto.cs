public class RegistroServicioDTO
{
    public string id { get; set; }
    public string cliente_id { get; set; }
    public string vehiculo_id { get; set; }
    public string servicio_id { get; set; }
    public string usuario_id { get; set; }
    public string estado_id { get; set; }
    public string observaciones { get; set; }
    public DateTime? fecha { get; set; }
    public DateTime? created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public ClienteSummaryDTO cliente { get; set; }
    public VehiculoSummaryDTO vehiculo { get; set; }
    public ServicioSummaryDTO servicio { get; set; }
    public UsuarioSummaryDTO usuario { get; set; }
    public EstadosServicioSummaryDTO estado { get; set; }

    public List<PagoSummaryDTO> Pagos { get; set; }
}
