namespace Infrastructure.ExchangeProviders.Frankfurter.Models;

public class FrankfurterSearchResponse
{
    public decimal Amount { get; set; }
    public string Base { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; } = new();
}
