using System.Text.Json.Serialization;

namespace MessageIndexer.Models
{
    public class Topic
    {
        public string Value { get; set; }
        public string Creator { get; set; }

        [JsonPropertyName("last_set")]
        public float LastSet { get; set; }
    }
}
