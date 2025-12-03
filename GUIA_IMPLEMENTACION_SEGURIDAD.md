# 🔒 Guía de Implementación de Seguridad - Pizzería Opita

Esta guía proporciona instrucciones paso a paso para implementar las mejoras de seguridad críticas identificadas en el análisis.

## 📋 Índice

1. [Implementar Hash de Contraseñas con BCrypt](#1-implementar-hash-de-contraseñas-con-bcrypt)
2. [Mover Credenciales a Configuración Externa](#2-mover-credenciales-a-configuración-externa)
3. [Implementar Logging y Auditoría](#3-implementar-logging-y-auditoría)
4. [Configurar SSL/TLS para MySQL](#4-configurar-ssltls-para-mysql)

---

## 1. Implementar Hash de Contraseñas con BCrypt

### Paso 1: Instalar BCrypt.Net-Next

```bash
cd PizzeriaOpita.App
dotnet add package BCrypt.Net-Next
```

### Paso 2: Crear Servicio de Hash

Crear archivo `Infra/PasswordHasher.cs`:

```csharp
using System;

namespace PizzeriaOpita.App.Infra
{
    /// <summary>
    /// Servicio para hash seguro de contraseñas usando BCrypt.
    /// </summary>
    public static class PasswordHasher
    {
        // Número de rondas de hashing (mayor = más seguro pero más lento)
        // 12 es un buen balance entre seguridad y rendimiento
        private const int WorkFactor = 12;

        /// <summary>
        /// Genera un hash seguro de la contraseña.
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash BCrypt de la contraseña</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Verifica si una contraseña coincide con su hash.
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="hash">Hash BCrypt almacenado</param>
        /// <returns>true si la contraseña es correcta</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
```

### Paso 3: Modificar AuthService

Actualizar `AuthService.cs`:

```csharp
public async Task<Usuario?> LoginAsync(string nombreUsuario, string password)
{
    if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password)) 
        return null;
    
    if (nombreUsuario.Length > 100 || password.Length > 100)
        return null;
    
    try
    {
        var u = await _usersRepo.GetByUsernameAsync(nombreUsuario.Trim());
        if (u == null) return null;

        // ✅ SEGURIDAD: Verificar hash de contraseña
        if (!PasswordHasher.VerifyPassword(password.Trim(), u.Password)) 
            return null;

        CurrentUser = u;
        return CurrentUser;
    }
    catch (Exception ex)
    {
        return null;
    }
}
```

### Paso 4: Migrar Base de Datos

**Opción A: Forzar Reset de Contraseñas (Más Simple)**

```sql
-- 1. Alterar la tabla para permitir contraseñas más largas (BCrypt genera hashes de 60 caracteres)
ALTER TABLE Usuarios MODIFY COLUMN `contraseña` VARCHAR(100);

-- 2. Crear contraseña temporal hasheada
-- IMPORTANTE: Generar un hash único para cada entorno
-- Ejemplo de cómo generar el hash en C#:
-- var hash = BCrypt.Net.BCrypt.HashPassword("TU_CONTRASEÑA_TEMPORAL_AQUI", 12);
-- 
-- NO usar este ejemplo en producción - es solo referencia:
-- UPDATE Usuarios 
-- SET `contraseña` = '$2a$12$...HASH_GENERADO_AQUI...';
--
-- Recomendación: Generar hash único y ejecutar UPDATE para cada usuario individualmente

-- 3. Notificar a usuarios para que cambien su contraseña en primer login
```

**Opción B: Migración Gradual (Recomendado)**

```sql
-- 1. Agregar columna temporal para hash
ALTER TABLE Usuarios ADD COLUMN `contraseña_hash` VARCHAR(100);

-- 2. Marcar qué contraseñas están hasheadas
ALTER TABLE Usuarios ADD COLUMN `usa_hash` BOOLEAN DEFAULT FALSE;

-- 3. Actualizar lógica de login para soportar ambos formatos temporalmente
```

Actualizar `AuthService.cs` para migración gradual:

```csharp
public async Task<Usuario?> LoginAsync(string nombreUsuario, string password)
{
    var u = await _usersRepo.GetByUsernameAsync(nombreUsuario.Trim());
    if (u == null) return null;

    bool isValid;
    
    // Durante migración: verificar si usa hash o texto plano
    if (u.Password.StartsWith("$2a$") || u.Password.StartsWith("$2b$")) 
    {
        // Password ya está hasheado
        isValid = PasswordHasher.VerifyPassword(password.Trim(), u.Password);
    }
    else
    {
        // Password en texto plano (legacy)
        isValid = u.Password.Equals(password.Trim(), StringComparison.Ordinal);
        
        // Si el login es exitoso, actualizar a hash
        if (isValid)
        {
            var hashedPassword = PasswordHasher.HashPassword(password.Trim());
            await _usersRepo.UpdatePasswordAsync(u.IdUsuario, hashedPassword);
            // Esto requiere agregar método UpdatePasswordAsync al repositorio
        }
    }

    if (!isValid) return null;
    
    CurrentUser = u;
    return CurrentUser;
}
```

---

## 2. Mover Credenciales a Configuración Externa

### Paso 1: Instalar Paquetes de Configuración

```bash
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package Microsoft.Extensions.Configuration.EnvironmentVariables
```

### Paso 2: Crear appsettings.json

Crear archivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=pizzeria;User=pizzeria_user;Password=CHANGE_ME;SslMode=Required;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### Paso 3: Crear appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=pizzeria_dev;User=root;Password=;SslMode=None;"
  }
}
```

### Paso 4: Actualizar .gitignore

```gitignore
# Archivos de configuración con secretos
appsettings.Production.json
appsettings.Local.json
*.user
*.secrets
```

### Paso 5: Actualizar Program.cs

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows.Forms;
using PizzeriaOpita.App.Infra;
using PizzeriaOpita.App.UI;

namespace PizzeriaOpita.App
{
    internal static class Program
    {
        public static IServiceProvider? ServiceProvider;
        public static IConfiguration? Configuration;

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // ✅ Configurar sistema de configuración
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var services = new ServiceCollection();

            // Agregar configuración a DI
            services.AddSingleton<IConfiguration>(Configuration);

            // Repos
            services.AddSingleton<UsuarioRepository>();
            services.AddSingleton<PizzaRepository>();
            services.AddSingleton<PedidoRepository>();

            // Servicios app
            services.AddSingleton<AuthService>();
            services.AddTransient<PizzaService>();
            services.AddTransient<PedidoService>();

            // Forms (transient)
            services.AddTransient<LoginForm>();
            services.AddTransient<AdminForm>();
            services.AddTransient<AsistenteForm>();
            services.AddTransient<PizzeroForm>();

            ServiceProvider = services.BuildServiceProvider();

            Application.Run(ServiceProvider.GetRequiredService<LoginForm>());
        }
    }
}
```

### Paso 6: Actualizar Db.cs

```csharp
using MySqlConnector;
using System;
using Microsoft.Extensions.Configuration;

namespace PizzeriaOpita.App.Infra
{
    public static class Db
    {
        private static string? _connectionString;

        public static void Initialize(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration");
        }

        public static MySqlConnection Get()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                // Fallback para compatibilidad - usar variable de entorno
                _connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
                    ?? "Server=localhost;Database=pizzeria;User=root;Password=;SslMode=None;";
            }

            return new MySqlConnection(_connectionString);
        }

        public static async System.Threading.Tasks.Task<MySqlConnection> GetOpenAsync()
        {
            var conn = Get();
            await conn.OpenAsync();
            return conn;
        }
    }
}
```

### Paso 7: Inicializar en Program.cs

```csharp
ServiceProvider = services.BuildServiceProvider();

// ✅ Inicializar conexión de base de datos con configuración
Db.Initialize(Configuration);

Application.Run(ServiceProvider.GetRequiredService<LoginForm>());
```

---

## 3. Implementar Logging y Auditoría

### Paso 1: Instalar Serilog

```bash
dotnet add package Serilog
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging
```

### Paso 2: Configurar Serilog en Program.cs

```csharp
using Serilog;

static void Main()
{
    // ✅ Configurar Serilog
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File("logs/pizzeria-.txt", 
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

    try
    {
        Log.Information("Iniciando aplicación Pizzería Opita");
        
        ApplicationConfiguration.Initialize();
        
        // ... resto del código de configuración
        
        Application.Run(ServiceProvider.GetRequiredService<LoginForm>());
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "La aplicación terminó inesperadamente");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}
```

### Paso 3: Agregar Logging a Servicios

```csharp
public class AuthService
{
    private readonly UsuarioRepository _usersRepo;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UsuarioRepository usersRepo, ILogger<AuthService> logger)
    {
        _usersRepo = usersRepo ?? throw new ArgumentNullException(nameof(usersRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Usuario?> LoginAsync(string nombreUsuario, string password)
    {
        _logger.LogInformation("Intento de login para usuario: {Username}", nombreUsuario);
        
        // ... código de validación ...
        
        var u = await _usersRepo.GetByUsernameAsync(nombreUsuario.Trim());
        
        if (u == null)
        {
            _logger.LogWarning("Usuario no encontrado: {Username}", nombreUsuario);
            return null;
        }

        if (!PasswordHasher.VerifyPassword(password.Trim(), u.Password))
        {
            _logger.LogWarning("Contraseña incorrecta para usuario: {Username}", nombreUsuario);
            return null;
        }

        _logger.LogInformation("Login exitoso: {Username} ({Role})", u.NombreUsuario, u.Rol);
        CurrentUser = u;
        return CurrentUser;
    }
}
```

---

## 4. Configurar SSL/TLS para MySQL

### En el Servidor MySQL

```sql
-- Verificar si SSL está habilitado
SHOW VARIABLES LIKE 'have_ssl';

-- Si no está habilitado, configurar en my.cnf o my.ini:
[mysqld]
ssl-ca=/path/to/ca.pem
ssl-cert=/path/to/server-cert.pem
ssl-key=/path/to/server-key.pem
require_secure_transport=ON
```

### En la Aplicación

Actualizar cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=pizzeria;User=pizzeria_user;Password=***;SslMode=Required;SslCa=/path/to/ca.pem;"
  }
}
```

---

## 🎯 Checklist de Implementación

- [ ] **Seguridad de Contraseñas**
  - [ ] Instalar BCrypt.Net-Next
  - [ ] Crear PasswordHasher
  - [ ] Modificar AuthService
  - [ ] Migrar base de datos
  - [ ] Probar login con hash

- [ ] **Configuración Externa**
  - [ ] Instalar paquetes de configuración
  - [ ] Crear appsettings.json
  - [ ] Actualizar .gitignore
  - [ ] Modificar Program.cs
  - [ ] Actualizar Db.cs
  - [ ] Probar conexión

- [ ] **Logging**
  - [ ] Instalar Serilog
  - [ ] Configurar en Program.cs
  - [ ] Agregar logs a AuthService
  - [ ] Agregar logs a otros servicios
  - [ ] Verificar logs generados

- [ ] **SSL/TLS**
  - [ ] Configurar MySQL server
  - [ ] Actualizar cadena de conexión
  - [ ] Probar conexión segura

---

## 🔍 Verificación Final

### Test de Seguridad de Contraseñas

```csharp
// En un programa de test:
var password = "TestPassword123!";
var hash = PasswordHasher.HashPassword(password);
Console.WriteLine($"Hash generado: {hash}");

var isValid = PasswordHasher.VerifyPassword(password, hash);
Console.WriteLine($"Verificación: {isValid}"); // Debe ser true

var isInvalid = PasswordHasher.VerifyPassword("WrongPassword", hash);
Console.WriteLine($"Contraseña incorrecta: {isInvalid}"); // Debe ser false
```

### Test de Configuración

```csharp
var connectionString = Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String cargada: {connectionString}");
```

### Test de Logging

```csharp
_logger.LogInformation("Test de logging funcionando correctamente");
// Verificar que aparece en logs/pizzeria-YYYYMMDD.txt
```

---

## 📚 Recursos Adicionales

- [BCrypt.Net Documentation](https://github.com/BcryptNet/bcrypt.net)
- [Serilog Documentation](https://serilog.net/)
- [Microsoft Configuration](https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [MySQL SSL/TLS Setup](https://dev.mysql.com/doc/refman/8.0/en/using-encrypted-connections.html)

---

**Última actualización:** 2025-12-03  
**Mantenedor:** Equipo de Desarrollo Pizzería Opita
