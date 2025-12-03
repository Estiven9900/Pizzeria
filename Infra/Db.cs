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
        // Esta cadena de conexión contiene credenciales hardcoded.
        // RECOMENDACIÓN: Mover a appsettings.json o variables de entorno en producción.
        // Ejemplo: Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
        private static readonly string ConnectionString = 
            "Server=localhost;Database=pizzeria;User=root;Password=;SslMode=None;";

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
