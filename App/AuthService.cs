using PizzeriaOpita.App.Domain;
using System.Collections.Generic;
using System.Linq;

namespace PizzeriaOpita.App
{
    public static class AuthService
    {
        private static readonly List<Usuario> Users = new()
        {
            new("admin", "admin1", Rol.Admin),
            new("user", "User1", Rol.Asistente),
            new("pizzero", "pizzero1", Rol.Pizzero)
        };

        public static Usuario? Login(string user, string pass)
            => Users.FirstOrDefault(u => u.NombreUsuario == user && u.Password == pass);

        public static int GetUserIdForDb(Rol rol) => rol switch
        {
            Rol.Admin => 1,
            Rol.Asistente => 2,
            Rol.Pizzero => 3,
            _ => 2
        };
    }
}