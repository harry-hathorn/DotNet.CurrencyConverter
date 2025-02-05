namespace Domain.Providers
{
    public interface IExchangeProvider
    {
        public Task GetLatestExchangeRatesAsync();
    }
}