using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Restaurant.Entities;

namespace Restaurant.Data
{
    public class ProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Obtener todos los productos
        public IEnumerable<Producto> ObtenerTodos()
        {
            var productos = new List<Producto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT ProductoId, Nombre, PrecioUnitario FROM Producto", conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    ProductoId = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    PrecioUnitario = reader.GetDecimal(2)
                });
            }

            return productos;
        }

        // Obtener un producto por Id
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

        // Insertar un producto nuevo
        public int Insertar(Producto producto)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO Producto (Nombre, PrecioUnitario) OUTPUT INSERTED.ProductoId VALUES (@Nombre, @PrecioUnitario)", conn);

            cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@PrecioUnitario", producto.PrecioUnitario);

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        // Eliminar producto
        public bool Eliminar(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM Producto WHERE ProductoId = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
