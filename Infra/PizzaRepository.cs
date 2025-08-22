using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.Infra
{
    public class PizzaRepository
    {
        public async Task Add(Pizza pizza)
        {
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "INSERT INTO Pizzas (Nombre, Precio) VALUES (@Nombre, @Precio)", conn);
            cmd.Parameters.AddWithValue("@Nombre", pizza.Nombre);
            cmd.Parameters.AddWithValue("@Precio", pizza.Precio);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Pizza>> GetAll()
        {
            var pizzas = new List<Pizza>();
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
            return pizzas;
        }

        // 🔹 Nuevo: Editar Pizza
        public async Task Update(Pizza pizza)
        {
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "UPDATE Pizzas SET Nombre = @Nombre, Precio = @Precio WHERE IdPizza = @IdPizza", conn);
            cmd.Parameters.AddWithValue("@IdPizza", pizza.IdPizza);
            cmd.Parameters.AddWithValue("@Nombre", pizza.Nombre);
            cmd.Parameters.AddWithValue("@Precio", pizza.Precio);
            await cmd.ExecuteNonQueryAsync();
        }

        // 🔹 Nuevo: Eliminar Pizza
        public async Task Delete(int idPizza)
        {
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand(
                "DELETE FROM Pizzas WHERE IdPizza = @IdPizza", conn);
            cmd.Parameters.AddWithValue("@IdPizza", idPizza);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
