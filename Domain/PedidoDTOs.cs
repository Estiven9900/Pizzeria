using System;

namespace PizzeriaOpita.App.Domain
{
    public class PedidoListItem
    {
        public int Id { get; }
        public int IdPizza { get; }
        public string NombrePizza { get; }
        public string Estado { get; }
        public DateTime Fecha { get; }

        public PedidoListItem(int id, int idPizza, string nombrePizza, string estado, DateTime fecha)
        {
            Id = id;
            IdPizza = idPizza;
            NombrePizza = nombrePizza ?? string.Empty;
            Estado = estado ?? string.Empty;
            Fecha = fecha;
        }
    }

    public class PedidoPendienteItem
    {
        public int Id { get; }
        public int IdPizza { get; }
        public string NombrePizza { get; }
        public DateTime Fecha { get; }

        public PedidoPendienteItem(int id, int idPizza, string nombrePizza, DateTime fecha)
        {
            Id = id;
            IdPizza = idPizza;
            NombrePizza = nombrePizza ?? string.Empty;
            Fecha = fecha;
        }
    }
}
