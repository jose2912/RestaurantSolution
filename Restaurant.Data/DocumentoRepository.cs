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

        //public int GenerarDocumentoPago(int pedidoId)
        //{
        //    using var conn = new SqlConnection(_connectionString);
        //    using var cmd = new SqlCommand("sp_GenerarDocumentoPago", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

        //    conn.Open();
        //    return Convert.ToInt32(cmd.ExecuteScalar());
        //}
        //public int GenerarDocumentoPago(int pedidoId)
        //{
        //    using var conn = new SqlConnection(_connectionString);
        //    using var cmd = new SqlCommand("sp_GenerarDocumentoPago", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    // Reglas de negocio en C#
        //    cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
        //    cmd.Parameters.AddWithValue("@FechaDocumento", DateTime.Now);
        //    cmd.Parameters.AddWithValue("@Estado", "En Proceso");

        //    // Calcular total desde DetallePedido
        //    decimal total = CalcularTotalPedido(pedidoId);
        //    cmd.Parameters.AddWithValue("@Total", total);

        //    conn.Open();
        //    return Convert.ToInt32(cmd.ExecuteScalar());
        //}
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
        //public bool CambiarEstadoDocumento(int documentoId, string estado)
        //{
        //    using var conn = new SqlConnection(_connectionString);
        //    using var cmd = new SqlCommand("sp_CambiarEstadoDocumento", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@DocumentoId", documentoId);
        //    cmd.Parameters.AddWithValue("@Estado", estado);

        //    conn.Open();
        //    int rows = cmd.ExecuteNonQuery();
        //    return rows >= 0;
        //}
        //public int CambiarEstadoDocumento(int documentoId, string estado)
        //{
        //    using var conn = new SqlConnection(_connectionString);
        //    using var cmd = new SqlCommand("sp_CambiarEstadoDocumento", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@DocumentoId", documentoId);
        //    cmd.Parameters.AddWithValue("@Estado", estado);

        //    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
        //    returnParameter.Direction = ParameterDirection.ReturnValue;

        //    conn.Open();
        //    cmd.ExecuteNonQuery();

        //    return (int)returnParameter.Value; // 1 = existe, 0 = no existe
        //}
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
    }
}
