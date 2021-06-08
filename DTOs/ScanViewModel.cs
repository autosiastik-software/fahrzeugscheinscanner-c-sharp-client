using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ScannerApp.DTOs
{
    public class ScanViewModel
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public Guid ScanId { get; set; }
        [JsonProperty("scanDetailId")]
        [JsonPropertyName("scanDetailId")]
        public Guid ScanDetailId { get; set; }
        [JsonProperty("timestamp")]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("registrationNumber")]
        [JsonPropertyName("registrationNumber")]
        public string RegistrationNumber { get; set; }
        [JsonProperty("detailsAvailable")]
        [JsonPropertyName("detailsAvailable")]
        public bool DetailsAvailable { get; set; }
    }
}
