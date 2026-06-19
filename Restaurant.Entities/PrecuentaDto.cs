namespace Restaurant.Entities
{
    public class PrecuentaDto
    {
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Recargo { get; set; }
        public decimal Total { get; set; }
    }
}
