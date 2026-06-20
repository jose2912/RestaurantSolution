using System.Data;
using Microsoft.Data.SqlClient;
using Restaurant.Entities;

namespace Restaurant.Data
{
    public class PedidoRepository
    {
        private readonly string _connectionString;

        public PedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Obtener pedido por Id
        public Pedido ObtenerPedidoPorId(int pedidoId)
        {
            Pedido pedido = null;
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_ObtenerPedidoPorId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                pedido = new Pedido
                {
                    PedidoId = reader.GetInt32(reader.GetOrdinal("PedidoId")),
                    FechaPedido = reader.GetDateTime(reader.GetOrdinal("FechaPedido")),
                    Estado = reader.GetString(reader.GetOrdinal("Estado")),
                    DetallePedidos = new List<DetallePedido>()
                };
            }

            // Si el pedido existe, cargar sus detalles en una segunda consulta
            if (pedido != null)
            {
                pedido.DetallePedidos = ObtenerDetallesPorPedido(pedidoId).ToList();
            }

            return pedido;
        }

        // Crear pedido
        public int CrearPedido(Pedido pedido)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_CrearPedido", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FechaPedido", pedido.FechaPedido);

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Cambiar estado
        public bool CambiarEstado(int id, string nuevoEstado)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_CambiarEstadoPedido", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", id);
            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public Precuenta CalcularTotales(int pedidoId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_CalcularTotalesPedido", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Precuenta
                {
                    //PedidoId = pedidoId,
                    SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                    Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                    Impuesto = reader.GetDecimal(reader.GetOrdinal("Impuesto")),
                    Recargo = reader.GetDecimal(reader.GetOrdinal("Recargo")),
                    Total = reader.GetDecimal(reader.GetOrdinal("Total"))
                };
            }

            return null;
        }
    
        public void AgregarDetalle(DetallePedido detalle)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_AgregarDetallePedido", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", detalle.PedidoId);
            cmd.Parameters.AddWithValue("@ProductoId", detalle.ProductoId);
            cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
            cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<Pedido> ObtenerTodos()
        {
            var pedidos = new List<Pedido>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT PedidoId, FechaPedido, Estado FROM Pedido", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pedidos.Add(new Pedido
                {
                    PedidoId = reader.GetInt32(0),
                    FechaPedido = reader.GetDateTime(1),
                    Estado = reader.GetString(2)
                });
            }
            return pedidos;
        }       
        public bool EliminarPedido(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_EliminarPedido", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", id);

            // Capturar el valor de retorno del SP
            var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
            returnParameter.Direction = ParameterDirection.ReturnValue;

            conn.Open();
            cmd.ExecuteNonQuery();

            int result = (int)returnParameter.Value;
            return result > 0; // true si se eliminó, false si no
        }

        public IEnumerable<Producto> ObtenerProductos()
        {
            var productos = new List<Producto>();
            using var conn = new SqlConnection(_connectionString);
            var query = "SELECT ProductoId, Nombre, PrecioUnitario, Categoria FROM Producto";
            using var cmd = new SqlCommand(query, conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    ProductoId = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    PrecioUnitario = reader.GetDecimal(2)
                    //Categoria = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return productos;
        }
        public IEnumerable<DetallePedido> ObtenerDetallesPorPedido(int pedidoId)
        {
            var detalles = new List<DetallePedido>();
            using var conn = new SqlConnection(_connectionString);

            var query = @"SELECT d.DetalleId, d.PedidoId, d.ProductoId, d.Cantidad, d.PrecioUnitario,
                         p.Nombre
                  FROM DetallePedido d
                  INNER JOIN Producto p ON d.ProductoId = p.ProductoId
                  WHERE d.PedidoId = @PedidoId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                detalles.Add(new DetallePedido
                {
                    DetalleId = reader.GetInt32(0),
                    PedidoId = reader.GetInt32(1),
                    ProductoId = reader.GetInt32(2),
                    Cantidad = reader.GetInt32(3),
                    PrecioUnitario = reader.GetDecimal(4),
                    Producto = new Producto
                    {
                        ProductoId = reader.GetInt32(2),
                        Nombre = reader.GetString(5)
                    }
                });
            }
            return detalles;
        }

    }
}
