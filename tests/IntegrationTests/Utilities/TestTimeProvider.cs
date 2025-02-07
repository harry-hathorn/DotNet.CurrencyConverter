using Application.Abstractions;

namespace Infrastructure.Utilities
{
    public class TestTimeProvider : ITimeProvider
    {
        public static DateTime TestDate = new DateTime(2002, 12, 3);
        public DateTime UtcNow()
        {
            return TestDate;
        }
    }
}
