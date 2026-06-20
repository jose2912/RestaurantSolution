using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Entities
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        //public int ClienteId { get; set; }
        [Required(ErrorMessage = "Debe seleccionar la fecha del pedido.")]
        [DataType(DataType.Date)]
        public DateTime? FechaPedido { get; set; }   
        public string Estado { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Recargo { get; set; }
        public decimal Total { get; set; }
        public List<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
    }
}
