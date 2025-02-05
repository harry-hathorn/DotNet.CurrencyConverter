using Domain.Currencies;

namespace UnitTests.Domain.CurrencySnapshots
{
    public class ConvertShould
    {
        [Theory]
        [InlineData("AUD", 1, 1.6629)]
        [InlineData("BGN", 1, 1.9558)]
        [InlineData("AUD", 2, 3.3258)]
        [InlineData("BGN", 44.1231, 86.295958)]
        [InlineData("AUD", 144.132, 239.677102)]
        public void ConvertCorrectly(string code, double amount, double expected)
        {
            var currencies = new List<(string Code, decimal Amount)>
            {
                ("AUD", 1.6629m),
                ("BGN", 1.9558m),
            };
            var currencySnapShot = CurrencySnapshot.Create("USD", new DateTime(2001, 12, 12), currencies).Value;

            var converted = currencySnapShot.Convert(new Money((decimal)amount), CurrencyCode.FromCode(code).Value).Value.Value;
            Assert.Equal((decimal)expected, converted, 4);
        }

    }
}
