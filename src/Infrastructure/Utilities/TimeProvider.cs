using Application.Abstractions;

namespace Infrastructure.Utilities
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
