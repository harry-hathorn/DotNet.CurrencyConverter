namespace Application.Abstractions
{
    public interface ITokenProvider
    {
        string Create(Guid userId);
    }
}
