using System;

namespace PizzeriaOpita.App.Domain
{
    /// <summary>
    /// Roles de usuario en el sistema.
    /// </summary>
    public enum Rol
    {
        Admin = 1,
        Asistente = 2,
        Pizzero = 3
    }

    /// <summary>
    /// Constantes para estados de pedidos.
    /// MEJORA: Usar constantes en lugar de strings hardcoded para evitar errores de tipeo.
    /// </summary>
    public static class EstadoPedido
    {
        public const string Pendiente = "Pendiente";
        public const string Entregado = "Entregado";
    }

    /// <summary>
    /// Entidad de usuario del sistema.
    /// ⚠️ VULNERABILIDAD: La contraseña se almacena en texto plano.
    /// </summary>
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        
        // ⚠️ CRÍTICO: Esta propiedad almacena contraseñas en texto plano
        // TODO: Reemplazar con hash de contraseña
        public string Password { get; set; } = string.Empty;
        
        public Rol Rol { get; set; } = Rol.Asistente;
    }

    /// <summary>
    /// Entidad de pizza.
    /// </summary>
    public class Pizza
    {
        public int IdPizza { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }

        public Pizza() { }

        public Pizza(int idPizza, string nombre, decimal precio)
        {
            IdPizza = idPizza;
            Nombre = nombre ?? string.Empty;
            Precio = precio;
        }
    }

    /// <summary>
    /// Entidad de pedido.
    /// </summary>
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdPizza { get; set; }
        public int IdAsistente { get; set; }
        public int? IdPizzero { get; set; }
        public string Estado { get; set; } = EstadoPedido.Pendiente;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
