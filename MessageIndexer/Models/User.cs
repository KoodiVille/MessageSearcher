using System.Text.Json.Serialization;

namespace MessageIndexer.Models
{
    public class User
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public Profile Profile { get; set; }

        [JsonPropertyName("tz")]
        public string TimeZone { get; set; }
    }
}
