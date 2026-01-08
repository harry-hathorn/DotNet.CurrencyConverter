
using TimeProvider = Infrastructure.Utilities.TimeProvider;

namespace UnitTests.Infrastructure.TimesProviders
{
    public class UtcNowShould
    {
        [Fact]
        public void ReturnCurrentUtcTime()
        {
            var beforeTime = DateTime.UtcNow.AddSeconds(-1);
            var timeProvider = new TimeProvider();
            var result = timeProvider.UtcNow();
            var afterTime = DateTime.UtcNow.AddSeconds(1);
            Assert.True(result >= beforeTime && result <= afterTime);
        }
    }
}
