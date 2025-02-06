namespace Application.Abstractions
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken) where T : class;
    }
}