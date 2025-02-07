
using TimeProvider = Infrastructure.Utilities.TimeProvider;

namespace UnitTests.Infrastructure.TimesProviders
{
    public class UtcNowShould
    {
        [Fact]
        public void ReturnCurrentUtcTime()
        {
            var testStartTime = DateTime.UtcNow;
            var timeProvider = new TimeProvider();
            var result = timeProvider.UtcNow();
            var testEndTime = DateTime.UtcNow;
            Assert.True(result < testEndTime && result > testStartTime);
        }
    }
}
