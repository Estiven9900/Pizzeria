using MySqlConnector;
using PizzeriaOpita.App.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzeriaOpita.App.Infra
{
    public class PedidoRepository
    {
        public async Task<int> AddAsync(int idPizza, int idAsistente)
        {
            using var cn = Db.Get();
            await cn.OpenAsync();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Pedidos(idPizza, idAsistente, estado) 
                                VALUES(@p, @a, 'Pendiente'); SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@p", idPizza);
            cmd.Parameters.AddWithValue("@a", idAsistente);
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        public async Task<int> EntregarAsync(int idPedido)
        {
            using var cn = Db.Get();
            await cn.OpenAsync();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = "UPDATE Pedidos SET estado='Entregado' WHERE idPedido=@id;";
            cmd.Parameters.AddWithValue("@id", idPedido);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<PedidoListItem>> ListAsync()
        {
            using var cn = Db.Get();
            await cn.OpenAsync();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"
                SELECT pe.idPedido, pe.idPizza, pi.nombre, pe.estado, pe.fecha
                FROM Pedidos pe 
                JOIN Pizzas pi ON pe.idPizza = pi.idPizza
                ORDER BY pe.idPedido DESC;";
            using var rd = await cmd.ExecuteReaderAsync();
            var list = new List<PedidoListItem>();
            while (await rd.ReadAsync())
             // En PedidoRepository.cs, ListAsync
list.Add(new PedidoListItem(rd.GetInt32(0), rd.GetInt32(1), rd.GetString(2), rd.GetString(3), rd.GetDateTime(4))
{
    NombrePizza = rd.GetString(2),
    Estado = rd.GetString(3)
});
            return list;
        }
        public async Task<List<PedidoPendienteItem>> ListPendientesAsync()
        {
            using var cn = Db.Get();
            await cn.OpenAsync();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"
                SELECT pe.idPedido, pe.idPizza, pi.nombre, pe.fecha
                FROM Pedidos pe 
                JOIN Pizzas pi ON pe.idPizza = pi.idPizza
                WHERE pe.estado='Pendiente'
                ORDER BY pe.fecha;";
            using var rd = await cmd.ExecuteReaderAsync();
            var list = new List<PedidoPendienteItem>();
            while (await rd.ReadAsync())
                // En PedidoRepository.cs, ListPendientesAsync
list.Add(new PedidoPendienteItem(rd.GetInt32(0), rd.GetInt32(1), rd.GetString(2), rd.GetDateTime(3))
{
    NombrePizza = rd.GetString(2)
});
            return list;
        }
    }
}