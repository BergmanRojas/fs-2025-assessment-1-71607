using System.Text.Json.Serialization;

namespace fs_2025_api_20250925_71607.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Isbn { get; set; } = "";

        // nombre exacto pedido en el JSON
        [JsonPropertyName("publication_date")]
        public DateOnly PublicationDate { get; set; }

        public string Author { get; set; } = "";
    }
}