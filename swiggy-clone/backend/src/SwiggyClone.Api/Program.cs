using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SwiggyClone.Api.Authorization;
using SwiggyClone.Api.Middleware;
using SwiggyClone.Api.Observability;
using SwiggyClone.Api.OpenApi;
using SwiggyClone.Api.Security;
using SwiggyClone.Api.Services;
using SwiggyClone.Application;
using SwiggyClone.Api.Hubs;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Infrastructure;
using SwiggyClone.Shared.Constants;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .Enrich.WithSpan()
    .Enrich.WithSensitiveDataMasking()
    .WriteTo.Console(formatProvider: System.Globalization.CultureInfo.InvariantCulture)
    .WriteTo.Seq("http://localhost:5341")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting {Application} host", AppConstants.ApiVersion);

    var builder = WebApplication.CreateBuilder(args);

    // ---------------------------------------------------------------------------
    // Serilog – replace default logging
    // ---------------------------------------------------------------------------
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithSpan()
            .Enrich.WithSensitiveDataMasking()
            .AddElasticsearchSink(context.HostingEnvironment, context.Configuration);
    });

    // ---------------------------------------------------------------------------
    // Application & Infrastructure layers
    // ---------------------------------------------------------------------------
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddObservability(builder.Configuration);
    builder.Services.AddSignalR();
    builder.Services.AddSingleton<IRealtimeNotifier, SwiggyClone.Api.Services.SignalRRealtimeNotifier>();

    // ---------------------------------------------------------------------------
    // Rate Limiting – AspNetCoreRateLimit
    // ---------------------------------------------------------------------------
    builder.Services.AddRateLimiting(builder.Configuration);

    // ---------------------------------------------------------------------------
    // Response Compression – Brotli + gzip
    // ---------------------------------------------------------------------------
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
        options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
        options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults
            .MimeTypes.Concat(["application/json"]);
    });

    builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(
        options => options.Level = System.IO.Compression.CompressionLevel.Fastest);

    // ---------------------------------------------------------------------------
    // Controllers & API explorer
    // ---------------------------------------------------------------------------
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddApiVersioningConfiguration();

    // ---------------------------------------------------------------------------
    // Swagger / OpenAPI with JWT bearer scheme
    // ---------------------------------------------------------------------------
    builder.Services.AddSwaggerConfiguration();

    // ---------------------------------------------------------------------------
    // HSTS + Kestrel request size limit
    // ---------------------------------------------------------------------------
    if (!builder.Environment.IsDevelopment())
    {
        builder.Services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });
    }

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
    });

    // ---------------------------------------------------------------------------
    // Authentication – JWT Bearer + API Key
    // ---------------------------------------------------------------------------
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var secretKey = jwtSection["SecretKey"]
                    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero,
            };

            // Allow SignalR to receive the JWT via query string
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        })
        .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });

    // ---------------------------------------------------------------------------
    // Authorization
    // ---------------------------------------------------------------------------
    builder.Services.AddAuthorizationPolicies();

    // ---------------------------------------------------------------------------
    // Current user accessor
    // ---------------------------------------------------------------------------
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    // ---------------------------------------------------------------------------
    // CORS – permissive for development; tighten in production via config
    // ---------------------------------------------------------------------------
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
            else
            {
                var allowedOrigins = builder.Configuration
                    .GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? [];

                policy
                    .WithOrigins(allowedOrigins)
                    .WithHeaders("Authorization", "Content-Type", "X-Requested-With", "X-Correlation-Id", "X-Api-Key", "Api-Version")
                    .WithExposedHeaders("api-supported-versions", "api-deprecated-versions")
                    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromHours(1));
            }
        });
    });

    // ---------------------------------------------------------------------------
    // Health checks – Postgres, Redis, Kafka, Elasticsearch
    // ---------------------------------------------------------------------------
    builder.Services.AddEnhancedHealthChecks(builder.Configuration);

    // ---------------------------------------------------------------------------
    // Build the app
    // ---------------------------------------------------------------------------
    var app = builder.Build();

    // ---------------------------------------------------------------------------
    // Database seeding (development only)
    // ---------------------------------------------------------------------------
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<SwiggyClone.Infrastructure.Persistence.AppDbContext>();
        await SwiggyClone.Infrastructure.Persistence.DataSeeder.SeedAsync(dbContext);
    }

    // ---------------------------------------------------------------------------
    // Middleware pipeline (order matters)
    // ---------------------------------------------------------------------------
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };
    });

    app.UseResponseCompression();

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Rate limiting
    app.UseIpRateLimiting();

    // Security headers
    app.UseMiddleware<SecurityHeadersMiddleware>();

    // HSTS + HTTPS redirect (production only)
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    // Swagger UI only in development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in app.DescribeApiVersions())
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                options.SwaggerEndpoint(url, $"SwiggyClone API {description.GroupName}");
            }

            options.RoutePrefix = string.Empty; // serve Swagger UI at the root
        });
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    // Audit logging for mutating requests on critical paths
    app.UseMiddleware<AuditLoggingMiddleware>();

    app.MapControllers();
    app.MapHub<OrderTrackingHub>("/hubs/order-tracking");
    app.MapHub<DineInHub>("/hubs/dine-in");
    app.MapHub<ChatSupportHub>("/hubs/chat-support");
    app.MapHealthChecks("/health");
    app.MapHealthChecks("/health/ready", HealthCheckResponseWriter.ReadinessOptions);
    app.MapPrometheusScrapingEndpoint("/metrics");

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
