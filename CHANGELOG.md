## 1.3.2
### Fixed
  - Time difference not returning correctly if the new world record was done by same runner (![caabffa](https://github.com/Toyro98/RunGet/commit/caabffa))

## 1.3.1
### Fixed
  - NullReferenceException thrown in Runs.cs inside GetLatestData method (![3048ed0](https://github.com/Toyro98/RunGet/commit/3048ed0)) 

## 1.3
### Added
  - Display time difference in the embed
  - Different colors on the embed if you get top 3
### Changed
  - Switched from .NET Framework 4.8 to .NET Core 6.0
### Fixed
  - Getting the correct rank now if the run has variables
  - Rounds the milliseconds correctly instead of getting the first 3 characters from the TimeSpan
