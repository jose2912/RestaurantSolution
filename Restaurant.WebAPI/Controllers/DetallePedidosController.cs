using Microsoft.AspNetCore.Mvc;
using Restaurant.Business;
using Restaurant.Entities;

namespace Restaurant.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetallePedidosController : ControllerBase
    {
        private readonly DetallePedidoService _detalleService;

        public DetallePedidosController(DetallePedidoService detalleService)
        {
            _detalleService = detalleService;
        }

        // POST: api/detallepedidos/agregar
        [HttpPost("agregar")]
        public IActionResult AgregarDetalle([FromBody] DetallePedido detalle)
        {
            var resultado = _detalleService.AgregarDetallePedido(detalle);

            if (!resultado)
                return BadRequest("No se pudo agregar el detalle al pedido.");

            return Ok(new { Message = "Detalle agregado correctamente." });
        }

        // GET: api/detallepedidos/{pedidoId}
        [HttpGet("{pedidoId}")]
        public IActionResult ObtenerDetalles(int pedidoId)
        {
            var detalles = _detalleService.ObtenerDetallesPorPedido(pedidoId);
            if (detalles == null || !detalles.Any())
                return NotFound("No se encontraron detalles para este pedido.");

            return Ok(detalles);
        }

        // DELETE: api/detallepedidos/{detalleId}
        [HttpDelete("{detalleId}")]
        public IActionResult EliminarDetalle(int detalleId)
        {
            var resultado = _detalleService.EliminarDetallePedido(detalleId);
            if (!resultado)
                return BadRequest("No se pudo eliminar el detalle.");

            return Ok(new { Message = "Detalle eliminado correctamente." });
        }


    }
}
