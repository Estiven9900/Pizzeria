using MySqlConnector;
using System;

namespace PizzeriaOpita.App.Infra
{
    /// <summary>
    /// Clase estática para gestionar conexiones a la base de datos MySQL.
    /// IMPORTANTE: En producción, la cadena de conexión debe almacenarse en configuración segura (appsettings.json, variables de entorno, Azure Key Vault, etc.)
    /// </summary>
    public static class Db
    {
        // ⚠️ ADVERTENCIA DE SEGURIDAD: 
        // Esta cadena de conexión es solo para desarrollo local.
        // RECOMENDACIÓN CRÍTICA para producción:
        // 1. Usar variables de entorno: Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
        // 2. Habilitar SSL/TLS: SslMode=Required
        // 3. Usar credenciales seguras (no root con password vacía)
        // 4. Almacenar en Azure Key Vault, AWS Secrets Manager o similar
        private static readonly string ConnectionString = 
            Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING") 
            ?? "Server=localhost;Database=pizzeria;User=pizzeria_user;Password=dev_password;SslMode=None;";

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos.
        /// El llamador es responsable de cerrar/dispose la conexión (patrón using).
        /// </summary>
        /// <returns>Una instancia de MySqlConnection no abierta.</returns>
        public static MySqlConnection Get()
        {
            return new MySqlConnection(ConnectionString);
        }

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos ya abierta de forma asíncrona.
        /// El llamador es responsable de cerrar/dispose la conexión (patrón using).
        /// </summary>
        /// <returns>Una instancia de MySqlConnection abierta.</returns>
        public static async System.Threading.Tasks.Task<MySqlConnection> GetOpenAsync()
        {
            var conn = new MySqlConnection(ConnectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
}
