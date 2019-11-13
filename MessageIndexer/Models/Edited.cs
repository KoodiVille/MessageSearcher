using System.Text.Json.Serialization;

namespace MessageIndexer.Models
{
    public class Edited
    {
        public string User { get; set; }

        [JsonPropertyName("ts")]
        public string TimeStamp { get; set; }
    }
}
