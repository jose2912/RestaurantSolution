using Restaurant.Data;
using Restaurant.Entities;

namespace Restaurant.Business
{
    public class DocumentoService
    {
        private readonly DocumentoRepository _documentoRepository;

        public DocumentoService(DocumentoRepository documentoRepository)
        {
            _documentoRepository = documentoRepository;
        }

        public int GenerarDocumentoPago(int pedidoId)
        {
            return _documentoRepository.GenerarDocumentoPago(pedidoId);
        }
        public IEnumerable<DocumentoPago> ListarDocumentos()
        {
            return _documentoRepository.ListarDocumentos();
        }
        public bool CambiarEstadoDocumento(int documentoId, string estado)
        {
            return _documentoRepository.CambiarEstadoDocumento(documentoId, estado);
        }
        public Precuenta CalcularTotales(int pedidoId)
        {
            return _documentoRepository.CalcularTotales(pedidoId);
        }
    }
}
