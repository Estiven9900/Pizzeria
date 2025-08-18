using System;
using System.Data;
using MySqlConnector;

namespace PizzeriaOpita.App.Domain
{
    public class Usuario
    {
        public string NombreUsuario { get; }
        public string Password { get; }
        public Rol Rol { get; }

        public Usuario(string nombreUsuario, string password, Rol rol)
        {
            NombreUsuario = nombreUsuario;
            Password = password;
            Rol = rol;
        }
    }

    public enum Rol
    {
        Admin = 1,
        Asistente = 2,
        Pizzero = 3
    }

    public class PedidoListItem
    {
        public int IdPedido { get; set; }
        public int IdPizza { get; set; }
        public string NombrePizza { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public DateTime Fecha { get; set; }

        public PedidoListItem(int idPedido, int idPizza, string nombrePizza, string estado, DateTime fecha)
        {
            IdPedido = idPedido;
            IdPizza = idPizza;
            NombrePizza = nombrePizza;
            Estado = estado;
            Fecha = fecha;
        }
    }

    public class PedidoPendienteItem
    {
        public int IdPedido { get; set; }
        public int IdPizza { get; set; }
        public string NombrePizza { get; set; } = null!;
        public DateTime Fecha { get; set; }

        public PedidoPendienteItem(int idPedido, int idPizza, string nombrePizza, DateTime fecha)
        {
            IdPedido = idPedido;
            IdPizza = idPizza;
            NombrePizza = nombrePizza;
            Fecha = fecha;
        }
    }

    public class Pizza
    {
        public int IdPizza { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }

        public Pizza(int idPizza, string nombre, decimal precio)
        {
            IdPizza = idPizza;
            Nombre = nombre;
            Precio = precio;
        }
    }

    public static class Db
    {
        private const string Conn = "Server=localhost;Port=3306;Database=sys;User Id=root;Password=Es.172629282;AllowUserVariables=True;";

        public static MySqlConnection Get()
            => new MySqlConnection(Conn);
    }
}