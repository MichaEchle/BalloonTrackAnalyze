# AGENTS.md

Tools set for for scoring in hot air balloon coompetitions. C# / .NET

# Solution

This solution is a rework and future replace for the other solution in this repository `Coordinates/Coordinates.sln`.
The target is to get a more integrated yet more flexible workflow.

- **Solution file:** `Scoring.slnx` (single project: `Scoring/Scoring.csproj`)
- **Framework:** `net10.0` (preview)
- **Zero NuGet dependencies** -- all math, UTM conversion, and geometry is self-contained
- **Nullable reference types:** enabled
- **Implicit usings:** enabled
- Build with `dotnet build Scoring/Scoring.slnx`

# Context

In Hot Air Balloon competitions, pilots performs one or more flights, each fight has a set of tasks.
Constraints and rules can apply on competition level, like the method for tracking altiude (GPS vs barometric), on flight level, like order of tasks, and on task level for eaxmple time limits.
Pilots can declare goals on their own, the time and position at time of declaration is important for scoring as well as the declared goal itself.
Pilots can also drop markers, both electronically and well as physical markers on the ground, which can be used for scoring as well. Physical markers take precedence over electronics ones.
The main input is a .igc track files that contains a header with useful information and then track points with time, position and altitude. And also contains the declaration and marker drop information.

Calculations are done in latitude and longitude, yet pilots declare in UTM as it is more human friendly, so a conversion is needed. The scoring system must be able to convert between the two formats.
Alitudes are also often defined and declared in feet, for calculations they must be converted to meters.

Scoring is done in phases.
1. Create flight and enter task setup
1. Parse the track files
1. Calculate the scores for each task, taking into account the constraints and rules
1. Double the check the results are reasonable, and if not, investigate the cause and fix the issue
1. Check for global incidents like flying in prohited zones, dangerous flying (based on proximity to other ballons and general rates of ascendence and decendence) 
1. Deal with mistakes pilots make, e.g. using the wrong goal number or dropping a differnet marker than stated in the task

# References
The following documents contain definitions, rules and regulations for hot air balloon competitions, and are used as reference for the implementation of the scoring system:
- `./coh_2024_-_final.pdf`
- `./axmer2024.pdf` 

# Directory Structure

```
Scoring/
  .editorconfig
  AGENTS.md
  Scoring.slnx
  Tasks.xlsx                          # Excel spreadsheet with task definitions
  axmer2024.pdf                       # Competition rules reference
  coh_2024_-_final.pdf                # CIA/FAI rules reference
  Scoring/
    Scoring.csproj
    Competitions/
      Competition.cs                  # Singleton root, enums: AltitudeSourceType, CommonLaunchPointType
      Flights/                        # EMPTY -- placeholder for future work
      Penalties/                      # EMPTY -- placeholder for future work
      Scorings/                       # EMPTY -- placeholder for future work
      Pilots/
        Pilot.cs
      PZ/
        ProhibitedZone.cs             # enum ProhibitedZoneType + class ProhibitedZone
        ProhibitedZoneViolation.cs
        ProhibtedZoneChecker.cs       # typo in filename: missing 'i' in "Prohibited"
      Tasks/
        Constraints/
          IConstraint.cs              # IConstraint<T> + enums: EvaluationOrderType, InfringementActionType, ReturnCriteriaType
          BaseConstraint.cs           # Template Method base class
          AltitudeConstraint.cs       # Partial -- PenaltyCalculation not implemented
          Distance2DConstraint.cs     # Partial -- PenaltyCalculation not implemented
          Distance3DConstraint.cs     # Partial -- PenaltyCalculation not implemented
          DurationConstraint.cs       # STUB -- empty class
          GridLineConstraint.cs       # STUB -- empty class
          OrConstraint.cs             # STUB -- empty class
          TiminingConstraint.cs       # STUB -- typo: should be "Timing"
          WithinShape2DConstraint.cs  # STUB
          WithinShape3DConstraint.cs  # STUB -- class name casing: "WithInShape3DConstraint"
          ProhibitedZone.cs           # DUPLICATE of ../PZ/ProhibitedZone.cs (different namespace)
          ProhibitedZoneViolation.cs  # DUPLICATE of ../PZ/ProhibitedZoneViolation.cs
          ProhibtedZoneChecker.cs     # DUPLICATE of ../PZ/ProhibtedZoneChecker.cs
      Tracks/
        Declaration.cs
        MarkerDrop.cs
        Track.cs
    Converters/
      AltitudeConverter.cs
      CoordinateSystemConverter.cs
      Ellipsoid.cs
    Coordinates/
      Coordinate.cs
      CoordinateMath.cs
    Parsers/                          # EMPTY -- IGC parsing not yet implemented
    Shapes/
      GridLine/
        GridLine.cs
      Shapes2D/
        Shapes2D.cs                   # Abstract base for 2D shapes
        Circle.cs
        Polygon.cs
      Shapes3D/
        Shapes3D.cs                   # Abstract base for 3D shapes
        Cylinder.cs
        Sphere.cs
        UniformPrism.cs
```

