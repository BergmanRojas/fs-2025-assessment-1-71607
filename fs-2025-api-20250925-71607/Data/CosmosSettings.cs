namespace fs_2025_api_20250925_71607.Data
{
    public class CosmosSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = "bike";
        public string ContainerName { get; set; } = "stations";
    }
}