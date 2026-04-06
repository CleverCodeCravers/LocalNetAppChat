# R00001 — Migration von .NET 8.0 auf .NET 10.0

**Quelle:** GitHub Issue: #30
**Typ:** Chore
**Status:** Offen

## Beschreibung

Das Projekt LocalNetAppChat soll von .NET 8.0 auf .NET 10.0 migriert werden, um den Ziel-TechStack laut Wartungsliste zu erreichen.

## Akzeptanzkriterien

1. Alle 10 `.csproj`-Dateien verwenden `<TargetFramework>net10.0</TargetFramework>`
2. NuGet-Pakete sind auf .NET 10.0-kompatible Versionen aktualisiert
3. CI/CD-Workflows (`ci.yml`, `release.yml`) verwenden .NET 10.0 SDK
4. `dotnet build` kompiliert fehlerfrei
5. Alle bestehenden Tests sind gruen
6. Keine neuen Deprecation-Warnungen (bestehende duerfen bleiben)
7. CLAUDE.md ist aktualisiert (`.NET 8.0` -> `.NET 10.0`)

## Betroffene Dateien

### Target Framework (net8.0 -> net10.0)

| Datei | NuGet-Pakete |
|-------|-------------|
| `CommandLineArguments/CommandLineArguments.csproj` | keine |
| `LocalNetAppChat.Domain/LocalNetAppChat.Domain.csproj` | keine |
| `LocalNetAppChat.Server.Domain/LocalNetAppChat.Server.Domain.csproj` | System.ServiceModel.Primitives 4.10.0 |
| `LocalNetAppChat.Cli/LocalNetAppChat.Cli.csproj` | Serilog.AspNetCore 8.0.0, Serilog.Sinks.File 5.0.0 |
| `LocalNetAppChat.Server/LocalNetAppChat.Server.csproj` | Serilog.AspNetCore 8.0.0, Serilog.Sinks.File 5.0.0 |
| `LocalNetAppChat.Bot/LocalNetAppChat.Bot.csproj` | keine |
| `LocalNetAppChat.ConsoleClient/LocalNetAppChat.ConsoleClient.csproj` | System.ServiceModel.Primitives 4.10.0 |
| `LocalNetAppChat.Bot.Tests/LocalNetAppChat.Bot.Tests.csproj` | Microsoft.NET.Test.Sdk 18.3.0, NUnit 4.5.1, NUnit3TestAdapter 4.5.0, NUnit.Analyzers 3.8.0, coverlet.msbuild 8.0.1, NSubstitute 5.3.0 |
| `LocalNetAppChat.Domain.Tests/LocalNetAppChat.Domain.Tests.csproj` | coverlet.collector 8.0.1, coverlet.msbuild 8.0.1, Microsoft.NET.Test.Sdk 18.3.0, NUnit 3.14.0, NUnit3TestAdapter 4.5.0 |
| `LocalNetAppChat.Server.Domain.Tests/LocalNetAppChat.Server.Domain.Tests.csproj` | coverlet.collector 8.0.1, coverlet.msbuild 8.0.1, Microsoft.NET.Test.Sdk 18.3.0, NUnit 3.14.0, NUnit3TestAdapter 4.5.0 |

### CI/CD Workflows

| Datei | Aenderung |
|-------|-----------|
| `.github/workflows/ci.yml` | `DOTNET_VERSION: "8.0.x"` -> `"10.0.x"` |
| `.github/workflows/release.yml` | `DOTNET_VERSION: "8.0.x"` -> `"10.0.x"` |

### Dokumentation

| Datei | Aenderung |
|-------|-----------|
| `CLAUDE.md` | `.NET 8.0` -> `.NET 10.0` |

## Teststrategie

- Bestehende Tests muessen nach der Migration weiterhin gruen sein
- Build muss fehlerfrei kompilieren
- Keine neuen Warnungen durch die Migration