# Architecture

## Module Overview

| Module | Namespace Root | Purpose |
|--------|---------------|---------|
| Coordinates | `Scoring.Coordinates` | Core coordinate type + geodesic math (Haversine, bearing, angle, area) |
| Converters | `Scoring.Converters` | Altitude (ft/m, QNH), UTM<->LatLon, Ellipsoid definitions |
| Shapes | `Scoring.Shapes` | 2D (Circle, Polygon) and 3D (Cylinder, Sphere, UniformPrism) geometry with point-in-shape tests |
| Competitions | `Scoring.Competitions` | Domain: Competition singleton, Pilots, Tracks, Declarations, Markers, PZ checking, Task constraints |
| Parsers | (empty) | Future: IGC file parsing |

## Key Design Patterns

**Singleton Competition root:** `Competition.Instance` is the global entry point. Created via `Competition.Create(...)` factory. Uses C# 13 `field` keyword for semi-auto property. Referenced by `CoordinateMath` and `CoordinateSystemConverter` for the active ellipsoid (WGS84 or GRS80).

**Immutable data objects:** `Coordinate`, `Declaration`, `MarkerDrop`, `Pilot`, `ProhibitedZone` all use `required` + `init`-only setters. Treat these as value-like objects -- do not add mutable state.

**Shape hierarchy:** Clean 2D/3D abstraction. 3D shapes compose 2D shapes (Cylinder wraps Circle, UniformPrism wraps Polygon). Both abstract bases provide `IsCoordinateInside()` and distance-within-shape calculations.

**Template Method for Constraints:** `BaseConstraint<T>` provides a generic evaluation loop driven by `EvaluationOrder`, `ReturnCriteria`, and `InfringementAction` enums. Concrete constraints supply `InfringementCheck` and `PenaltyCalculation` lambdas.

## Type Quick Reference

### Enums
| Enum | Namespace | Values |
|------|-----------|--------|
| `AltitudeSourceType` | `Scoring.Competitions` | GPS, Barometric |
| `CommonLaunchPointType` | `Scoring.Competitions` | CLP_A through CLP_E |
| `ProhibitedZoneType` | `Scoring.Competitions.PZ` | Blue, Yellow, Red |
| `EvaluationOrderType` | `Scoring.Competitions.Tasks.Constraints` | First, Last |
| `InfringementActionType` | `Scoring.Competitions.Tasks.Constraints` | InfringementInvalidates, InfringementPenalizes |
| `ReturnCriteriaType` | `Scoring.Competitions.Tasks.Constraints` | WithoutInfringement, WithInfringement |

### Core Types
| Type | Namespace | Kind | Notes |
|------|-----------|------|-------|
| `Coordinate` | `Scoring.Coordinates` | class | Lat/Lon/Alt/optional Timestamp, immutable |
| `CoordinateMath` | `Scoring.Coordinates` | static class | Haversine 2D/3D distance, bearing, angle, area |
| `Ellipsoid` | `Scoring.Converters` | class | Immutable; factory via `Ellipsoid.WGS84` / `Ellipsoid.GRS80` |
| `AltitudeConverter` | `Scoring.Converters` | static class | ft<->m, QNH correction (linear + barometric) |
| `CoordinateSystemConverter` | `Scoring.Converters` | class (internal) | UTM<->LatLon, self-contained (no external lib) |

### Domain Types
| Type | Namespace | Kind | Notes |
|------|-----------|------|-------|
| `Competition` | `Scoring.Competitions` | class | Singleton root, holds pilots/PZs/launch points/ellipsoid |
| `Pilot` | `Scoring.Competitions.Pilots` | class | Name, number, identifier; immutable |
| `Track` | `Scoring.Competitions.Tracks` | class | Track points + declarations + marker drops for one pilot |
| `Declaration` | `Scoring.Competitions.Tracks` | class | Declared goal with UTM originals; immutable |
| `MarkerDrop` | `Scoring.Competitions.Tracks` | class | Marker number + location; immutable |

