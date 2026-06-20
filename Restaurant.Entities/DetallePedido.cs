using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Restaurant.Entities;
using System.ComponentModel.DataAnnotations;
namespace Restaurant.Entities
{
    public class DetallePedido
    {
        public int DetalleId { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto")]
        public int ProductoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        public decimal TotalLinea => Cantidad * PrecioUnitario;
        [ValidateNever] 
        public Producto Producto { get; set; }
    }
}
