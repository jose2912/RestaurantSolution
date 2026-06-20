using System.Data;
using Microsoft.Data.SqlClient;
using Restaurant.Entities;

namespace Restaurant.Data
{
    public class DocumentoRepository
    {
        private readonly string _connectionString;

        public DocumentoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Producto ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT ProductoId, Nombre, PrecioUnitario FROM Producto WHERE ProductoId = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Producto
                {
                    ProductoId = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    PrecioUnitario = reader.GetDecimal(2)
                };
            }

            return null;
        }
        public int GenerarDocumentoPago(int pedidoId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GenerarDocumentoPago", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
            cmd.Parameters.AddWithValue("@FechaDocumento", DateTime.Now);
            cmd.Parameters.AddWithValue("@Estado", "En Proceso");

            decimal total = CalcularTotalPedido(pedidoId);
            cmd.Parameters.AddWithValue("@Total", total);

            conn.Open();
            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                return 0; // Pedido no existe
            }

            return Convert.ToInt32(result);
        }



        private decimal CalcularTotalPedido(int pedidoId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT SUM(TotalLinea) FROM DetallePedido WHERE PedidoId = @PedidoId", conn);
            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

            conn.Open();
            var result = cmd.ExecuteScalar();
            return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }


        // Listar documentos generados
        public IEnumerable<DocumentoPago> ListarDocumentos()
        {
            var documentos = new List<DocumentoPago>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM DocumentoPago", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                documentos.Add(new DocumentoPago
                {
                    DocumentoId = (int)reader["DocumentoId"],
                    PedidoId = (int)reader["PedidoId"],
                    FechaDocumento = (DateTime)reader["FechaDocumento"],
                    Estado = reader["Estado"].ToString(),
                    Total = (decimal)reader["Total"]
                });
            }

            return documentos;
        }       
        public bool CambiarEstadoDocumento(int documentoId, string estado)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Verificar estado actual
            using (var checkCmd = new SqlCommand("SELECT Estado FROM DocumentoPago WHERE DocumentoId = @DocumentoId", conn))
            {
                checkCmd.Parameters.AddWithValue("@DocumentoId", documentoId);
                var currentEstado = checkCmd.ExecuteScalar()?.ToString();

                if (currentEstado == null)
                {
                    // Documento no existe
                    return false;
                }

                if (currentEstado == estado)
                {
                    // Ya está en ese estado → éxito
                    return true;
                }
            }

            // Ejecutar el SP para cambiar estado
            using (var cmd = new SqlCommand("sp_CambiarEstadoDocumento", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentoId", documentoId);
                cmd.Parameters.AddWithValue("@Estado", estado);

                cmd.ExecuteNonQuery();
                return true;
            }
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
        public Pedido ObtenerPedidoPorId(int pedidoId)
        {
            Pedido pedido = null;

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                @"SELECT p.PedidoId, p.FechaPedido, p.Estado,
                 d.DetalleId, d.ProductoId, d.Cantidad, d.PrecioUnitario,
                 pr.Nombre, pr.PrecioUnitario AS PrecioProducto
          FROM Pedido p
          INNER JOIN DetallePedido d ON p.PedidoId = d.PedidoId
          INNER JOIN Producto pr ON d.ProductoId = pr.ProductoId
          WHERE p.PedidoId = @PedidoId", conn);

            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (pedido == null)
                {
                    pedido = new Pedido
                    {
                        PedidoId = reader.GetInt32(reader.GetOrdinal("PedidoId")),
                        FechaPedido = reader.GetDateTime(reader.GetOrdinal("FechaPedido")),
                        Estado = reader.GetString(reader.GetOrdinal("Estado")),
                        DetallePedidos = new List<DetallePedido>()
                    };
                }

                var detalle = new DetallePedido
                {
                    DetalleId = reader.GetInt32(reader.GetOrdinal("DetalleId")),
                    PedidoId = pedido.PedidoId,
                    ProductoId = reader.GetInt32(reader.GetOrdinal("ProductoId")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                    Producto = new Producto
                    {
                        ProductoId = reader.GetInt32(reader.GetOrdinal("ProductoId")),
                        Nombre = reader.GetString(reader.GetOrdinal("Nombre")),
                        PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioProducto"))
                    }
                };

                pedido.DetallePedidos.Add(detalle);
            }

            return pedido;
        }
    }
}
