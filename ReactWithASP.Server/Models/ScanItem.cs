using System;
using Newtonsoft.Json;

namespace ReactWithASP.Server.Models
{
    public class ScanItem
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? BlobUrl { get; set; }
        public string? OriginalFileName { get; set; }
    }
} 