using System.Text.Json.Serialization;

namespace fs_2025_api_20250925_71607.Models
{
    public class Station
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }

        public string? Status { get; set; }          // OPEN / CLOSED
        public int BikeStands { get; set; }          // capacidad total
        public int AvailableBikes { get; set; }      // bicis disponibles
        public int AvailableStands { get; set; }     // anclajes libres

        // ✅ Posición en objeto anidado (coincide con el JSON: "position": { "lat": ..., "lng": ... })
        public GeoPosition Position { get; set; } = new();

        // ✅ Campo crudo que viene del JSON: epoch en milisegundos
        //    "last_update": 1700018764000
        [JsonPropertyName("last_update")]
        public long LastUpdateEpochMs { get; set; }

        // 🔹 UTC calculado a partir del epoch (no hace falta que se serialice)
        [JsonIgnore]
        public DateTimeOffset LastUpdateUtc =>
            LastUpdateEpochMs > 0
                ? DateTimeOffset.FromUnixTimeMilliseconds(LastUpdateEpochMs)
                : DateTimeOffset.MinValue;

        // ✅ Campo calculado en hora local de Dublín para mostrar al cliente
        //    Esto es lo que el docente quiere ver como "LastUpdateLocal" (Europe/Dublin)
        [JsonPropertyName("lastUpdateLocal")]
        public DateTimeOffset LastUpdateLocal
        {
            get
            {
                if (LastUpdateEpochMs <= 0)
                {
                    return DateTimeOffset.MinValue;
                }

                var utc = LastUpdateUtc;

                try
                {
                    // En Windows el ID suele ser distinto, por eso cubrimos los dos casos
                    var tzId = OperatingSystem.IsWindows()
                        ? "GMT Standard Time"   // zona equivalente a Irlanda/Reino Unido
                        : "Europe/Dublin";

                    var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
                    return TimeZoneInfo.ConvertTime(utc, tz);
                }
                catch
                {
                    // Si por algún motivo no se encuentra la zona, caemos al local del servidor
                    return utc.ToLocalTime();
                }
            }
        }
    }
}