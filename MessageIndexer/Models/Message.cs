using System.Text.Json.Serialization;

namespace MessageIndexer.Models
{
    public class Message
    {
        public string Type { get; set; }

        public string User { get; set; }

        public string Text { get; set; }

        public Edited Edited { get; set; }

        [JsonPropertyName("ts")]
        public string TimeStamp { get; set; }

        [JsonPropertyName("client_msg_id")]
        public string ClientMessageId { get; set; }

    }
}
