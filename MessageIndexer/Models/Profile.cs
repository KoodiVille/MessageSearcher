using System.Text.Json.Serialization;

namespace MessageIndexer.Models
{
    public class Profile
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }
}
