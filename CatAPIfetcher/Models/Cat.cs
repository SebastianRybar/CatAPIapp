using SQLite;
using System.Text.Json.Serialization;

namespace CatAPIfetcher.Model
{
    [Table("cats")]
    public class Cat
    {
        [PrimaryKey]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLocalOnly { get; set; }
        public DateTime CreatedAt { get; set; }

        [Ignore]
        [JsonPropertyName("breeds")]
        public List<Breed> Breeds { get; set; }

        [Ignore]
        public string DisplayName => !string.IsNullOrEmpty(Name)
            ? Name
            : (Breeds?.FirstOrDefault()?.Name ?? "Unknown Cat");
    }
}