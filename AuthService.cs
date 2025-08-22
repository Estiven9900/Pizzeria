using System;
using System.Threading.Tasks;
using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;

namespace PizzeriaOpita.App
{
    public class AuthService
    {
        private readonly UsuarioRepository _usersRepo;

        // Usuario en sesión (null si no hay)
        public Usuario? CurrentUser { get; private set; }

        public AuthService(UsuarioRepository usersRepo)
        {
            _usersRepo = usersRepo ?? throw new ArgumentNullException(nameof(usersRepo));
        }

        // Login contra DB. nombreUsuario case-insensitive, password exacto
        public async Task<Usuario?> LoginAsync(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password)) return null;
            var u = await _usersRepo.GetByUsernameAsync(nombreUsuario.Trim());
            if (u == null) return null;

            // contraseña exacta; trim input to avoid espacios accidentales
            if (!u.Password.Equals(password.Trim(), StringComparison.Ordinal)) return null;

            CurrentUser = u;
            return CurrentUser;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        // utilidad
        public int GetUserIdForDb(Rol rol) => (int)rol;
    }
}
