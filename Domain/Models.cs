using System;

namespace PizzeriaOpita.App.Domain
{
    public enum Rol
    {
        Admin = 1,
        Asistente = 2,
        Pizzero = 3
    }

    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Rol Rol { get; set; } = Rol.Asistente;
    }

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

    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdPizza { get; set; }
        public int IdAsistente { get; set; }
        public int? IdPizzero { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
