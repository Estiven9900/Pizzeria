using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.Infra
{
    public class PizzaRepository
    {
        public async Task AddAsync(Pizza pizza)
        {
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("INSERT INTO Pizzas (nombre, precio) VALUES (@nombre, @precio);", conn);
            cmd.Parameters.AddWithValue("@nombre", pizza.Nombre);
            cmd.Parameters.AddWithValue("@precio", pizza.Precio);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Pizza>> ListAsync()
        {
            var pizzas = new List<Pizza>();
            using var conn = Db.Get();
            await conn.OpenAsync();
            using var cmd = new MySqlCommand("SELECT idPizza, nombre, precio FROM Pizzas ORDER BY idPizza DESC;", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                pizzas.Add(new Pizza(
                    reader.GetInt32("idPizza"),
                    reader.GetString("nombre"),
                    reader.GetDecimal("precio")
                ));
            }
            return pizzas;
        }
    }
}
