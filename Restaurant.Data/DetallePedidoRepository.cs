using System.Data;
using Microsoft.Data.SqlClient;
using Restaurant.Entities;

namespace Restaurant.Data
{
    public class DetallePedidoRepository
    {
        private readonly string _connectionString;

        public DetallePedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool AgregarDetallePedido(DetallePedido detalle)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_AgregarDetallePedido", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PedidoId", detalle.PedidoId);
            cmd.Parameters.AddWithValue("@ProductoId", detalle.ProductoId);
            cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
            cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public List<DetallePedido> ObtenerDetallesPorPedido(int pedidoId)
        {
            var detalles = new List<DetallePedido>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_ObtenerDetallesPorPedido", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                detalles.Add(new DetallePedido
                {
                    DetalleId = (int)reader["DetalleId"],
                    PedidoId = (int)reader["PedidoId"],
                    ProductoId = (int)reader["ProductoId"],
                    Cantidad = (int)reader["Cantidad"],
                    PrecioUnitario = (decimal)reader["PrecioUnitario"]
                });
            }
            return detalles;
        }
        public bool EliminarDetallePedido(int detalleId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_EliminarDetallePedido", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DetalleId", detalleId);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
