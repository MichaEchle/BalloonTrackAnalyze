# AGENTS.md

Hot air balloon competition track analysis and scoring system. C# / .NET / Windows-only.

## Solutions

There are **two independent solutions** in this repo:

| Solution | Path | Framework | Purpose |
|----------|------|-----------|---------|
| Coordinates | `Coordinates/Coordinates.sln` | net8.0-windows (x64) | Main application suite: track parsing, scoring, GUI tools |
| Scoring | `Scoring/Scoring.slnx` | **net10.0** (preview) | Ground-up rewrite of scoring engine, zero NuGet deps, early stage |

Build each independently:
```
dotnet build Coordinates/Coordinates.sln
dotnet build Scoring/Scoring.slnx
```

## Coordinates Solution -- Project Map

Shared settings in `Coordinates/Directory.Build.props`: net8.0-windows, win-x64, nullable enabled, implicit usings, XML doc generation required, code style enforced in build.

**Executables:**
- `BalloonTrackAnalyze` -- Main WinForms GUI for configuring and scoring competition tasks
- `TrackReportGenerator` -- WinForms GUI for generating track reports
- `BLC2021` -- WinForms GUI specific to BLC 2021 competition
- `TestProgramm` -- Console app; manual batch scorer with hardcoded competition configs (NOT a test framework)
- `ResultsToScores` -- Console app; reads CSV results, calculates scores

**Libraries:**
- `Coordinates` -- Core: coordinate models, UTM conversion (CoordinateSharp), track/declaration/marker models, BalloonLive and FAI Logger parsers
- `Competition` -- Domain: flights, tasks (Donut, Pie, Elbow, LandRun, HesitationWaltz, AltitudeProfile), score calculation, penalties, validation
- `Shapes` (AreasAndShapes/) -- 2D/3D geometry: circles, polygons, cylinders, spheres
- `CoordinateConverter` -- WPF coordinate conversion utility
- Logging stack: `LoggingConnector`, `UILoggingProvider`, `WinFormsLoggerControl`, `LoggerComponent`, `LoggerFactoryProvider` -- wires Microsoft.Extensions.Logging to WinForms UI

**Ignored:** `JansScoring/` is an empty leftover directory (no source, no csproj).

## Scoring Solution (New Rewrite)

`Scoring/Scoring.slnx` targets **net10.0** with zero external dependencies. Self-contains its own Coordinates, Shapes, Converters, and Constraints modules. `Parsers/` directory is empty (IGC parsing not yet implemented). Some duplicate PZ (prohibited zone) files exist under both `Competitions/PZ/` and `Competitions/Tasks/Constraints/`.

## Code Style

Both solutions have strict `.editorconfig` files. Key conventions an agent must follow:

- **No `var`** -- always use explicit types (`csharp_style_var_*: false`)
- **Always use braces** (`csharp_prefer_braces: true`)
- **Private/internal fields:** `_camelCase` prefix (Scoring enforces at error severity)
- **Constants:** `ALL_UPPER` with underscore separators (Scoring)
- **Allman brace style** (opening brace on new line)
- **4-space indentation**, CRLF line endings, no final newline
- **File-scoped namespaces** in Scoring (`csharp_style_namespace_declarations: file_scoped:error`)
- **No expression-bodied methods/constructors/operators** -- only properties and accessors
- Scoring .editorconfig is stricter (many rules at `:error` severity)

## Testing

**No unit test projects exist.** `TestProgramm` is a manual console runner, not an automated test suite. `TestTrack/` at the repo root contains sample `.igc` files used as test data.

## Build Notes

- Windows-only: WinForms projects require Windows SDK (`net8.0-windows10.0.22621.0` for apps with SDK references)
- No CI/CD pipelines, no build scripts -- build with `dotnet build` or Visual Studio
- No README in the repo
- Key NuGet packages: CoordinateSharp (geodesic math), EPPlus (Excel export), Newtonsoft.Json, Microsoft.Extensions.Hosting/Logging
