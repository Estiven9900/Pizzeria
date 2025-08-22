using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;

namespace PizzeriaOpita.App
{
    public class PedidoService
    {
        private readonly PedidoRepository _repo;

        public PedidoService(PedidoRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task Registrar(int idPizza, int idAsistente)
        {
            if (idPizza <= 0) throw new ArgumentException("Pizza inválida");
            if (idAsistente <= 0) throw new ArgumentException("Asistente inválido");
            await _repo.AddAsync(idPizza, idAsistente);
        }

        public async Task Entregar(int idPedido)
        {
            if (idPedido <= 0) throw new ArgumentException("Pedido inválido");
            await _repo.EntregarAsync(idPedido);
        }

        public async Task<List<PedidoListItem>> Listar()
        {
            return await _repo.ListAsync();
        }

        public async Task<List<PedidoPendienteItem>> Pendientes()
        {
            return await _repo.ListPendientesAsync();
        }
    }
}
