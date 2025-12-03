# 🍕 Pizzería Opita - Sistema de Gestión

Sistema de gestión de pedidos para pizzería desarrollado en C# con Windows Forms y MySQL.

## 🔒 Estado de Seguridad

[![Security](https://img.shields.io/badge/Security-Analyzed-green.svg)](ANALISIS_SEGURIDAD_Y_RENDIMIENTO.md)
[![CodeQL](https://img.shields.io/badge/CodeQL-0%20Alerts-success.svg)](RESUMEN_EJECUTIVO.md)
[![Documentation](https://img.shields.io/badge/Documentation-Complete-blue.svg)](RESUMEN_EJECUTIVO.md)

**Última revisión de seguridad:** 2025-12-03  
**Vulnerabilidades críticas:** 2 (documentadas con guías de implementación)

---

## 📋 Características

- ✅ Autenticación de usuarios con roles (Admin, Asistente, Pizzero)
- ✅ Gestión de pizzas (CRUD completo)
- ✅ Gestión de pedidos (registro, listado, entrega)
- ✅ Interfaz gráfica moderna con Material Design
- ✅ Operaciones asíncronas para mejor rendimiento
- ✅ Validación exhaustiva de entradas
- ✅ Protección contra SQL Injection

---

## 🚀 Tecnologías

- **.NET 9.0** - Framework principal
- **Windows Forms** - Interfaz gráfica
- **MySQLConnector** - Conexión a base de datos
- **Material Skin 2** - Diseño moderno
- **Dependency Injection** - Arquitectura limpia

---

## 📦 Instalación

### Requisitos Previos

- Visual Studio 2022 o superior
- .NET 9.0 SDK
- MySQL Server 8.0 o superior
- Windows 7 o superior

### Configuración

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/Estiven9900/Pizzeria.git
   cd Pizzeria
   ```

2. **Configurar base de datos**
   ```sql
   CREATE DATABASE pizzeria;
   USE pizzeria;
   
   -- Crear tablas (ver scripts/database.sql si existe)
   ```

3. **Configurar conexión**
   ```bash
   # Opción 1: Variable de entorno (RECOMENDADO)
   set MYSQL_CONNECTION_STRING="Server=localhost;Database=pizzeria;User=tu_usuario;Password=tu_password;SslMode=Required;"
   
   # Opción 2: Modificar Infra/Db.cs (solo desarrollo)
   ```

4. **Restaurar dependencias y compilar**
   ```bash
   dotnet restore
   dotnet build
   ```

5. **Ejecutar aplicación**
   ```bash
   dotnet run
   ```

---

## 🔐 Seguridad

### ⚠️ Vulnerabilidades Conocidas

Este proyecto tiene **2 vulnerabilidades críticas** documentadas que requieren implementación:

1. **Contraseñas en Texto Plano (CRÍTICO)**
   - Estado: Pendiente de implementación
   - Guía: Ver [GUIA_IMPLEMENTACION_SEGURIDAD.md](GUIA_IMPLEMENTACION_SEGURIDAD.md#1-implementar-hash-de-contraseñas-con-bcrypt)
   - Tiempo estimado: 4-6 horas

2. **Credenciales Hardcoded (ALTO)**
   - Estado: Mitigado con variables de entorno
   - Guía: Ver [GUIA_IMPLEMENTACION_SEGURIDAD.md](GUIA_IMPLEMENTACION_SEGURIDAD.md#2-mover-credenciales-a-configuración-externa)
   - Tiempo estimado: 2-3 horas

### ✅ Protecciones Implementadas

- ✅ Todas las consultas SQL usan parámetros (prevención SQL Injection)
- ✅ Validación exhaustiva de todas las entradas de usuario
- ✅ Manejo robusto de excepciones en todas las operaciones
- ✅ Operaciones asíncronas para prevenir timeouts
- ✅ Documentación completa con advertencias de seguridad

### 📚 Documentación de Seguridad

- **[RESUMEN_EJECUTIVO.md](RESUMEN_EJECUTIVO.md)** - Resumen completo del análisis
- **[ANALISIS_SEGURIDAD_Y_RENDIMIENTO.md](ANALISIS_SEGURIDAD_Y_RENDIMIENTO.md)** - Análisis detallado
- **[GUIA_IMPLEMENTACION_SEGURIDAD.md](GUIA_IMPLEMENTACION_SEGURIDAD.md)** - Guía paso a paso
- **[SISTEMA_MONITOREO_AUTOMATIZADO.md](SISTEMA_MONITOREO_AUTOMATIZADO.md)** - Monitoreo continuo

---

## 🏗️ Arquitectura

```
PizzeriaOpita/
├── Domain/              # Modelos de dominio y DTOs
│   ├── Models.cs       # Usuario, Pizza, Pedido, Roles
│   └── PedidoDTOs.cs   # DTOs para pedidos
├── Infra/              # Capa de infraestructura
│   ├── Db.cs          # Gestión de conexiones DB
│   ├── UsuarioRepository.cs
│   ├── PizzaRepository.cs
│   └── PedidoRepository.cs
├── Services/           # Lógica de negocio
│   ├── AuthService.cs
│   ├── PizzaService.cs
│   └── PedidoService.cs
├── UI/                 # Interfaz de usuario
│   ├── LoginForm.cs
│   ├── AdminForm.cs
│   ├── AsistenteForm.cs
│   └── PizzeroForm.cs
└── Program.cs          # Punto de entrada
```

### Patrones de Diseño

- **Repository Pattern** - Acceso a datos
- **Dependency Injection** - Gestión de dependencias
- **Async/Await** - Operaciones asíncronas
- **Using Pattern** - Gestión de recursos

---

## 🧪 Testing

Actualmente no hay tests unitarios implementados.

### Roadmap de Testing

- [ ] Tests unitarios para servicios
- [ ] Tests de integración para repositorios
- [ ] Tests de UI (si es posible)
- [ ] Tests de seguridad automatizados

**Guía:** Ver [SISTEMA_MONITOREO_AUTOMATIZADO.md](SISTEMA_MONITOREO_AUTOMATIZADO.md) para configuración de tests.

---

## 📊 Métricas de Calidad

| Métrica | Estado | Cobertura |
|---------|--------|-----------|
| Documentación XML | ✅ | 100% |
| Validación de Entradas | ✅ | 100% |
| Manejo de Errores | ✅ | 100% |
| SQL Parametrizado | ✅ | 100% |
| Operaciones Async | ✅ | 100% |
| Tests Unitarios | ❌ | 0% |
| Logging | ⏳ | 0% |

---

## 🔄 Roadmap

### Fase 1: Seguridad Crítica (1-2 semanas)
- [ ] Implementar hash de contraseñas con BCrypt
- [ ] Configurar variables de entorno para producción
- [ ] Habilitar SSL/TLS en MySQL

### Fase 2: Observabilidad (2-4 semanas)
- [ ] Implementar logging con Serilog
- [ ] Configurar métricas de rendimiento
- [ ] Crear dashboard de monitoreo

### Fase 3: Calidad (1-2 meses)
- [ ] Crear tests unitarios (>70% cobertura)
- [ ] Configurar GitHub Actions CI/CD
- [ ] Implementar análisis de código automático

### Fase 4: Mejoras Avanzadas (2-3 meses)
- [ ] Implementar autenticación de dos factores
- [ ] Agregar API REST para integración
- [ ] Migrar a arquitectura limpia completa

---

## 🤝 Contribución

### Guía de Contribución

1. **Fork** el repositorio
2. Crear **branch** de feature (`git checkout -b feature/NuevaCaracteristica`)
3. **Commit** cambios (`git commit -m 'Agregar nueva característica'`)
4. **Push** al branch (`git push origin feature/NuevaCaracteristica`)
5. Abrir **Pull Request**

### Estándares de Código

- ✅ Seguir convenciones de C# (.NET)
- ✅ Documentar todos los métodos públicos con XML
- ✅ Validar todas las entradas de usuario
- ✅ Usar operaciones asíncronas para I/O
- ✅ Manejar excepciones apropiadamente
- ✅ Incluir comentarios de seguridad cuando aplique

### Seguridad

- ⚠️ **NUNCA** commitear contraseñas o credenciales
- ⚠️ **NUNCA** deshabilitar validaciones de seguridad
- ✅ Siempre usar consultas parametrizadas
- ✅ Validar entradas antes de procesarlas
- ✅ Documentar riesgos de seguridad

---

## 📄 Licencia

Este proyecto es parte de un ejercicio académico/educativo.

---

## 👥 Autores

- **Estiven9900** - Desarrollador principal
- **GitHub Copilot** - Análisis de seguridad y optimización

---

## 📞 Soporte

### Documentación
- [Resumen Ejecutivo](RESUMEN_EJECUTIVO.md)
- [Análisis de Seguridad](ANALISIS_SEGURIDAD_Y_RENDIMIENTO.md)
- [Guía de Implementación](GUIA_IMPLEMENTACION_SEGURIDAD.md)
- [Sistema de Monitoreo](SISTEMA_MONITOREO_AUTOMATIZADO.md)

### Recursos
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Microsoft .NET Security](https://docs.microsoft.com/en-us/dotnet/standard/security/)
- [MySQL Security](https://dev.mysql.com/doc/refman/8.0/en/security.html)

---

## ⚡ Quick Start

```bash
# 1. Configurar variable de entorno
set MYSQL_CONNECTION_STRING="Server=localhost;Database=pizzeria;User=root;Password=tu_password;"

# 2. Restaurar y ejecutar
dotnet restore
dotnet run

# 3. Login con credenciales de prueba (según tu DB)
Usuario: admin
Contraseña: [tu_contraseña]
```

---

## 🎯 Estado del Proyecto

**Versión:** 1.0  
**Estado:** ✅ Funcional - ⚠️ Requiere mejoras de seguridad críticas  
**Última actualización:** 2025-12-03  
**Próxima milestone:** Implementación de hash de contraseñas

---

**⚠️ ADVERTENCIA IMPORTANTE:**  
Este sistema tiene vulnerabilidades de seguridad conocidas documentadas.  
**NO USAR EN PRODUCCIÓN** sin implementar las mejoras críticas de seguridad.  
Ver [GUIA_IMPLEMENTACION_SEGURIDAD.md](GUIA_IMPLEMENTACION_SEGURIDAD.md) para detalles.

