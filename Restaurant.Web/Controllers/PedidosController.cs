using Microsoft.AspNetCore.Mvc;
using Restaurant.Business;
using Restaurant.Entities;

namespace Restaurant.Web.Controllers
{
    public class PedidosController : Controller
    {
        private readonly PedidoService _pedidoService;
        private readonly DocumentoService _documentoService;

        public PedidosController(PedidoService pedidoService, DocumentoService documentoService)
        {
            _pedidoService = pedidoService;
            _documentoService = documentoService;
        }

        [HttpGet]
        public IActionResult Form()
        {
            return View();
        }

        public IActionResult Index()
        {
            var pedidos = _pedidoService.ObtenerTodos();
            return View(pedidos);
        }

        public IActionResult Detalle(int id)
        {
            var pedido = _pedidoService.ObtenerPedidoPorId(id);
            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        [HttpPost]
        public IActionResult Crear(Pedido pedido)
        {
            var errors = ModelState.Values
    .SelectMany(v => v.Errors)
    .Select(e => e.ErrorMessage)
    .ToList();

            if (!ModelState.IsValid)
            {
                return View("Form", pedido);
            }
            if (pedido.FechaPedido != null)
            {

                pedido.Estado = "Generado";
                _pedidoService.CrearPedido(pedido);

                TempData["SuccessMessage"] = "Pedido creado correctamente.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Debes seleccionar la fecha.";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult CambiarEstado(int id, string estado)
        {
            var ok = _pedidoService.CambiarEstado(id, estado);
            if (!ok) return BadRequest();

            return RedirectToAction("Detalle", new { id });
        }

      
        public IActionResult Precuenta(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un pedido válido para generar la precuenta.";
                return RedirectToAction("Index");
            }

            var pedido = _pedidoService.ObtenerPedidoPorId(id);
            if (pedido == null)
            {
                TempData["ErrorMessage"] = "El pedido no existe en la base de datos.";
                return RedirectToAction("Index");
            }

            var precuenta = _pedidoService.CalcularTotales(id);
            if (precuenta == null)
            {
                TempData["ErrorMessage"] = "No se pudo calcular la precuenta.";
                return RedirectToAction("Index");
            }

            int nuevoDocId = _documentoService.GenerarDocumentoPago(id);
            if (nuevoDocId <= 0)
            {
                TempData["ErrorMessage"] = "No se pudo generar el documento.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = $"Precuenta generada correctamente. Documento ID: {nuevoDocId}";
            return RedirectToAction("Index", "Documentos");
        }
        [HttpGet]
        public IActionResult AgregarDetalle(int pedidoId)
        {
            // Cargar productos para el combo
            ViewBag.Productos = _pedidoService.ObtenerProductos();

            // Inicializar el modelo con el PedidoId
            var detalle = new DetallePedido { PedidoId = pedidoId };

            return View(detalle);
        }

        [HttpPost]
        public IActionResult AgregarDetalle(DetallePedido detalle)
        {
            if (ModelState.IsValid )
            {
                // Guardar el detalle en la base
                _pedidoService.AgregarDetalle(detalle);

                // Redirigir al detalle del pedido
                return RedirectToAction("Detalle", new { id = detalle.PedidoId });
            }

            // Si falla la validación, mostrar errores y recargar productos
            var errores = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            TempData["ErrorMessage"] = string.Join(" | ", errores);

            ViewBag.Productos = _pedidoService.ObtenerProductos();
            return View(detalle);
        }


        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            var ok = _pedidoService.EliminarPedido(id);
            if (!ok)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el pedido porque tiene documentos asociados.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Pedido eliminado correctamente.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [HttpPost]
        public IActionResult EliminarDetalle(int detalleId)
        {
            var ok = _pedidoService.EliminarDetalle(detalleId);
            if (!ok) return Json(new { success = false, message = "No se pudo eliminar el detalle." });

            return Json(new { success = true, message = "Detalle eliminado correctamente." });
        }


    }
}
