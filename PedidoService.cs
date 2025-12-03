using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;

namespace PizzeriaOpita.App
{
    /// <summary>
    /// Servicio para gestionar operaciones de pedidos.
    /// Implementa validación de entradas y manejo de errores.
    /// </summary>
    public class PedidoService
    {
        private readonly PedidoRepository _repo;

        public PedidoService(PedidoRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Registra un nuevo pedido en el sistema.
        /// SEGURIDAD: Valida estrictamente los parámetros de entrada para prevenir datos inválidos.
        /// </summary>
        /// <param name="idPizza">ID de la pizza (debe ser > 0)</param>
        /// <param name="idAsistente">ID del asistente (debe ser > 0)</param>
        /// <exception cref="ArgumentException">Si los parámetros son inválidos</exception>
        public async Task Registrar(int idPizza, int idAsistente)
        {
            // ✅ SEGURIDAD: Validación estricta de entradas
            if (idPizza <= 0) 
                throw new ArgumentException("El ID de la pizza debe ser mayor a 0", nameof(idPizza));
            if (idAsistente <= 0) 
                throw new ArgumentException("El ID del asistente debe ser mayor a 0", nameof(idAsistente));
            
            try
            {
                await _repo.AddAsync(idPizza, idAsistente);
            }
            catch (Exception ex)
            {
                // ⚠️ MEJORA: Aquí se podría agregar logging para auditoría
                // Ej: _logger.LogError(ex, "Error al registrar pedido. Pizza: {IdPizza}, Asistente: {IdAsistente}", idPizza, idAsistente);
                throw new InvalidOperationException($"Error al registrar el pedido: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Marca un pedido como entregado.
        /// SEGURIDAD: Valida que el ID del pedido sea válido.
        /// </summary>
        /// <param name="idPedido">ID del pedido (debe ser > 0)</param>
        /// <exception cref="ArgumentException">Si el ID del pedido es inválido</exception>
        public async Task Entregar(int idPedido)
        {
            // ✅ SEGURIDAD: Validación estricta de entradas
            if (idPedido <= 0) 
                throw new ArgumentException("El ID del pedido debe ser mayor a 0", nameof(idPedido));
            
            try
            {
                await _repo.EntregarAsync(idPedido);
            }
            catch (Exception ex)
            {
                // ⚠️ MEJORA: Logging para auditoría
                throw new InvalidOperationException($"Error al entregar el pedido {idPedido}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene la lista completa de pedidos.
        /// RENDIMIENTO: Operación asíncrona para no bloquear la UI.
        /// </summary>
        /// <returns>Lista de todos los pedidos</returns>
        public async Task<List<PedidoListItem>> Listar()
        {
            try
            {
                return await _repo.ListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al listar pedidos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene la lista de pedidos pendientes.
        /// RENDIMIENTO: Operación asíncrona para no bloquear la UI.
        /// </summary>
        /// <returns>Lista de pedidos pendientes</returns>
        public async Task<List<PedidoPendienteItem>> Pendientes()
        {
            try
            {
                return await _repo.ListPendientesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al listar pedidos pendientes: {ex.Message}", ex);
            }
        }
    }
}
