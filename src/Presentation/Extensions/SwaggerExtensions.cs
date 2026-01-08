namespace Presentation.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Currency Converter API",
                Version = "v1",
                Description = "A currency converter API with comprehensive features"
            });
        });

        return services;
    }
}
