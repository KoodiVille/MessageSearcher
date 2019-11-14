using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SearchAPI.Models
{
    public class Message
    {
        public string Id { get; set; }
        [Text(Name = "user_name")]
        public string UserName { get; set; }
        [Text(Name = "display_name")]
        public string DisplayName { get; set; }

        public string Channel { get; set; }
        public string Timestamp { get; set; }
        public string Text { get; set; }
    }
}
