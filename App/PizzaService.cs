using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzeriaOpita.App
{
    public class PizzaService
    {
        private readonly PizzaRepository _repo;

        public PizzaService(PizzaRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Pizza>> Listar() => _repo.ListAsync();

        public Task<int> Registrar(string nombre, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("Nombre requerido");
            if (precio <= 0) throw new ArgumentException("Precio debe ser mayor a 0");
            return _repo.AddAsync(nombre.Trim(), precio);
        }
    }
}