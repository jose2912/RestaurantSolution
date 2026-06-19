using Microsoft.AspNetCore.Mvc;
using Restaurant.Business;
using Restaurant.Entities;

namespace Restaurant.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosApiController : ControllerBase
    {
        private readonly PedidoService _pedidoService;

        public PedidosApiController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // GET api/pedidos/5
        [HttpGet("{id}")]
        public ActionResult<Pedido> ObtenerPedido(int id)
        {
            var pedido = _pedidoService.ObtenerPedidoPorId(id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        // POST api/pedidos
        [HttpPost]
        public ActionResult<int> CrearPedido([FromBody] Pedido pedido)
        {
            var id = _pedidoService.CrearPedido(pedido);
            return Ok(id);
        }

        // POST api/pedidos/detalle
        [HttpPost("detalle")]
        public IActionResult AgregarDetalle([FromBody] DetallePedido detalle)
        {
            _pedidoService.AgregarDetalle(detalle);
            return Ok();
        }

        // GET api/pedidos/precuenta/5
        [HttpGet("precuenta/{id}")]
        public ActionResult<Precuenta> CalcularTotales(int id)
        {
            var precuenta = _pedidoService.CalcularTotales(id);
            if (precuenta == null) return NotFound();
            return Ok(precuenta);
        }

        // PUT api/pedidos/estado/5?estado=Facturado
        [HttpPut("estado/{id}")]
        public IActionResult CambiarEstado(int id, [FromQuery] string estado)
        {
            var ok = _pedidoService.CambiarEstadoPedido(id, estado);
            if (!ok) return BadRequest();
            return Ok();
        }
    }
}
