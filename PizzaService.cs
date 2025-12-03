using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;

namespace PizzeriaOpita.App
{
    /// <summary>
    /// Servicio para gestionar operaciones de pizzas.
    /// Implementa validación de entradas y manejo de errores.
    /// </summary>
    public class PizzaService
    {
        private readonly PizzaRepository _repo;

        public PizzaService(PizzaRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Registra una nueva pizza en el sistema.
        /// SEGURIDAD: Valida los datos de la pizza para prevenir datos inválidos.
        /// </summary>
        /// <param name="pizza">Objeto Pizza con datos validados</param>
        /// <exception cref="ArgumentNullException">Si pizza es null</exception>
        /// <exception cref="ArgumentException">Si los datos de la pizza son inválidos</exception>
        public async Task Registrar(Pizza pizza)
        {
            // ✅ SEGURIDAD: Validación estricta de entradas
            if (pizza == null)
                throw new ArgumentNullException(nameof(pizza));
            
            ValidarPizza(pizza);
            
            try
            {
                await _repo.Add(pizza);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al registrar la pizza '{pizza.Nombre}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene la lista de todas las pizzas.
        /// RENDIMIENTO: Operación asíncrona para no bloquear la UI.
        /// </summary>
        /// <returns>Lista de pizzas</returns>
        public async Task<List<Pizza>> Listar()
        {
            try
            {
                return await _repo.GetAll();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al listar pizzas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Edita una pizza existente.
        /// SEGURIDAD: Valida los datos antes de actualizar.
        /// </summary>
        /// <param name="pizza">Objeto Pizza con datos actualizados</param>
        /// <exception cref="ArgumentNullException">Si pizza es null</exception>
        /// <exception cref="ArgumentException">Si los datos de la pizza son inválidos</exception>
        public async Task Editar(Pizza pizza)
        {
            // ✅ SEGURIDAD: Validación estricta de entradas
            if (pizza == null)
                throw new ArgumentNullException(nameof(pizza));
            
            if (pizza.IdPizza <= 0)
                throw new ArgumentException("El ID de la pizza debe ser mayor a 0", nameof(pizza));
            
            ValidarPizza(pizza);
            
            try
            {
                await _repo.Update(pizza);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al editar la pizza '{pizza.Nombre}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina una pizza del sistema.
        /// SEGURIDAD: Valida que el ID sea válido antes de eliminar.
        /// </summary>
        /// <param name="idPizza">ID de la pizza a eliminar (debe ser > 0)</param>
        /// <exception cref="ArgumentException">Si el ID es inválido</exception>
        public async Task Eliminar(int idPizza)
        {
            // ✅ SEGURIDAD: Validación estricta de entradas
            if (idPizza <= 0)
                throw new ArgumentException("El ID de la pizza debe ser mayor a 0", nameof(idPizza));
            
            try
            {
                await _repo.Delete(idPizza);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar la pizza con ID {idPizza}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida los datos de una pizza.
        /// SEGURIDAD: Previene inyección de datos inválidos y XSS.
        /// </summary>
        /// <param name="pizza">Pizza a validar</param>
        /// <exception cref="ArgumentException">Si los datos son inválidos</exception>
        private void ValidarPizza(Pizza pizza)
        {
            if (string.IsNullOrWhiteSpace(pizza.Nombre))
                throw new ArgumentException("El nombre de la pizza no puede estar vacío", nameof(pizza.Nombre));
            
            if (pizza.Nombre.Length > 100)
                throw new ArgumentException("El nombre de la pizza no puede exceder 100 caracteres", nameof(pizza.Nombre));
            
            if (pizza.Precio <= 0)
                throw new ArgumentException("El precio de la pizza debe ser mayor a 0", nameof(pizza.Precio));
            
            if (pizza.Precio > 999999.99m)
                throw new ArgumentException("El precio de la pizza es demasiado alto", nameof(pizza.Precio));
            
            // ✅ SEGURIDAD: Prevenir caracteres especiales problemáticos
            // Permitir solo letras, números, espacios y caracteres comunes en nombres de comida
            var caracteresProhibidos = new[] { '<', '>', '\'', '"', ';', '\\', '/', '*', '=', '|' };
            if (pizza.Nombre.IndexOfAny(caracteresProhibidos) >= 0)
                throw new ArgumentException("El nombre de la pizza contiene caracteres no permitidos", nameof(pizza.Nombre));
        }
    }
}
