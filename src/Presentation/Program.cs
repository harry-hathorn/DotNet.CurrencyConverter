using Presentation.Currencies;
using Application;
using Infrastructure;
using Presentation.Extensions;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerWithAuth();

            builder.Services.AddRateLimiting();
            builder.Services.InjectApplication(builder.Configuration);
            builder.Services.InjectInfrastructure(builder.Configuration);
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();
            app.MapCurrencyEndpoints();

            app.Run();
        }
    }
}
