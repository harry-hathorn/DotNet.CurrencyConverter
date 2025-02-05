namespace Domain.Currencies
{
    public interface IExchangeProvider
    {
        public ExchangeProviderType ProviderType { get; }
        public Task GetLatestExchangeRatesAsync();
    }
}