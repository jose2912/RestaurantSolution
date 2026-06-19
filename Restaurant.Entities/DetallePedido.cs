namespace Restaurant.Entities
{
    public class DetallePedido
    {
        public int DetalleId { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public decimal TotalLinea => Cantidad * PrecioUnitario;
    }
}
