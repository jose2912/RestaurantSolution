using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.Entities
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        //public int ClienteId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaPedido { get; set; }
        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; } = "Generado";
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Recargo { get; set; }
        public decimal Total { get; set; }
        public List<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
    }
}
