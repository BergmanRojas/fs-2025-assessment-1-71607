using System.Collections.Generic;
using System.Linq;
using fs_2025_api_20250925_71607.Models;
using fs_2025_api_20250925_71607.Services;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace fs_2025_api_20250925_71607.Tests.Services
{
    public class BikeStationServiceTests
    {
        // Helper para crear un servicio con datos de prueba en memoria
        private BikeStationService CreateService()
        {
            var stations = new List<Station>
            {
                new()
                {
                    Number = 1,
                    Name = "CLARENDON ROW",
                    Address = "Clarendon Row",
                    Status = "OPEN",
                    BikeStands = 24,
                    AvailableBikes = 10,
                    AvailableStands = 14
                },
                new()
                {
                    Number = 2,
                    Name = "PARK STATION",
                    Address = "Park Street",
                    Status = "OPEN",
                    BikeStands = 20,
                    AvailableBikes = 5,
                    AvailableStands = 15
                },
                new()
                {
                    Number = 3,
                    Name = "CLOSED STATION",
                    Address = "Hidden Street",
                    Status = "CLOSED",
                    BikeStands = 10,
                    AvailableBikes = 0,
                    AvailableStands = 10
                }
            };

            var cache = new MemoryCache(new MemoryCacheOptions());
            return new BikeStationService(stations, cache);
        }

        [Fact]
        public void GetStations_FilterByStatus_OpenOnly()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = service.GetStations(
                q: null,
                status: "OPEN",
                minBikes: null,
                sort: null,
                dir: null,
                page: 1,
                pageSize: 10,
                out var totalCount);

            // Assert
            Assert.Equal(2, totalCount);
            Assert.All(result, s => Assert.Equal("OPEN", s.Status));
        }

        [Fact]
        public void GetStations_SearchByName_ContainsPark()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = service.GetStations(
                q: "park",
                status: null,
                minBikes: null,
                sort: null,
                dir: null,
                page: 1,
                pageSize: 10,
                out var totalCount);

            // Assert
            Assert.Equal(1, totalCount);
            var station = Assert.Single(result);
            Assert.Contains("PARK", station.Name);
        }

        [Fact]
        public void GetStations_FilterByMinBikes_OnlyStationsWithEnoughBikes()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = service.GetStations(
                q: null,
                status: null,
                minBikes: 5,
                sort: null,
                dir: null,
                page: 1,
                pageSize: 10,
                out var totalCount);

            // Assert
            Assert.Equal(2, totalCount); // 10 y 5
            Assert.All(result, s => Assert.True(s.AvailableBikes >= 5));
        }

        [Fact]
        public void GetStations_SortByBikes_Desc()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = service.GetStations(
                q: null,
                status: null,
                minBikes: null,
                sort: "bikes",
                dir: "desc",
                page: 1,
                pageSize: 10,
                out var totalCount);

            // Assert
            var list = result.ToList();
            Assert.Equal(3, totalCount);
            Assert.Equal(3, list.Count);

            // 10, 5, 0
            Assert.True(list[0].AvailableBikes >= list[1].AvailableBikes);
            Assert.True(list[1].AvailableBikes >= list[2].AvailableBikes);
        }

        [Fact]
        public void GetStations_Pagination_Works()
        {
            // Arrange
            var service = CreateService();

            // Act
            var page1 = service.GetStations(
                q: null,
                status: null,
                minBikes: null,
                sort: "number",
                dir: "asc",
                page: 1,
                pageSize: 2,
                out var totalCountPage1);

            var page2 = service.GetStations(
                q: null,
                status: null,
                minBikes: null,
                sort: "number",
                dir: "asc",
                page: 2,
                pageSize: 2,
                out var totalCountPage2);

            // Assert
            Assert.Equal(3, totalCountPage1);
            Assert.Equal(3, totalCountPage2);

            var list1 = page1.ToList();
            var list2 = page2.ToList();

            Assert.Equal(2, list1.Count);
            Assert.Single(list2);

            Assert.Equal(1, list1[0].Number);
            Assert.Equal(2, list1[1].Number);
            Assert.Equal(3, list2[0].Number);
        }

        [Fact]
        public void GetSummary_ReturnsAggregatedValues()
        {
            // Arrange
            var service = CreateService();

            // Act
            var summary = service.GetSummary();

            // Assert
            Assert.Equal(3, summary.TotalStations);        // 3 estaciones
            Assert.Equal(54, summary.TotalBikeStands);     // 24 + 20 + 10
            Assert.Equal(15, summary.TotalAvailableBikes); // 10 + 5 + 0
            Assert.Equal(39, summary.TotalAvailableStands);// 14 + 15 + 10
            Assert.Equal(2, summary.OpenStations);         // 2 OPEN
            Assert.Equal(1, summary.ClosedStations);       // 1 CLOSED
        }
    }
}