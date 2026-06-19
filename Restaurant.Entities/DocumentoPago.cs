using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Entities
{
    public class DocumentoPago
    {
        public int DocumentoId { get; set; }
        public int PedidoId { get; set; }
        public DateTime FechaDocumento { get; set; }
        public string Estado { get; set; } = "Generado";
        public decimal Total { get; set; }
    }
}
