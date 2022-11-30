## 0.5
### Added
  - [NodaTime](https://github.com/nodatime/nodatime) to get accurate date between 2 dates
  - Every run will display how long it took for the runner to improve their run
### Changed
  - Added back the last digit in milliseconds
### Fixed
  - Return a special string if date played was null 

## 0.4
### Added
  - Additional rank for the non default platform
  - Display how many days it took until world record was beaten and who held it previously
### Changed
  - Renamed folder from "api" to "model"
  - Removed the last digit in milliseconds
  - Show date played as 21 Jan 2023 instead of 2023-01-21

## 0.3.2
### Fixed
  - Time difference not returning correctly if the new world record was done by same runner

## 0.3.1
### Fixed
  - NullReferenceException thrown in Runs.cs inside GetLatestData method 

## 0.3
### Added
  - Display time difference in the embed
  - Different colors on the embed if you get top 3
### Changed
  - Switched from .NET Framework 4.8 to .NET Core 6.0
### Fixed
  - Getting the correct rank now if the run has variables
  - Rounds the milliseconds correctly instead of getting the first 3 characters from the TimeSpan
### Removed
  - Loading and saving a file with the game's abbreviation and latest run id

## 0.2
### Added
  - Now sends the new verified run details to a discord webhook
  - Showing the run id and game name in different color in the console
  - Loading and saving a file with the game's abbreviation and latest run id
### Changed
  - .NET Framework 4.7.2 to .NET Framework 4.8

## 0.1
Converted from php into csharp and the new project was born. At the start it only wrote to console if it found a new run