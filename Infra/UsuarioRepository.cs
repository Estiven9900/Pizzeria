using System;
using System.Threading.Tasks;
using MySqlConnector;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.Infra
{
    /// <summary>
    /// Repositorio para operaciones de base de datos relacionadas con usuarios.
    /// SEGURIDAD: Usa parámetros SQL para prevenir inyección SQL.
    /// 
    /// ⚠️ CRÍTICO - VULNERABILIDAD DE SEGURIDAD:
    /// Las contraseñas se almacenan en texto plano en la base de datos.
    /// Ver documentación en AuthService para recomendaciones de mitigación.
    /// </summary>
    public class UsuarioRepository
    {
        /// <summary>
        /// Obtiene un usuario por nombre de usuario.
        /// SEGURIDAD: Usa parámetros SQL para prevenir inyección.
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario a buscar</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<Usuario?> GetByUsernameAsync(string nombreUsuario)
        {
            try
            {
                using var conn = Db.Get();
                await conn.OpenAsync();

                // ✅ SEGURIDAD: Consulta parametrizada previene SQL injection
                using var cmd = new MySqlCommand(
                    @"SELECT idUsuario,
                             nombre,
                             rol,
                             usuario,
                             `contraseña` AS contrasena
                      FROM Usuarios
                      WHERE usuario = @u
                      LIMIT 1;", conn);

                cmd.Parameters.AddWithValue("@u", nombreUsuario?.Trim());

                using var r = await cmd.ExecuteReaderAsync();
                if (!await r.ReadAsync()) return null;

                var rolStr = r.IsDBNull(r.GetOrdinal("rol")) ? string.Empty : r.GetString("rol");
                Enum.TryParse<Rol>(rolStr, ignoreCase: true, out var rol);

                return new Usuario
                {
                    IdUsuario = r.IsDBNull(r.GetOrdinal("idUsuario")) ? 0 : r.GetInt32("idUsuario"),
                    Nombre = r.IsDBNull(r.GetOrdinal("nombre")) ? string.Empty : r.GetString("nombre"),
                    NombreUsuario = r.IsDBNull(r.GetOrdinal("usuario")) ? string.Empty : r.GetString("usuario"),
                    Password = r.IsDBNull(r.GetOrdinal("contrasena")) ? string.Empty : r.GetString("contrasena"),
                    Rol = rol
                };
            }
            catch (MySqlException ex)
            {
                throw new InvalidOperationException($"Error de base de datos al obtener usuario: {ex.Message}", ex);
            }
        }
    }
}
