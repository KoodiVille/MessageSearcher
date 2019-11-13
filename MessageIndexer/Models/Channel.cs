using System.Text.Json.Serialization;

namespace MessageIndexer.Models
{
    public class Channel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Members { get; set; }
        public Topic Topic { get; set; }
        public Purpose Purpose { get; set; }
        public int Created { get; set; }
        public int Unlinked { get; set; }
        public string Creator { get; set; }

        [JsonPropertyName("is_channel")]
        public bool IsChannel{ get; set; }

        [JsonPropertyName("is_general")]
        public bool IsGeneral { get; set; }

        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }
        
        [JsonPropertyName("name_normalized")]
        public string NameNormalized { get; set; }
        
        [JsonPropertyName("is_shared")]
        public bool IsShared { get; set; }
        
        [JsonPropertyName("is_org_shared")]
        public bool IsOrgShared { get; set; }

        [JsonPropertyName("is_member")]
        public bool IsMember { get; set; }

        [JsonPropertyName("is_private")]
        public bool IsPrivate { get; set; }

        [JsonPropertyName("is_mpim")]
        public bool IsMpim { get; set; }

        [JsonPropertyName("previous_names")]
        public string[] PreviousNames { get; set; }

        [JsonPropertyName("num_members")]
        public int NumberOfMembers { get; set; }
    }
}
