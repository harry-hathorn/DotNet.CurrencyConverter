namespace Application.Abstractions;

public interface ITimeProvider
{
    DateTime UtcNow();
}
