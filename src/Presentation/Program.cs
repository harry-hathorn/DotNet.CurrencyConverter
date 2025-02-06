using Presentation.Currencies;
using Application;
using Infrastructure;
using Presentation.Extensions;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Presentation.Middleware;
using System.Security.Claims;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerWithAuth();

            builder.Services.AddRateLimiting();
            builder.Services.InjectApplication(builder.Configuration);
            builder.Services.InjectInfrastructure(builder.Configuration);

            builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("Currency.Converter.Api"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();
                tracing.AddOtlpExporter();
            });

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();
            app.UseMiddleware<RequestLogContextMiddleware>();
            app.UseSerilogRequestLogging(options => {
                options.EnrichDiagnosticContext = (diagnosticContext, context) =>
                {
                    var clientIp = context.Connection.RemoteIpAddress?.ToString();
                    var clientId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var AccessToken = context.Request.Headers["Authorization"];
                    diagnosticContext.Set("ClientIp", clientIp);
                    diagnosticContext.Set("ClientId", clientId);
                    diagnosticContext.Set("JwtToken", AccessToken);
                };
            });
            app.MapCurrencyEndpoints();

            app.Run();
        }
    }
}