### Prohibited Zones
| Type | Namespace | Kind | Notes |
|------|-----------|------|-------|
| `ProhibitedZone` | `Scoring.Competitions.PZ` | class | Shape-based zone (Blue/Yellow/Red) |
| `ProhibitedZoneViolation` | `Scoring.Competitions.PZ` | class (internal) | Violation result: coords, time, penalties |
| `ProhibtedZoneChecker` | `Scoring.Competitions.PZ` | static class (internal) | Checks tracks against PZ list |

### Constraints
| Type | Namespace | Kind | Status |
|------|-----------|------|--------|
| `IConstraint<T>` | `Scoring.Competitions.Tasks.Constraints` | interface | Complete |
| `BaseConstraint<T>` | `Scoring.Competitions.Tasks.Constraints` | abstract class | Complete (Template Method) |
| `AltitudeConstraint` | `Scoring.Competitions.Tasks.Constraints` | class (internal) | Partial -- PenaltyCalculation not implemented |
| `Distance2DConstraint` | `Scoring.Competitions.Tasks.Constraints` | class | Partial -- PenaltyCalculation not implemented |
| `Distance3DConstraint` | `Scoring.Competitions.Tasks.Constraints` | class (internal) | Partial -- PenaltyCalculation not implemented |
| `DurationConstraint` | `Scoring.Competitions.Tasks.Constraints` | class (internal) | Stub |
| `GridLineConstraint` | `Scoring.Competitions.Tasks.Constraints` | class (internal) | Stub |
| `OrConstraint` | `Scoring.Competitions.Tasks.Constraints` | class (internal) | Stub |
| `TiminingConstraint` | `Scoring.Competitions.Tasks.Constraints` | class (internal) | Stub |
| `WithinShape2DConstraint` | `Scoring.Competitions.Tasks.Constraints` | class (internal) | Stub |
| `WithInShape3DConstraint` | `Scoring.Competitions.Tasks.Constraints` | class (internal) | Stub |

### Shapes
| Type | Namespace | Kind | Notes |
|------|-----------|------|-------|
| `Shapes2D` | `Scoring.Shapes` | abstract class | `IsCoordinateInside()` + distance-within |
| `Circle` | `Scoring.Shapes` | class | Center + radius; Haversine distance check |
| `Polygon` | `Scoring.Shapes` | class | Winding number algorithm; shoelace area |
| `Shapes3D` | `Scoring.Shapes` | abstract class | `IsCoordinateInside()` + altitude boundary check |
| `Cylinder` | `Scoring.Shapes` | class | Circle + lower/upper altitude bounds |
| `Sphere` | `Scoring.Shapes` | class | Center + radius; 3D distance check |
| `UniformPrism` | `Scoring.Shapes` | class | Polygon + lower/upper altitude bounds |
| `GridLine` | `Scoring.Shapes.GridLine` | class (internal) | Cardinal direction tests + distance to grid axes |

# Code Style (enforced by .editorconfig at error severity)

- **No `var`** -- always use explicit types
- **Always use braces** for control flow
- **File-scoped namespaces** (`namespace Foo;`)
- **Allman brace style** (opening brace on new line)
- **4-space indentation**, CRLF line endings, no final newline
- **No expression-bodied methods/constructors/operators/lambdas/local-functions** -- only properties and accessors may use expression bodies
- **Primary constructors preferred** where applicable
- **Naming conventions:**
  - Interfaces: `IFoo` (I prefix + PascalCase)
  - Types and non-field members: PascalCase
  - Constants: `ALL_UPPER` with underscores
  - Private/internal fields: `_camelCase` (underscore prefix)
  - Locals and parameters: camelCase
  - Public/protected fields: PascalCase
- **No `this.` qualification**
- Language keywords over BCL types (`int` not `Int32`, `string` not `String`)
- Pattern matching preferred over `as`/`is` with cast
- `using` directives outside namespace, System first

# Testing

**No test projects exist.** There are no unit tests in this solution. The sibling `TestTrack/` directory at repo root contains sample `.igc` files.

# Units and Conventions

- **Distances:** meters (internal), but pilots declare in UTM grid units (meters) and sometimes feet for altitude
- **Altitudes:** meters internally; feet used in pilot-facing contexts and some penalty calculations
- **Coordinates:** WGS84 latitude/longitude internally; UTM for pilot declarations
- **Time:** `DateTime` timestamps on track points; `TimeSpan` for durations
- **Angles:** degrees (bearing, interior angles)
- **QNH:** hectopascals (hPa), valid range 800-1200
- **NaN sentinel:** `double.NaN` is used to mean "no limit" / "not set" for altitude boundaries and constraint thresholds