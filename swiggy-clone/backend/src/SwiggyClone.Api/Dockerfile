# =============================================================================
# Stage 1 – Restore & Publish
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy Directory.Build.props and csproj files first to leverage Docker layer caching for NuGet restore.
COPY Directory.Build.props .
COPY src/SwiggyClone.Domain/SwiggyClone.Domain.csproj           src/SwiggyClone.Domain/
COPY src/SwiggyClone.Shared/SwiggyClone.Shared.csproj           src/SwiggyClone.Shared/
COPY src/SwiggyClone.Application/SwiggyClone.Application.csproj src/SwiggyClone.Application/
COPY src/SwiggyClone.Infrastructure/SwiggyClone.Infrastructure.csproj src/SwiggyClone.Infrastructure/
COPY src/SwiggyClone.Api/SwiggyClone.Api.csproj                 src/SwiggyClone.Api/

RUN dotnet restore src/SwiggyClone.Api/SwiggyClone.Api.csproj

# Copy everything else and publish.
COPY . .
RUN dotnet publish src/SwiggyClone.Api/SwiggyClone.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# =============================================================================
# Stage 2 – Runtime
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Install ICU libraries for globalization/localization support.
RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Security: run as non-root user.
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SwiggyClone.Api.dll"]
