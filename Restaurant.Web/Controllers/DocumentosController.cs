using Microsoft.AspNetCore.Mvc;
using Restaurant.Business;   
using Restaurant.Entities;   // entidad DocumentoPago

namespace Restaurant.Web.Controllers
{
    public class DocumentosController : Controller
    {
        private readonly DocumentoService _documentoService;

        public DocumentosController(DocumentoService documentoService)
        {
            _documentoService = documentoService;
        }

        public IActionResult Index()
        {
            IEnumerable<DocumentoPago> documentos = _documentoService.ListarDocumentos();
            return View(documentos);
        }

        // Generar documento desde un pedido
        [HttpPost]
        public IActionResult Generar(int pedidoId)
        {
            int nuevoId = _documentoService.GenerarDocumentoPago(pedidoId);
            if (nuevoId <= 0)
            {
                TempData["ErrorMessage"] = "No se pudo generar el documento.";
            }
            else
            {
                TempData["SuccessMessage"] = $"Documento generado correctamente (ID: {nuevoId}).";
            }
            return RedirectToAction("Index");
        }       
        [HttpPost]
        public IActionResult CambiarEstado(int id, string estado)
        {
            var ok = _documentoService.CambiarEstadoDocumento(id, estado);

            if (!ok)
            {
                TempData["ErrorMessage"] = "No se encontró el documento.";
            }
            else
            {
                var doc = _documentoService.ListarDocumentos().FirstOrDefault(d => d.DocumentoId == id);

                if (doc != null && doc.Estado == estado)
                {
                    TempData["SuccessMessage"] = $"El documento {id} ya estaba en estado {estado}.";
                }
                else
                {
                    TempData["SuccessMessage"] = $"Estado del documento {id} cambiado a {estado}.";
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Precuenta(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un pedido válido para generar la precuenta.";
                return RedirectToAction("Index", "Pedidos");
            }

            var precuenta = _documentoService.CalcularTotales(id);
            if (precuenta == null)
            {
                TempData["ErrorMessage"] = "El pedido no existe en la base de datos.";
                return RedirectToAction("Index", "Pedidos");
            }

            int nuevoDocId = _documentoService.GenerarDocumentoPago(id);
            if (nuevoDocId <= 0)
            {
                TempData["ErrorMessage"] = "No se pudo generar el documento.";
                return RedirectToAction("Index", "Pedidos");
            }

            TempData["SuccessMessage"] = $"Precuenta generada correctamente. Documento ID: {nuevoDocId}";
            return RedirectToAction("Index", "Documentos");
        }

        public IActionResult Descargar(int id)
        {
            var pdfBytes = _documentoService.ObtenerPdfPorId(id);
            if (pdfBytes == null) return NotFound();

            return File(pdfBytes, "application/pdf", $"Documento_{id}.pdf");
        }

    }
}
