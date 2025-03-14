namespace CarWashBackend.Models
{
    public class ProductoDto
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public bool? activo { get; set; }
        public decimal precio { get; set; }
    }       
    public class ProductoDtoSelect
    {
        public string id { get; set; }
        public string nombre { get; set; }
        public decimal precio { get; set; }
    }    
    public class ProductoCreateDto
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public bool? activo { get; set; }
        public decimal precio { get; set; }
    }

    public class ProductoUsageDto
    {
        public string ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }

    }
    public class ProductoUsageViewDto
    {
        public string ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public string Nombre { get; set; }

    }

}
