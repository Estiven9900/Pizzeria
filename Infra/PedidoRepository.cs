using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.Infra
{
    /// <summary>
    /// Repositorio para operaciones de base de datos relacionadas con pedidos.
    /// RENDIMIENTO: Todas las operaciones son asíncronas para no bloquear el hilo principal.
    /// SEGURIDAD: Usa parámetros SQL para prevenir inyección SQL.
    /// </summary>
    public class PedidoRepository
    {
        /// <summary>
        /// Agrega un nuevo pedido a la base de datos.
        /// SEGURIDAD: Usa parámetros SQL parametrizados para prevenir SQL injection.
        /// </summary>
        /// <param name="idPizza">ID de la pizza</param>
        /// <param name="idAsistente">ID del asistente que toma el pedido</param>
        public async Task AddAsync(int idPizza, int idAsistente)
        {
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();
                
                // ✅ SEGURIDAD: Consulta parametrizada previene SQL injection
                using var cmd = new MySqlCommand(
                    "INSERT INTO Pedidos (idPizza, idAsistente, estado, fecha) VALUES (@idPizza, @idAsistente, @estado, @fecha);",
                    conn);
                    
                cmd.Parameters.AddWithValue("@idPizza", idPizza);
                cmd.Parameters.AddWithValue("@idAsistente", idAsistente);
                cmd.Parameters.AddWithValue("@estado", EstadoPedido.Pendiente);
                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                
                await cmd.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al agregar pedido: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Marca un pedido como entregado.
        /// SEGURIDAD: Usa parámetros SQL para prevenir inyección.
        /// </summary>
        /// <param name="id">ID del pedido a marcar como entregado</param>
        public async Task EntregarAsync(int id)
        {
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();
                
                // ✅ SEGURIDAD: Consulta parametrizada
                using var cmd = new MySqlCommand(
                    "UPDATE Pedidos SET estado = @estado WHERE idPedido = @id;", 
                    conn);
                    
                cmd.Parameters.AddWithValue("@estado", EstadoPedido.Entregado);
                cmd.Parameters.AddWithValue("@id", id);
                
                var affectedRows = await cmd.ExecuteNonQueryAsync();
                
                // ⚠️ MEJORA: Validar que el pedido existía
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException($"No se encontró el pedido con ID {id}");
                }
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al entregar pedido: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene lista completa de pedidos con información de la pizza.
        /// RENDIMIENTO: Join optimizado, ordenado por ID descendente.
        /// </summary>
        /// <returns>Lista de pedidos</returns>
        public async Task<List<PedidoListItem>> ListAsync()
        {
            var pedidos = new List<PedidoListItem>();
            
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();
                
                // ✅ RENDIMIENTO: Query optimizado con JOIN
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
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al listar pedidos: {ex.Message}", ex);
            }
            
            return pedidos;
        }

        /// <summary>
        /// Obtiene lista de pedidos pendientes.
        /// RENDIMIENTO: Filtro en la consulta SQL para mejor rendimiento.
        /// </summary>
        /// <returns>Lista de pedidos pendientes</returns>
        public async Task<List<PedidoPendienteItem>> ListPendientesAsync()
        {
            var pendientes = new List<PedidoPendienteItem>();
            
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();
                
                // ✅ RENDIMIENTO: WHERE optimizado para filtrar en base de datos
                using var cmd = new MySqlCommand(@"
                    SELECT p.idPedido, p.idPizza, pi.nombre AS NombrePizza, p.fecha
                      FROM Pedidos p
                      JOIN Pizzas pi ON p.idPizza = pi.idPizza
                      WHERE p.estado = @estado
                      ORDER BY p.fecha;", conn);

                cmd.Parameters.AddWithValue("@estado", EstadoPedido.Pendiente);

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
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al listar pedidos pendientes: {ex.Message}", ex);
            }
            
            return pendientes;
        }
    }
}
