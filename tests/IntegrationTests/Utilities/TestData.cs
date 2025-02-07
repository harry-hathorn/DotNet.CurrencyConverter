using Application.Currencies.FindLatestCurrency.Dtos;
using Infrastructure.ExchangeProviders.Frankfurter.Models;

namespace IntegrationTests.Utilities
{
    internal class TestData
    {
        internal static FrankfurterSearchResponse FrankfurterSearchResponse = new FrankfurterSearchResponse()
        {
            Amount = 1,
            Base = "USD",
            StartDate = new DateTime(2021, 5, 1),
            EndDate = new DateTime(2021, 5, 15),
            Rates = new Dictionary<DateTime, Dictionary<string, decimal>>
            {
                { new DateTime(2021, 5, 1), new Dictionary<string, decimal> { { "EUR", 0.83m }, { "GBP", 0.72m } } },
                { new DateTime(2021, 5, 2), new Dictionary<string, decimal> { { "EUR", 0.84m }, { "GBP", 0.73m } } }
            }
        };

        internal static FrankfurterLatestResponse FrankfurterLatestResponse = new FrankfurterLatestResponse()
        {
            Amount = 1,
            Base = "EUR",
            Date = new DateTime(2025, 02, 04),
            Rates = new()
            {
                AUD = 1.6629m,
                BGN = 1.9558m,
                BRL = 6.0138m,
                CAD = 1.4894m,
                CHF = 0.9396m,
                CNY = 7.4943m,
                CZK = 25.172m,
                DKK = 7.461m,
                GBP = 0.83188m,
                HKD = 8.0489m,
                HUF = 407.15m,
                IDR = 16865m,
                ILS = 3.6961m,
                INR = 90.01m,
                ISK = 146.8m,
                JPY = 160.52m,
                KRW = 1504.21m,
                MXN = 21.132m,
                MYR = 4.5929m,
                NOK = 11.721m,
                NZD = 1.8418m,
                PHP = 60.222m,
                PLN = 4.2193m,
                RON = 4.9769m,
                SEK = 11.418m,
                SGD = 1.4015m,
                THB = 34.937m,
                TRY = 37.155m,
                USD = 1.0335m,
                ZAR = 19.4072m
            }
        };

        internal static FindLatestCurrencyResultDto LatestDto = new FindLatestCurrencyResultDto(TestData.FrankfurterLatestResponse.Base,
                TestData.FrankfurterLatestResponse.Date,
                new List<FindLatestCurrencyExchangeRateDto>() {
                  new FindLatestCurrencyExchangeRateDto("AUD", FrankfurterLatestResponse.Rates.AUD.Value),
                  new FindLatestCurrencyExchangeRateDto("BGN", FrankfurterLatestResponse.Rates.BGN.Value),
                  new FindLatestCurrencyExchangeRateDto("BRL", FrankfurterLatestResponse.Rates.BRL.Value),
                  new FindLatestCurrencyExchangeRateDto("CAD", FrankfurterLatestResponse.Rates.CAD.Value),
                  new FindLatestCurrencyExchangeRateDto("CHF", FrankfurterLatestResponse.Rates.CHF.Value),
                  new FindLatestCurrencyExchangeRateDto("CNY", FrankfurterLatestResponse.Rates.CNY.Value),
                  new FindLatestCurrencyExchangeRateDto("CZK", FrankfurterLatestResponse.Rates.CZK.Value),
                  new FindLatestCurrencyExchangeRateDto("DKK", FrankfurterLatestResponse.Rates.DKK.Value),
                  new FindLatestCurrencyExchangeRateDto("GBP", FrankfurterLatestResponse.Rates.GBP.Value),
                  new FindLatestCurrencyExchangeRateDto("HKD", FrankfurterLatestResponse.Rates.HKD.Value),
                  new FindLatestCurrencyExchangeRateDto("HUF", FrankfurterLatestResponse.Rates.HUF.Value),
                  new FindLatestCurrencyExchangeRateDto("IDR", FrankfurterLatestResponse.Rates.IDR.Value),
                  new FindLatestCurrencyExchangeRateDto("ILS", FrankfurterLatestResponse.Rates.ILS.Value),
                  new FindLatestCurrencyExchangeRateDto("INR", FrankfurterLatestResponse.Rates.INR.Value),
                  new FindLatestCurrencyExchangeRateDto("ISK", FrankfurterLatestResponse.Rates.ISK.Value),
                  new FindLatestCurrencyExchangeRateDto("JPY", FrankfurterLatestResponse.Rates.JPY.Value),
                  new FindLatestCurrencyExchangeRateDto("KRW", FrankfurterLatestResponse.Rates.KRW.Value),
                  new FindLatestCurrencyExchangeRateDto("MXN", FrankfurterLatestResponse.Rates.MXN.Value),
                  new FindLatestCurrencyExchangeRateDto("MYR", FrankfurterLatestResponse.Rates.MYR.Value),
                  new FindLatestCurrencyExchangeRateDto("NOK", FrankfurterLatestResponse.Rates.NOK.Value),
                  new FindLatestCurrencyExchangeRateDto("NZD", FrankfurterLatestResponse.Rates.NZD.Value),
                  new FindLatestCurrencyExchangeRateDto("PHP", FrankfurterLatestResponse.Rates.PHP.Value),
                  new FindLatestCurrencyExchangeRateDto("PLN", FrankfurterLatestResponse.Rates.PLN.Value),
                  new FindLatestCurrencyExchangeRateDto("RON", FrankfurterLatestResponse.Rates.RON.Value),
                  new FindLatestCurrencyExchangeRateDto("SEK", FrankfurterLatestResponse.Rates.SEK.Value),
                  new FindLatestCurrencyExchangeRateDto("SGD", FrankfurterLatestResponse.Rates.SGD.Value),
                  new FindLatestCurrencyExchangeRateDto("THB", FrankfurterLatestResponse.Rates.THB.Value),
                  new FindLatestCurrencyExchangeRateDto("TRY", FrankfurterLatestResponse.Rates.TRY.Value),
                  new FindLatestCurrencyExchangeRateDto("USD", FrankfurterLatestResponse.Rates.USD.Value),
                  new FindLatestCurrencyExchangeRateDto("ZAR", FrankfurterLatestResponse.Rates.ZAR.Value)
                });
    }
}
