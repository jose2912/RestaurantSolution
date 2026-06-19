using Microsoft.AspNetCore.Mvc;
using Restaurant.Business;
using Restaurant.Entities;

namespace Restaurant.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : Controller
    {
        private readonly PedidoService _pedidoService;

        public PedidosController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // GET: api/pedidos/{id}
        [HttpGet("{id}")]
        public IActionResult GetPedido(int id)
        {
            var pedido = _pedidoService.ObtenerPedidoPorId(id);
            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

        // POST: api/pedidos/{id}/calcular
        [HttpPost("{id}/calcular")]
        public IActionResult CalcularTotales(int id)
        {
            var resultado = _pedidoService.CalcularTotales(id);
            if (resultado == null)
                return BadRequest("No se pudo calcular los totales del pedido.");

            return Ok(new { Message = "Totales calculados correctamente." });
        }

        // POST: api/pedidos
        [HttpPost]
        public IActionResult CrearPedido([FromBody] Pedido pedido)
        {
            var nuevoPedidoId = _pedidoService.CrearPedido(pedido);
            return CreatedAtAction(nameof(GetPedido), new { id = nuevoPedidoId }, pedido);
        }

        // PUT: api/pedidos/{id}/estado
        [HttpPut("{id}/estado")]
        public IActionResult CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            var resultado = _pedidoService.CambiarEstado(id, nuevoEstado);
            if (!resultado)
                return BadRequest("No se pudo cambiar el estado del pedido.");

            return Ok(new { Message = $"Estado cambiado a {nuevoEstado}" });
        }
        [HttpGet("{id}/precuenta")]
        public IActionResult Precuenta(int id)
        {
            var precuenta = _pedidoService.CalcularTotales(id);
            if (precuenta == null)
                return NotFound();

            return View(precuenta);
        }

    }
}
