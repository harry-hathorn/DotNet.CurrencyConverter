using Microsoft.AspNetCore.Authorization;

namespace Presentation.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("UserOnly", policy =>
                policy.RequireRole("user"));
        });

        return services;
    }
}
