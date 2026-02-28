using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SwiggyClone.Api.Authorization;
using SwiggyClone.Api.Middleware;
using SwiggyClone.Api.Observability;
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
    // Controllers & API explorer
    // ---------------------------------------------------------------------------
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // ---------------------------------------------------------------------------
    // Swagger / OpenAPI with JWT bearer scheme
    // ---------------------------------------------------------------------------
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "SwiggyClone API",
            Version = "v1",
            Description = "Food delivery & dine-in platform API",
        });

        var jwtSecurityScheme = new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            Description = "Enter your JWT token below (do **not** include the 'Bearer' prefix).",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme,
            },
        };

        options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtSecurityScheme, Array.Empty<string>() },
        });
    });

    // ---------------------------------------------------------------------------
    // Authentication – JWT Bearer
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
        });

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
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
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

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Swagger UI only in development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "SwiggyClone API v1");
            options.RoutePrefix = string.Empty; // serve Swagger UI at the root
        });
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHub<OrderTrackingHub>("/hubs/order-tracking");
    app.MapHub<DineInHub>("/hubs/dine-in");
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
