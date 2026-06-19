using Restaurant.Data;
using Restaurant.Entities;

namespace Restaurant.Business
{
    public class DetallePedidoService
    {
        private readonly DetallePedidoRepository _detalleRepository;

        public DetallePedidoService(DetallePedidoRepository detalleRepository)
        {
            _detalleRepository = detalleRepository;
        }

        public bool AgregarDetallePedido(DetallePedido detalle)
        {
            return _detalleRepository.AgregarDetallePedido(detalle);
        }

        public List<DetallePedido> ObtenerDetallesPorPedido(int pedidoId)
        {
            return _detalleRepository.ObtenerDetallesPorPedido(pedidoId);
        }

        public bool EliminarDetallePedido(int detalleId)
        {
            return _detalleRepository.EliminarDetallePedido(detalleId);
        }

    }
}
