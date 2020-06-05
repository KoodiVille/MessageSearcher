using Nest;
using System;

namespace MessageIndexer.Models
{
    public class MessageIndexItem
    {
        public string Id { get; }

        [Text(Name = "user_name")]
        public string? UserName { get; }

        [Text(Name = "display_name")]
        public string? DisplayName { get; }

        [Text(Name = "channel")]
        public string ChannelName { get; }

        [Text(Name = "timestamp")]
        public DateTime Timestamp { get; }

        [Text(Name = "text")]
        public string Text { get; }

        public MessageIndexItem(string id, string userName, string displayName, string channelName, DateTime timestamp, string text)
        {
            Id = id;
            UserName = userName;
            DisplayName = displayName;
            ChannelName = channelName;
            Timestamp = timestamp;
            Text = text;
        }
    }
}
