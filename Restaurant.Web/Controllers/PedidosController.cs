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
            if (!ModelState.IsValid)
            {
                return View(pedido); // vuelve a mostrar el formulario con errores
            }

            pedido.Estado = "Generado"; // estado inicial fijo
            _pedidoService.CrearPedido(pedido);

            TempData["SuccessMessage"] = "Pedido creado correctamente.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult CambiarEstado(int id, string estado)
        {
            var ok = _pedidoService.CambiarEstado(id, estado);
            if (!ok) return BadRequest();

            return RedirectToAction("Detalle", new { id });
        }

        // Acción Precuenta extendida
        //public IActionResult Precuenta(int id)
        //{
        //    // Validar que se haya enviado un id válido
        //    if (id <= 0)
        //    {
        //        TempData["ErrorMessage"] = "Debe seleccionar un pedido válido para generar la precuenta.";
        //        return RedirectToAction("Index");
        //    }

        //    var precuenta = _pedidoService.CalcularTotales(id);
        //    if (precuenta == null)
        //    {
        //        TempData["ErrorMessage"] = "No se pudo calcular la precuenta porque el pedido no existe.";
        //        return RedirectToAction("Index");
        //    }

        //    int nuevoDocId = _documentoService.GenerarDocumentoPago(id);
        //    if (nuevoDocId <= 0)
        //    {
        //        TempData["ErrorMessage"] = "No se pudo generar el documento porque el pedido no existe.";
        //        return RedirectToAction("Index");
        //    }

        //    TempData["SuccessMessage"] = $"Precuenta generada correctamente. Documento ID: {nuevoDocId}";
        //    return RedirectToAction("Index", "Documentos");
        //}
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
            var productos = _pedidoService.ObtenerProductos();
            ViewBag.PedidoId = pedidoId;
            ViewBag.Productos = productos;
            return View(new DetallePedido { PedidoId = pedidoId });
        }

        [HttpPost]
        public IActionResult AgregarDetalle(DetallePedido detalle)
        {
            if (ModelState.IsValid)
            {
                _pedidoService.AgregarDetalle(detalle);
                return RedirectToAction("Detalle", new { id = detalle.PedidoId });
            }
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
    }
}
