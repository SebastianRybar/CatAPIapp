using System.Text.Json.Serialization;

namespace CatAPIfetcher.Model
{
    public class Weight
    {
        [JsonPropertyName("imperial")]
        public string Imperial { get; set; }

        [JsonPropertyName("metric")]
        public string Metric { get; set; }
    }
}