using System;
using System.Collections.Generic;
using System.Text;

namespace MessageIndexer.Models
{
    public class ChannelMessages
    {
        public string Channel { get; }
        public IEnumerable<Message> Messages { get; }

        public ChannelMessages(string channel, IEnumerable<Message> messages) 
        {
            Channel = string.IsNullOrWhiteSpace(channel) ? throw new ArgumentNullException(nameof(channel)) : channel;
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        }
    }
}
