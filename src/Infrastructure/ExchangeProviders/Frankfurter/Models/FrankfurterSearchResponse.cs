using System.Text.Json.Serialization;

namespace Infrastructure.ExchangeProviders.Frankfurter.Models
{
    public class FrankfurterSearchResponse
    {
        public double Amount { get; set; }
        public string Base { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }
        public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; }
    }
}
