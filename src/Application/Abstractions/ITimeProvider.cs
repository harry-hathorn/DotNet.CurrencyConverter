namespace Application.Abstractions
{
    public interface ITimeProvider
    {
        public DateTime UtcNow();
    }
}
