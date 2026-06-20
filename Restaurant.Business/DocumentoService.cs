using Restaurant.Data;
using Restaurant.Entities;
using System.Reflection.Metadata;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Restaurant.Business
{
    public class DocumentoService
    {
        private readonly DocumentoRepository _documentoRepository;
        private readonly PedidoService _pedidoService;
        public DocumentoService(DocumentoRepository documentoRepository, PedidoService pedidoService)
        {
            _documentoRepository = documentoRepository;
            _pedidoService = pedidoService;
        }
        public byte[] ObtenerPdfPorId(int pedidoId)
        {
            // Recuperar el pedido con sus detalles y totales
            var pedido = _pedidoService.ObtenerPedidoPorId(pedidoId);
            if (pedido == null) return null;

            using (var ms = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Encabezado
                doc.Add(new Paragraph("Precuenta"));
                doc.Add(new Paragraph($"Pedido ID: {pedido.PedidoId}"));
                doc.Add(new Paragraph($"Fecha: {pedido.FechaPedido:dd/MM/yyyy}"));
                doc.Add(new Paragraph($"Estado: {pedido.Estado}"));
                doc.Add(new Paragraph(" "));

                // Tabla de productos
                PdfPTable table = new PdfPTable(4);
                table.AddCell("Producto");
                table.AddCell("Cantidad");
                table.AddCell("Precio Unitario");
                table.AddCell("Total Línea");

                foreach (var d in pedido.DetallePedidos)
                {
                    table.AddCell(d.Producto.Nombre);
                    table.AddCell(d.Cantidad.ToString());
                    table.AddCell(d.PrecioUnitario.ToString("C"));
                    table.AddCell(d.TotalLinea.ToString("C"));
                }

                doc.Add(table);
                doc.Add(new Paragraph(" "));

                // Totales del pedido
                doc.Add(new Paragraph($"Subtotal: {pedido.SubTotal:C}"));
                doc.Add(new Paragraph($"Impuesto: {pedido.Impuesto:C}"));
                doc.Add(new Paragraph($"Total: {pedido.Total:C}"));

                doc.Close();
                return ms.ToArray();
            }
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
