using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace ReactWithASP.Server.Models
{
    public class ToDoItem
    {
        [JsonProperty("id")]
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsComplete { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
