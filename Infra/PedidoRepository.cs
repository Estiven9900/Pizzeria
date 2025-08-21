using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.Infra
{
    public class PedidoRepository
    {
        public async Task AddAsync(int idPizza, int idAsistente)
        {
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "INSERT INTO Pedidos (idPizza, idAsistente, estado, fecha) VALUES (@idPizza, @idAsistente, 'Pendiente', @fecha);",
                conn);
            cmd.Parameters.AddWithValue("@idPizza", idPizza);
            cmd.Parameters.AddWithValue("@idAsistente", idAsistente);
            cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EntregarAsync(int id)
        {
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("UPDATE Pedidos SET estado = 'Entregado' WHERE idPedido = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<PedidoListItem>> ListAsync()
        {
            var pedidos = new List<PedidoListItem>();
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT p.idPedido, p.idPizza, pi.nombre AS NombrePizza, p.estado, p.fecha
                  FROM Pedidos p
                  JOIN Pizzas pi ON p.idPizza = pi.idPizza
                  ORDER BY p.idPedido DESC;", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                pedidos.Add(new PedidoListItem(
                    reader.GetInt32("idPedido"),
                    reader.GetInt32("idPizza"),
                    reader.GetString("NombrePizza"),
                    reader.GetString("estado"),
                    reader.GetDateTime("fecha")
                ));
            }
            return pedidos;
        }

        public async Task<List<PedidoPendienteItem>> ListPendientesAsync()
        {
            var pendientes = new List<PedidoPendienteItem>();
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(@"
                SELECT p.idPedido, p.idPizza, pi.nombre AS NombrePizza, p.fecha
                  FROM Pedidos p
                  JOIN Pizzas pi ON p.idPizza = pi.idPizza
                  WHERE p.estado = 'Pendiente'
                  ORDER BY p.fecha;", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                pendientes.Add(new PedidoPendienteItem(
                    reader.GetInt32("idPedido"),
                    reader.GetInt32("idPizza"),
                    reader.GetString("NombrePizza"),
                    reader.GetDateTime("fecha")
                ));
            }
            return pendientes;
        }
    }
}
