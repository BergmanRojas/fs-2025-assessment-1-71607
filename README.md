# fs-2025-api-20250925-71607 – Dublin Bikes & Sample API

College assignment project for the **Web API / .NET 9** module.  
The solution implements:

- A set of **sample endpoints** (Weather, Books, Courses).
- A full **Dublin Bikes API v1** backed by **in-memory data** loaded from `Data/dublinbike.json`.
- A **Dublin Bikes API v2** backed by **Azure Cosmos DB** (using the **Cosmos DB Emulator** locally).
- A small **Blazor Server UI** (Razor Components) hosted in the same project.
- **Swagger UI** documentation for exploring and testing the API.

---

## Table of Contents

1. [Technologies](#technologies)
2. [Project Structure](#project-structure)
3. [Getting Started](#getting-started)
4. [Configuration](#configuration)
5. [Running the Project](#running-the-project)
6. [API Overview](#api-overview)
   - [Courses](#courses)
   - [Dublin Bikes v1 (in-memory)](#dublin-bikes-v1-in-memory)
   - [Dublin Bikes v2 (Cosmos)](#dublin-bikes-v2-cosmos)
   - [Weather](#weather)
   - [Books](#books)
7. [Dublin Bikes Domain Models](#dublin-bikes-domain-models)
8. [Background Data Updater & Caching](#background-data-updater--caching)
9. [Swagger & Testing](#swagger--testing)
10. [Known Issues / Troubleshooting](#known-issues--troubleshooting)

---

## Technologies

- **.NET 9.0** (Minimal APIs + Razor Components / Blazor Server)
- **ASP.NET Core** 9
- **Swagger / Swashbuckle** for API documentation
- **Azure Cosmos DB** (`Microsoft.Azure.Cosmos` SDK)
- **Newtonsoft.Json** for Cosmos JSON serialization compatibility
- **In-memory caching** + background service for Dublin Bikes v1
- **Rider / Visual Studio / `dotnet` CLI** to build & run

---

##  Unit Tests

The solution includes an xUnit test project: `fs_2025_api_20250925_71607.Tests`.

Tests focus on the core business logic in `BikeStationService`:

- Filtering by status (OPEN/CLOSED)
- Text search using `q` (e.g. `q=park`)
- Minimum available bikes (`minBikes`)
- Sorting (e.g. by `bikes` desc)
- Pagination (`page`, `pageSize`)
- Aggregate summary via `GetSummary()`

To run the tests:
dotnet test

---

## Project Structure

Key folders and files:

```text
fs-2025-api-20250925-71607/
│
├── Program.cs                     // App startup (Minimal API + Blazor)
├── Startup/
│   └── Dependencies.cs            // DI registration (services, Cosmos client, etc.)
│
├── Components/                    // Blazor components (Razor)
│   ├── App.razor
│   ├── Layout/
│   └── Page/
│
├── Data/
│   ├── dublinbike.json            // Seed data for in-memory Dublin Bikes v1
│   ├── BookData.cs
│   ├── CourseData.cs
│   └── CosmosSettings.cs          // Strongly-typed config for Cosmos
│
├── Models/
│   ├── Station.cs                 // Dublin Bikes station model
│   ├── GeoPosition.cs
│   ├── BikeSummary.cs
│   ├── Book.cs
│   ├── Course.cs
│   └── WeatherForecast.cs
│
├── Endpoints/
│   ├── BikeStationEndpoints.cs    // v1 (in-memory) + v2 (Cosmos) endpoints
│   ├── BookEndpoints.cs
│   ├── CourseEndPoints.cs
│   ├── WeatherEndPoints.cs
│   └── RootEndPoints.cs
│
├── Services/
│   ├── IBikeStationService.cs     // Abstraction for Dublin Bikes v1
│   ├── BikeStationService.cs      // In-memory implementation
│   └── CosmosBikeStationService.cs// Cosmos implementation for v2
│
├── Background/
│   └── BikeDataUpdater.cs         // Background service that refreshes v1 cached data
│
└── appsettings.json               // Configuration (Cosmos, logging, etc.)
