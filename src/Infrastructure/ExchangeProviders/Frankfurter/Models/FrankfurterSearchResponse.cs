using System.Text.Json.Serialization;

namespace Infrastructure.ExchangeProviders.Frankfurter.Models
{
    internal class FrankfurterSearchResponse
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; }
    }
}
