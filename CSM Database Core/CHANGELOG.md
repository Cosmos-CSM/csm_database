# CSM Database CHANGELOG

## [4.3.0] - 16.02-2026

### Fixed

- Added [EntityBase{IEntity}] to correctly implement [Clone] features from [ObjectBase].

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.1.0            | 2.1.0           |
| Microsoft.EntityFrameworkCore           | 10.0.3           | 10.0.3          |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.3           | 10.0.3          |

## [4.2.3] - 13.02-2026

### Fixed

- Fixed an issue with [EntityError] that was not passing the [Exception] caught to its base, causing error handling errors.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.1.0           |
| Microsoft.EntityFrameworkCore           | 10.0.1           | 10.0.3          |
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.1           | 10.0.3          |

## [4.2.0] - 29.01-2026

### Patched

- Patched an error that was throwing errors about never finding connection options for Databases when searching at application assemblies.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.1.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 10.0.1          |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 10.0.1          |

## [4.1.2] - 24.12-2025

### Patched

- Patched connection options tracing when the file is configured at machine / user environment variables.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.1.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 10.0.1          |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 10.0.1          |

## [4.1.1] - 22.12-2025

### Patched

- Patched the way DatabaseUtils were locating connection options automatically for database context building, now using the correct application directory and not the assembly directory.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.0.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 10.0.1          |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 10.0.1          |

## [4.1.0] - 22.12-2025

### Added

- Added a base class for database design ef context handling.

### Changed

- Updated packages and framework.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.0.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 10.0.1          |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 10.0.1          |

## [4.0.0] - 11.12-2025

### Changed

- Reverted and removed version [3.0.0] interface concept is not needed in this abstraction layer.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.0.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 9.0.8           |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 9.0.8           |

## [3.0.0] - 26.11-2025

### Changed

- Now DepotBase operations are based on interfaces trading instead of object classes.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.0.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 9.0.8           |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 9.0.8           |

## [2.1.0] - 26.11-2025

### Changed

- Database activation based on testing purposes.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.0.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 9.0.8           |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 9.0.8           |


## [2.0.1] - 25.11-2025

### Changed

- Repository name.

#### Dependencies

| Package                                 | Previous Version | New Version     |
|:----------------------------------------|:----------------:|:---------------:|
| CSM.Foundation.Core                     | 2.0.0            | 2.0.0           |
| Microsoft.EntityFrameworkCore           | 9.0.8            | 9.0.8           |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.8            | 9.0.8           |

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
