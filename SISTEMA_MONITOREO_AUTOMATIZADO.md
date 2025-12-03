# 🤖 Sistema de Monitoreo y Alertas Automatizadas

Este documento describe un sistema de monitoreo pasivo para detectar problemas de seguridad, rendimiento y calidad de código en el proyecto Pizzería Opita.

## 📋 Índice

1. [GitHub Actions para CI/CD](#github-actions-para-cicd)
2. [Análisis de Código Estático](#análisis-de-código-estático)
3. [Escaneo de Dependencias](#escaneo-de-dependencias)
4. [Métricas de Rendimiento](#métricas-de-rendimiento)
5. [Reportes Automáticos](#reportes-automáticos)

---

## 1. GitHub Actions para CI/CD

### Workflow Principal de Análisis

Crear `.github/workflows/security-analysis.yml`:

```yaml
name: Security and Quality Analysis

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]
  schedule:
    # Ejecutar análisis diario a las 2 AM
    - cron: '0 2 * * *'

jobs:
  security-scan:
    name: Security Vulnerability Scan
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Security Scan - dotnet list package
      run: dotnet list package --vulnerable --include-transitive 2>&1 | tee security-scan.txt
      continue-on-error: true
    
    - name: Check for vulnerabilities
      run: |
        if grep -q "has the following vulnerable packages" security-scan.txt; then
          echo "⚠️ ALERTA: Dependencias vulnerables detectadas"
          cat security-scan.txt
          exit 1
        fi
    
    - name: Upload Security Report
      if: always()
      uses: actions/upload-artifact@v3
      with:
        name: security-scan-report
        path: security-scan.txt

  code-quality:
    name: Code Quality Analysis
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Run .NET Analyzers
      run: dotnet build --no-restore /p:TreatWarningsAsErrors=false /p:WarningLevel=4

  dependency-review:
    name: Dependency Review
    runs-on: ubuntu-latest
    if: github.event_name == 'pull_request'
    
    steps:
    - uses: actions/checkout@v3
    - uses: actions/dependency-review-action@v3
      with:
        fail-on-severity: moderate
```

### Workflow de CodeQL

Crear `.github/workflows/codeql-analysis.yml`:

```yaml
name: CodeQL Security Analysis

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]
  schedule:
    - cron: '0 3 * * 1' # Lunes a las 3 AM

jobs:
  analyze:
    name: Analyze C#
    runs-on: ubuntu-latest
    permissions:
      actions: read
      contents: read
      security-events: write

    steps:
    - name: Checkout repository
      uses: actions/checkout@v3

    - name: Initialize CodeQL
      uses: github/codeql-action/init@v2
      with:
        languages: csharp
        queries: security-extended,security-and-quality

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore --configuration Release

    - name: Perform CodeQL Analysis
      uses: github/codeql-action/analyze@v2
      with:
        category: "/language:csharp"
```

---

## 2. Análisis de Código Estático

### Configurar Roslyn Analyzers

Actualizar `PizzeriaOpita.App.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0-windows7.0</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <OutputType>WinExe</OutputType>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- ✅ Análisis de código habilitado -->
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MaterialSkin.2" Version="2.3.1" />
    <PackageReference Include="MySqlConnector" Version="2.3.7" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="9.0.0" />
    
    <!-- ✅ Analyzers de seguridad y calidad -->
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="SecurityCodeScan.VS2019" Version="5.6.7">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="SonarAnalyzer.CSharp" Version="9.16.0.82469">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

### Crear .editorconfig para Reglas de Estilo

Crear `.editorconfig`:

```ini
# ✅ Configuración de estilo y análisis de código
root = true

[*.cs]
# Indentación
indent_style = space
indent_size = 4
tab_width = 4

# Líneas nuevas
end_of_line = crlf
insert_final_newline = true

# Organización de usings
dotnet_sort_system_directives_first = true
dotnet_separate_import_directive_groups = false

# ✅ Reglas de seguridad
dotnet_diagnostic.CA2100.severity = error  # SQL Injection
dotnet_diagnostic.CA5350.severity = error  # Weak cryptography
dotnet_diagnostic.CA5351.severity = error  # Broken cryptography
dotnet_diagnostic.CA3075.severity = error  # Insecure DTD processing
dotnet_diagnostic.CA3147.severity = error  # Validate CSRF tokens

# ✅ Reglas de rendimiento
dotnet_diagnostic.CA1806.severity = warning  # Do not ignore method results
dotnet_diagnostic.CA1810.severity = warning  # Initialize static fields inline
dotnet_diagnostic.CA1822.severity = suggestion  # Mark members as static
dotnet_diagnostic.CA1825.severity = warning  # Avoid zero-length array allocations

# ✅ Reglas de mejores prácticas
dotnet_diagnostic.CA1031.severity = suggestion  # Do not catch general exception types
dotnet_diagnostic.CA1062.severity = suggestion  # Validate arguments of public methods
dotnet_diagnostic.CA1303.severity = none  # Do not pass literals as localized parameters
dotnet_diagnostic.CA2007.severity = none  # ConfigureAwait (not needed in WinForms)
```

---

## 3. Escaneo de Dependencias

### Workflow de Dependabot

Crear `.github/dependabot.yml`:

```yaml
version: 2
updates:
  # ✅ Mantener paquetes NuGet actualizados
  - package-ecosystem: "nuget"
    directory: "/"
    schedule:
      interval: "weekly"
      day: "monday"
      time: "09:00"
    open-pull-requests-limit: 10
    reviewers:
      - "Estiven9900"
    labels:
      - "dependencies"
      - "security"
    commit-message:
      prefix: "deps"
      prefix-development: "deps-dev"
    
    # ✅ Alertas de seguridad tienen prioridad
    allow:
      - dependency-type: "all"
    
    # Agrupar actualizaciones menores
    groups:
      minor-updates:
        patterns:
          - "*"
        update-types:
          - "minor"
          - "patch"

  # ✅ Mantener GitHub Actions actualizadas
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "monthly"
    labels:
      - "dependencies"
      - "github-actions"
```

### Script de Verificación Manual

Crear `scripts/check-vulnerabilities.ps1`:

```powershell
# ✅ Script de verificación de vulnerabilidades

Write-Host "🔍 Verificando vulnerabilidades en dependencias..." -ForegroundColor Cyan

# Verificar paquetes vulnerables
Write-Host "`n📦 Analizando paquetes NuGet..." -ForegroundColor Yellow
dotnet list package --vulnerable --include-transitive

# Verificar versiones desactualizadas
Write-Host "`n📊 Verificando actualizaciones disponibles..." -ForegroundColor Yellow
dotnet list package --outdated

# Restaurar y compilar para verificar analyzers
Write-Host "`n🔨 Compilando con análisis de código..." -ForegroundColor Yellow
dotnet build /p:TreatWarningsAsErrors=false /p:WarningLevel=4

Write-Host "`n✅ Análisis completado" -ForegroundColor Green
```

---

## 4. Métricas de Rendimiento

### Monitoreo de Consultas SQL

Crear `Infra/QueryMetrics.cs`:

```csharp
using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace PizzeriaOpita.App.Infra
{
    /// <summary>
    /// Utilidad para medir y registrar el rendimiento de consultas SQL.
    /// </summary>
    public class QueryMetrics
    {
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch;
        private readonly string _queryName;

        public QueryMetrics(ILogger logger, string queryName)
        {
            _logger = logger;
            _queryName = queryName;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Complete(int rowsAffected = 0)
        {
            _stopwatch.Stop();
            var elapsedMs = _stopwatch.ElapsedMilliseconds;

            if (elapsedMs > 1000) // Más de 1 segundo
            {
                _logger.LogWarning(
                    "⚠️ Consulta lenta detectada: {QueryName} tomó {ElapsedMs}ms (Filas: {RowsAffected})",
                    _queryName, elapsedMs, rowsAffected);
            }
            else
            {
                _logger.LogDebug(
                    "Consulta ejecutada: {QueryName} en {ElapsedMs}ms (Filas: {RowsAffected})",
                    _queryName, elapsedMs, rowsAffected);
            }
        }
    }
}
```

Uso en repositorios:

```csharp
public async Task<List<Pizza>> GetAll()
{
    var pizzas = new List<Pizza>();
    
    using var metrics = new QueryMetrics(_logger, "PizzaRepository.GetAll");
    
    try
    {
        using var conn = Db.Get();
        await conn.OpenAsync();
        
        using var cmd = new MySqlCommand("SELECT IdPizza, Nombre, Precio FROM Pizzas", conn);
        using var reader = await cmd.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            pizzas.Add(new Pizza { /* ... */ });
        }
        
        metrics.Complete(pizzas.Count);
    }
    catch (MySqlException ex)
    {
        throw new InvalidOperationException($"Error de base de datos: {ex.Message}", ex);
    }
    
    return pizzas;
}
```

---

## 5. Reportes Automáticos

### GitHub Action para Reporte Semanal

Crear `.github/workflows/weekly-report.yml`:

```yaml
name: Weekly Security Report

on:
  schedule:
    # Cada lunes a las 9 AM
    - cron: '0 9 * * 1'
  workflow_dispatch: # Permitir ejecución manual

jobs:
  generate-report:
    name: Generate Weekly Report
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Generate Security Report
      run: |
        echo "# 📊 Reporte Semanal de Seguridad - $(date +%Y-%m-%d)" > weekly-report.md
        echo "" >> weekly-report.md
        
        echo "## 🔍 Escaneo de Vulnerabilidades" >> weekly-report.md
        dotnet list package --vulnerable --include-transitive >> weekly-report.md || echo "✅ No se detectaron vulnerabilidades" >> weekly-report.md
        
        echo "" >> weekly-report.md
        echo "## 📦 Paquetes Desactualizados" >> weekly-report.md
        dotnet list package --outdated >> weekly-report.md || echo "✅ Todos los paquetes actualizados" >> weekly-report.md
        
        echo "" >> weekly-report.md
        echo "## 🏗️ Estado de Build" >> weekly-report.md
        dotnet build --configuration Release 2>&1 | tail -10 >> weekly-report.md
    
    - name: Create Issue with Report
      uses: peter-evans/create-issue-from-file@v4
      with:
        title: "📊 Reporte Semanal de Seguridad - ${{ github.event.repository.updated_at }}"
        content-filepath: weekly-report.md
        labels: |
          security
          automated-report
          weekly
```

### Plantilla de Issue para Vulnerabilidades

Crear `.github/ISSUE_TEMPLATE/security-vulnerability.md`:

```markdown
---
name: 🔒 Vulnerabilidad de Seguridad
about: Reportar una vulnerabilidad de seguridad detectada
title: '[SECURITY] '
labels: security, high-priority
assignees: Estiven9900
---

## 🔴 Descripción de la Vulnerabilidad

**Severidad:** [CRÍTICA / ALTA / MEDIA / BAJA]

**Componente afectado:**
- [ ] AuthService
- [ ] Base de datos
- [ ] Configuración
- [ ] Dependencias
- [ ] Otro: ___________

**Descripción:**
[Describe la vulnerabilidad detectada]

## 🎯 Impacto

**¿Qué información o funcionalidad está en riesgo?**

**¿Cómo podría ser explotada esta vulnerabilidad?**

## ✅ Recomendación

**¿Qué se debe hacer para mitigar esta vulnerabilidad?**

## 📚 Referencias

- [Enlace a documentación]
- [CVE si aplica]
- [OWASP referencia]

## 🕒 Timeline

- Detectado: [Fecha]
- Prioridad de resolución: [INMEDIATO / 1 semana / 1 mes]
```

---

## 🔔 Notificaciones y Alertas

### Configurar Notificaciones de GitHub

1. **Ir a Settings → Notifications**
2. **Habilitar:**
   - Dependabot alerts
   - Security vulnerability alerts
   - Code scanning alerts

### Integración con Slack (Opcional)

Crear `.github/workflows/slack-notifications.yml`:

```yaml
name: Slack Notifications

on:
  issues:
    types: [opened]
  pull_request:
    types: [opened]

jobs:
  notify:
    runs-on: ubuntu-latest
    if: contains(github.event.issue.labels.*.name, 'security') || contains(github.event.pull_request.labels.*.name, 'security')
    
    steps:
    - name: Send Slack Notification
      uses: slackapi/slack-github-action@v1
      with:
        payload: |
          {
            "text": "🔒 Nueva alerta de seguridad en Pizzería Opita",
            "blocks": [
              {
                "type": "section",
                "text": {
                  "type": "mrkdwn",
                  "text": "*Nueva alerta de seguridad*\n${{ github.event.issue.title || github.event.pull_request.title }}\n<${{ github.event.issue.html_url || github.event.pull_request.html_url }}|Ver detalles>"
                }
              }
            ]
          }
      env:
        SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}
```

---

## 📝 Checklist de Configuración

- [ ] **GitHub Actions**
  - [ ] Crear workflow de security-analysis.yml
  - [ ] Crear workflow de codeql-analysis.yml
  - [ ] Crear workflow de weekly-report.yml
  - [ ] Verificar ejecución correcta

- [ ] **Analyzers**
  - [ ] Actualizar .csproj con analyzers
  - [ ] Crear .editorconfig
  - [ ] Compilar y verificar warnings

- [ ] **Dependabot**
  - [ ] Crear dependabot.yml
  - [ ] Habilitar Dependabot alerts en GitHub
  - [ ] Verificar PRs automáticos

- [ ] **Reportes**
  - [ ] Configurar issue templates
  - [ ] Habilitar notificaciones
  - [ ] Verificar generación de reportes

- [ ] **Monitoreo**
  - [ ] Implementar QueryMetrics
  - [ ] Agregar logging en servicios
  - [ ] Configurar niveles de log

---

## 🎯 Ejemplos de Alertas

### Alerta de Dependencia Vulnerable

```
⚠️ ALERTA DE SEGURIDAD

Paquete: MySqlConnector 2.3.7
Vulnerabilidad: CVE-2024-XXXXX
Severidad: ALTA

Descripción: Potencial SQL injection en versiones < 2.3.8

Acción requerida:
dotnet add package MySqlConnector --version 2.3.8

PR automático creado: #123
```

### Alerta de Consulta Lenta

```
⚠️ ALERTA DE RENDIMIENTO

Query: PedidoRepository.ListAsync
Tiempo: 2,350ms
Filas: 150

Recomendación: Considerar agregar índice en columna 'estado'
```

---

**Última actualización:** 2025-12-03  
**Mantenedor:** Equipo DevOps Pizzería Opita
