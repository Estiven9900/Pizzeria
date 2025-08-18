using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzeriaOpita.App
{
    public class PedidoService
    {
        private readonly PedidoRepository _repo;

        public PedidoService(PedidoRepository repo)
        {
            _repo = repo;
        }

        public Task<int> Registrar(int idPizza, int idAsistente)
        {
            if (idPizza <= 0) throw new ArgumentException("Pizza inválida");
            if (idAsistente <= 0) throw new ArgumentException("Asistente inválido");
            return _repo.AddAsync(idPizza, idAsistente);
        }

        public Task<int> Entregar(int idPedido)
        {
            if (idPedido <= 0) throw new ArgumentException("Pedido inválido");
            return _repo.EntregarAsync(idPedido);
        }

        public Task<List<PedidoListItem>> Listar() => _repo.ListAsync();

        public Task<List<PedidoPendienteItem>> Pendientes() => _repo.ListPendientesAsync();
    }
}