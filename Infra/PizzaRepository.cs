using MySqlConnector;
using PizzeriaOpita.App.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzeriaOpita.App.Infra
{
    public class PizzaRepository
    {
        public async Task<int> AddAsync(string nombre, decimal precio)
        {
            using var cn = Db.Get();
            await cn.OpenAsync();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = "INSERT INTO Pizzas(nombre, precio) VALUES(@n, @p); SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@n", nombre);
            cmd.Parameters.AddWithValue("@p", precio);
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(id);
        }

        public async Task<List<Pizza>> ListAsync()
        {
            using var cn = Db.Get();
            await cn.OpenAsync();
            using var cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT idPizza, nombre, precio FROM Pizzas ORDER BY idPizza DESC;";
            using var rd = await cmd.ExecuteReaderAsync();
            var list = new List<Pizza>();
            while (await rd.ReadAsync())
                // En PizzaRepository.cs, ListAsync
list.Add(new Pizza(rd.GetInt32(0), rd.GetString(1), rd.GetDecimal(2))
{
    Nombre = rd.GetString(1)
});
            return list;
        }
    }
}