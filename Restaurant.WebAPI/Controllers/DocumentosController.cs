using Microsoft.AspNetCore.Mvc;
using Restaurant.Business;
using Restaurant.Entities;

namespace Restaurant.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly DocumentoService _documentoService;

        public DocumentosController(DocumentoService documentoService)
        {
            _documentoService = documentoService;
        }

        // POST: api/documentos/generar/{pedidoId}
        [HttpPost("generar/{pedidoId}")]
        public IActionResult GenerarDocumento(int pedidoId)
        {
            var nuevoDocumentoId = _documentoService.GenerarDocumentoPago(pedidoId);

            if (nuevoDocumentoId <= 0)
                return BadRequest("No se pudo generar el documento de pago.");

            return Ok(new { DocumentoId = nuevoDocumentoId, Message = "Documento generado correctamente." });
        }
    }
}
