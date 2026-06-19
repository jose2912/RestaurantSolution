using Restaurant.Data;
using Restaurant.Entities;

namespace Restaurant.Business
{
    public class PedidoService
    {
        private readonly PedidoRepository _pedidoRepository;

        public PedidoService(PedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public Pedido? ObtenerPedidoPorId(int id)
        {
            return _pedidoRepository.ObtenerPedidoPorId(id);
        }

        public int CrearPedido(Pedido pedido)
        {
            return _pedidoRepository.CrearPedido(pedido);
        }

        public bool CambiarEstado(int id, string nuevoEstado)
        {
            return _pedidoRepository.CambiarEstado(id, nuevoEstado);
        }

        public Precuenta CalcularTotales(int pedidoId)
        {
            return _pedidoRepository.CalcularTotales(pedidoId);
        }
        // NUEVO: Agregar detalle
        public void AgregarDetalle(DetallePedido detalle)
        {
            _pedidoRepository.AgregarDetalle(detalle);
        }
        public IEnumerable<Pedido> ObtenerTodos()
        {
            return _pedidoRepository.ObtenerTodos();
        }
        public bool EliminarPedido(int id)
        {
            return _pedidoRepository.EliminarPedido(id);
        }
        public IEnumerable<Producto> ObtenerProductos()
        {
            return _pedidoRepository.ObtenerProductos();
        }
        public bool CambiarEstadoPedido(int id, string nuevoEstado)
        {
            return _pedidoRepository.CambiarEstado(id, nuevoEstado);
        }
    }
}
