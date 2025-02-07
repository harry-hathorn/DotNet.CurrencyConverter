using Presentation.Currencies;
using Application;
using Infrastructure;
using Presentation.Extensions;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Presentation.Middleware;
using System.Security.Claims;
using Asp.Versioning.Builder;
using Asp.Versioning;

namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();
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

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            var app = builder.Build();

            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder versionedGroup = app
                .MapGroup("api/v{apiVersion:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            versionedGroup.MapCurrencyEndpoints();
      

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

            app.Run();
        }
    }
}
