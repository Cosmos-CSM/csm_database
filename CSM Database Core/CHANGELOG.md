# CSM Foundation Database CHANGELOG

## [2.0.0] - 24.11-2025

### Changed

- File and abstraction organizations.
- Renaming of concepts.
- Added a way to get the [Sign] directly from [IDatabase] fro Testing Classes.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 1.3.0            | 2.0.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 9.0.8           |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 9.0.8           |

## [1.2.0] - 03.09-2025

### Added

- A new [BActivableEntity] / [IActivableEntity] base added, that indicates an entity has [IsEnabled] property indicating if the object is enabled or no.

- A new [BCatalogEntity] / [ICatalogEntity] base added, that indicates an entity is catalog referenced, meaning it has [Name], [Description], [IsEnabled] and a [Reference].

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 1.3.0            | 1.3.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 9.0.8           |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 9.0.8           |

## [1.1.0] - 06.08-2025

### Added

- A new [ActivationReference] attribute was created to handle database contexts DbSet usage with interfaces setting a default reference as the activation at the database activation process.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 1.2.1            | 1.3.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 9.0.8           |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 9.0.8           |

## [1.0.0] - 06.08-2025

### Added

- Package initialization.

### Fixed

### Changed

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | --.--.--         | 1.2.1           |
| Microsoft.EntityFrameworkCore           | --.--.--         | 9.0.8           |
| Microsoft.EntityFrameworkCore.SqlServer | --.--.--         | 9.0.8           |
| xunit									  | --.--.--         | 2.9.3           |

### Removed
