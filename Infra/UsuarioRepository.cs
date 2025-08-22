using System.Threading.Tasks;
using MySqlConnector;
using PizzeriaOpita.App.Domain;

namespace PizzeriaOpita.App.Infra
{
    public class UsuarioRepository
    {
        public async Task<Usuario?> GetByUsernameAsync(string nombreUsuario)
        {
            using var conn = Db.Get();
            await conn.OpenAsync();

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
    }
}
