// PizzaService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;

namespace PizzeriaOpita.App
{
    public class PizzaService
    {
        private readonly PizzaRepository _repo;

        public PizzaService(PizzaRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task Registrar(string nombre, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido");
            if (precio <= 0) throw new ArgumentException("Precio debe ser mayor a 0");
            await _repo.AddAsync(new Pizza(0, nombre.Trim(), precio));
        }

        public async Task<List<Pizza>> Listar()
        {
            return await _repo.ListAsync();
        }
    }
}