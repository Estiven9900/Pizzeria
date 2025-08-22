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
            _repo = repo;
        }

        public async Task Registrar(Pizza pizza)
        {
            await _repo.Add(pizza);
        }

        public async Task<List<Pizza>> Listar()
        {
            return await _repo.GetAll();
        }

        // 🔹 Editar Pizza
        public async Task Editar(Pizza pizza)
        {
            await _repo.Update(pizza);
        }

        // 🔹 Eliminar Pizza
        public async Task Eliminar(int idPizza)
        {
            await _repo.Delete(idPizza);
        }
    }
}
