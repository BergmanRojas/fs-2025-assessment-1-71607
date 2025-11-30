namespace fs_2025_api_20250925_71607.Models
{
    public class BikeSummary
    {
        public int TotalStations { get; set; }
        public int TotalBikeStands { get; set; }
        public int TotalAvailableBikes { get; set; }
        public int TotalAvailableStands { get; set; }
        public int OpenStations { get; set; }
        public int ClosedStations { get; set; }

        // Extra para lucirte (no obligatorio pero suma)
        public double AverageOccupancy { get; set; }

        public DateTimeOffset? LatestUpdateUtc { get; set; }
        public DateTimeOffset? LatestUpdateLocal { get; set; }
    }
}