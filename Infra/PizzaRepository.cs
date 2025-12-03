using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.Infra
{
    /// <summary>
    /// Repositorio para operaciones de base de datos relacionadas con pizzas.
    /// RENDIMIENTO: Todas las operaciones son asíncronas.
    /// SEGURIDAD: Usa parámetros SQL para prevenir inyección SQL.
    /// </summary>
    public class PizzaRepository
    {
        /// <summary>
        /// Agrega una nueva pizza a la base de datos.
        /// SEGURIDAD: Usa parámetros SQL parametrizados para prevenir SQL injection.
        /// </summary>
        /// <param name="pizza">Objeto Pizza a agregar</param>
        public async Task Add(Pizza pizza)
        {
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();
                
                // ✅ SEGURIDAD: Consulta parametrizada previene SQL injection
                using var cmd = new MySqlCommand(
                    "INSERT INTO Pizzas (Nombre, Precio) VALUES (@Nombre, @Precio)", conn);
                    
                cmd.Parameters.AddWithValue("@Nombre", pizza.Nombre);
                cmd.Parameters.AddWithValue("@Precio", pizza.Precio);
                
                await cmd.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al agregar pizza: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las pizzas de la base de datos.
        /// RENDIMIENTO: Operación asíncrona optimizada.
        /// </summary>
        /// <returns>Lista de todas las pizzas</returns>
        public async Task<List<Pizza>> GetAll()
        {
            var pizzas = new List<Pizza>();
            
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();
                
                using var cmd = new MySqlCommand("SELECT IdPizza, Nombre, Precio FROM Pizzas", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    pizzas.Add(new Pizza
                    {
                        IdPizza = reader.GetInt32("IdPizza"),
                        Nombre = reader.GetString("Nombre"),
                        Precio = reader.GetDecimal("Precio")
                    });
                }
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al obtener pizzas: {ex.Message}", ex);
            }
            
            return pizzas;
        }

        /// <summary>
        /// Actualiza una pizza existente en la base de datos.
        /// SEGURIDAD: Usa parámetros SQL para prevenir inyección.
        /// </summary>
        /// <param name="pizza">Pizza con datos actualizados</param>
        public async Task Update(Pizza pizza)
        {
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();
                
                // ✅ SEGURIDAD: Consulta parametrizada
                using var cmd = new MySqlCommand(
                    "UPDATE Pizzas SET Nombre = @Nombre, Precio = @Precio WHERE IdPizza = @IdPizza", conn);
                    
                cmd.Parameters.AddWithValue("@IdPizza", pizza.IdPizza);
                cmd.Parameters.AddWithValue("@Nombre", pizza.Nombre);
                cmd.Parameters.AddWithValue("@Precio", pizza.Precio);
                
                var affectedRows = await cmd.ExecuteNonQueryAsync();
                
                // ⚠️ MEJORA: Validar que la pizza existía
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException($"No se encontró la pizza con ID {pizza.IdPizza}");
                }
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al actualizar pizza: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina una pizza de la base de datos.
        /// SEGURIDAD: Usa parámetros SQL para prevenir inyección.
        /// ⚠️ ADVERTENCIA: Puede fallar si hay pedidos asociados (constraint de FK).
        /// </summary>
        /// <param name="idPizza">ID de la pizza a eliminar</param>
        public async Task Delete(int idPizza)
        {
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();
                
                // ✅ SEGURIDAD: Consulta parametrizada
                using var cmd = new MySqlCommand(
                    "DELETE FROM Pizzas WHERE IdPizza = @IdPizza", conn);
                    
                cmd.Parameters.AddWithValue("@IdPizza", idPizza);
                
                var affectedRows = await cmd.ExecuteNonQueryAsync();
                
                // ⚠️ MEJORA: Validar que la pizza existía
                if (affectedRows == 0)
                {
                    throw new InvalidOperationException($"No se encontró la pizza con ID {idPizza}");
                }
            }
            catch (MySqlException ex) when (ex.Number == 1451) // Foreign key constraint
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar la pizza con ID {idPizza} porque tiene pedidos asociados", ex);
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al eliminar pizza: {ex.Message}", ex);
            }
        }
    }
}
