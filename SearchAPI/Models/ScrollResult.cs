using System;
using System.Collections.Generic;

namespace SearchAPI.Models
{
    public class ScrollResult
    {
        public IEnumerable<Message> Messages { get; }
        public string ScrollId { get; }
        public ScrollResult(IEnumerable<Message> messages, string scrollId)
        {
            Messages = messages ?? throw new ArgumentNullException(nameof(messages));
            ScrollId = string.IsNullOrEmpty(scrollId) ? string.Empty : scrollId;
        }
    }
}
