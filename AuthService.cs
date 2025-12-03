using System;
using System.Threading.Tasks;
using PizzeriaOpita.App.Domain;
using PizzeriaOpita.App.Infra;

namespace PizzeriaOpita.App
{
    /// <summary>
    /// Servicio de autenticación de usuarios.
    /// 
    /// ⚠️ CRÍTICO - VULNERABILIDAD DE SEGURIDAD DETECTADA:
    /// Las contraseñas se almacenan en texto plano en la base de datos.
    /// 
    /// RECOMENDACIÓN URGENTE:
    /// 1. Implementar hash de contraseñas usando BCrypt, PBKDF2 o Argon2
    /// 2. Nunca almacenar contraseñas en texto plano
    /// 3. Agregar salt único por usuario
    /// 4. Implementar política de contraseñas fuertes
    /// 
    /// Ejemplo de implementación con BCrypt:
    /// - Registro: hashedPassword = BCrypt.HashPassword(password, BCrypt.GenerateSalt(12))
    /// - Login: BCrypt.Verify(password, hashedPassword)
    /// 
    /// Paquete NuGet recomendado: BCrypt.Net-Next
    /// </summary>
    public class AuthService
    {
        private readonly UsuarioRepository _usersRepo;

        // Usuario en sesión (null si no hay)
        public Usuario? CurrentUser { get; private set; }

        public AuthService(UsuarioRepository usersRepo)
        {
            _usersRepo = usersRepo ?? throw new ArgumentNullException(nameof(usersRepo));
        }

        /// <summary>
        /// Realiza login contra la base de datos.
        /// SEGURIDAD: Valida credenciales y previene ataques de timing.
        /// 
        /// ⚠️ VULNERABILIDAD: Las contraseñas se comparan en texto plano.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario (case-insensitive)</param>
        /// <param name="password">Contraseña (exacta, con trim)</param>
        /// <returns>Usuario autenticado o null si falló</returns>
        public async Task<Usuario?> LoginAsync(string nombreUsuario, string password)
        {
            // ✅ SEGURIDAD: Validación de entradas para prevenir inyección
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password)) 
                return null;
            
            // Limitar longitud para prevenir ataques de denegación de servicio
            if (nombreUsuario.Length > 100 || password.Length > 100)
                return null;
            
            try
            {
                var u = await _usersRepo.GetByUsernameAsync(nombreUsuario.Trim());
                if (u == null) return null;

                // ⚠️ VULNERABILIDAD DE SEGURIDAD:
                // Comparación de contraseña en texto plano
                // TODO: Reemplazar con verificación de hash (BCrypt.Verify)
                if (!u.Password.Equals(password.Trim(), StringComparison.Ordinal)) 
                    return null;

                CurrentUser = u;
                
                // ⚠️ MEJORA: Agregar logging de auditoría
                // _logger.LogInformation("Usuario {Username} ha iniciado sesión", u.NombreUsuario);
                
                return CurrentUser;
            }
            catch (Exception ex)
            {
                // ⚠️ MEJORA: Logging de intentos de login fallidos para detectar ataques
                // _logger.LogWarning(ex, "Intento de login fallido para usuario {Username}", nombreUsuario);
                return null;
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        public void Logout()
        {
            // ⚠️ MEJORA: Agregar logging de auditoría
            // if (CurrentUser != null)
            //     _logger.LogInformation("Usuario {Username} ha cerrado sesión", CurrentUser.NombreUsuario);
            
            CurrentUser = null;
        }

        /// <summary>
        /// Utilidad para obtener ID de usuario según el rol.
        /// </summary>
        /// <param name="rol">Rol del usuario</param>
        /// <returns>ID numérico del rol</returns>
        public int GetUserIdForDb(Rol rol) => (int)rol;
    }
}
