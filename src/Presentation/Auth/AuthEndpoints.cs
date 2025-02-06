using Application.Abstractions;

namespace Presentation.Currencies;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("auth/generate-token",(
             Guid userId,
             ITokenProvider tokenProvider) =>
        {
            string token = tokenProvider.Create(userId);
            return Results.Ok(new { Token = token });
        });
    }
}
