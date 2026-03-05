# 🚀 CLAUDE CODE — MASTER PROMPT
# Enterprise Swiggy Clone + Dine-In Order Platform
# Web + Android + iOS (Flutter) | .NET 9 Backend | PostgreSQL | Docker

---

## ⚠️ CRITICAL: CONTEXT PRESERVATION PROTOCOL

**Before starting ANY module, ALWAYS execute this command first:**

```bash
if [ -f "./context.md" ]; then
  echo "=== CONTEXT FILE FOUND — READING PREVIOUS STATE ==="
  cat ./context.md
else
  echo "=== NO CONTEXT FILE — FRESH START ==="
  echo "# Project Context — Swiggy Clone Enterprise Platform" > ./context.md
  echo "## Initialized: $(date -u +"%Y-%m-%dT%H:%M:%SZ")" >> ./context.md
  echo "## Status: FRESH_START" >> ./context.md
  echo "## Completed Modules: none" >> ./context.md
fi
```

**After completing EVERY module, ALWAYS update `context.md`:**

```bash
# Template — append after each module completion
cat >> ./context.md << 'CONTEXT_BLOCK'

---
### Module: [MODULE_NAME]
- **Status**: ✅ COMPLETED
- **Completed At**: [TIMESTAMP]
- **Files Created**:
  - [list every file path created/modified]
- **Database Tables Created**:
  - [list tables]
- **API Endpoints Created**:
  - [list endpoints with HTTP method + path]
- **Docker Services Added**:
  - [list service names]
- **Dependencies Added**:
  - [list NuGet/pub packages added]
- **Environment Variables Required**:
  - [list env vars]
- **Pending/Known Issues**:
  - [list any deferred items]
- **Next Module Prerequisites**:
  - [what the next module depends on from this one]
CONTEXT_BLOCK
```

**If Claude Code session is restarted or a new module begins:**
1. Read `context.md` FIRST
2. Verify file structure matches context
3. Run existing tests to confirm green state
4. ONLY THEN proceed with new module

---

## 📋 TABLE OF CONTENTS — MODULE EXECUTION ORDER

| Phase | Module | Description | Estimated Files |
|-------|--------|-------------|-----------------|
| 0 | Project Scaffold | Monorepo structure, tooling, Docker base | ~30 |
| 1 | Database Foundation | PostgreSQL schema, migrations, seed data | ~50 |
| 2 | Auth & Identity | OAuth2 + JWT, user registration, RBAC | ~40 |
| 3 | Restaurant Management | Restaurant CRUD, menu, categories, hours | ~45 |
| 4 | Customer Discovery | Search, filters, restaurant listing, details | ~35 |
| 5 | Cart & Ordering (Delivery) | Cart, checkout, order placement, tracking | ~50 |
| 6 | Dine-In (Customer) | QR scan, table session, in-seat ordering | ~40 |
| 7 | Dine-In (Restaurant) | Table management, order queue, KDS | ~45 |
| 8 | Payments | Razorpay/Stripe integration, refunds, ledger | ~30 |
| 9 | Notifications | FCM push, WebSocket real-time, email/SMS | ~25 |
| 10 | Delivery Partner | Assignment, tracking, earnings | ~35 |
| 11 | Admin Panel | Super admin dashboard, analytics, moderation | ~40 |
| 12 | Search & Discovery | Elasticsearch indexing, autocomplete, filters | ~20 |
| 13 | Ratings & Reviews | Customer reviews, restaurant ratings, photos | ~20 |
| 14 | Promotions & Coupons | Coupon engine, offers, loyalty points | ~25 |
| 15 | Observability | Logging, metrics, tracing, alerting | ~20 |
| 16 | DevOps & Deployment | CI/CD, Terraform, K8s manifests, Helm | ~30 |

---

## 🏗️ GLOBAL ARCHITECTURE RULES (APPLY TO ALL MODULES)

### Persona
You are a **principal-level technical leader** with 25+ years of experience building enterprise-scale systems. You operate across four integrated roles: **Senior Code Engineer**, **Principal Software Architect**, **Database Administrator**, and **DevOps Architect**. Activate all relevant roles per module.

### Tech Stack (Non-Negotiable)

**Frontend:**
- Flutter (latest stable) + Dart 3+
- Clean Architecture: Presentation → Application → Domain → Data
- Feature-first modular structure
- MVVM + Riverpod for state management
- Freezed for immutable models + union types
- GoRouter for routing (deep linking + web support)
- Dio for networking (interceptors, retry, logging)
- JSON Serializable for serialization
- Hive for local storage, Flutter Secure Storage for tokens
- Firebase Cloud Messaging for push notifications
- WebSockets for live order/dine-in tracking
- Google Maps Flutter + Geolocator + Places API
- Razorpay / Stripe SDK for payments
- Material 3, Lottie, Cached Network Image

**Backend:**
- .NET 9 (ASP.NET Core Web API)
- Clean Architecture: API → Application → Domain → Infrastructure
- CQRS with MediatR
- FluentValidation for request validation
- Mapster or AutoMapper for object mapping
- Entity Framework Core 9 (PostgreSQL provider) with code-first migrations
- REST API (primary) + gRPC (internal service communication)
- Event-driven via Kafka (MassTransit or Confluent.Kafka)
- Background jobs: Hangfire or Quartz.NET

**Database:**
- PostgreSQL 16 (primary OLTP)
- Redis 7 (caching, sessions, rate limiting, cart)
- Elasticsearch 8 (search, autocomplete, filters)
- Apache Kafka (event streaming, order events, inventory sync)

**Infrastructure:**
- Docker (multi-stage builds, non-root, distroless base)
- Docker Compose (local development)
- Kubernetes (EKS) manifests for production
- Terraform for IaC
- GitHub Actions for CI/CD
- Nginx reverse proxy / API gateway

### Code Quality Standards

1. **Every code file** must follow SOLID principles — no god classes, no feature envy
2. **Every public method** must have XML doc comments (.NET) or dartdoc (Flutter)
3. **Every entity** must have database-level constraints (NOT NULL, CHECK, FK, unique)
4. **Every API endpoint** must have: request validation, proper HTTP status codes, structured error response `{ code, message, details, traceId }`
5. **Every query** touching tables expected to exceed 10K rows must have an index strategy
6. **Every external call** must have: circuit breaker, retry with exponential backoff + jitter, timeout
7. **Unit test stubs** for every service class — happy path, edge cases, error cases
8. **No magic strings** — use constants or enums
9. **No `SELECT *`** — project only needed columns
10. **Cursor-based pagination** — never offset-based for large datasets

### Naming Conventions

| Artifact | Convention | Example |
|----------|-----------|---------|
| .NET Project | PascalCase | `SwiggyClone.Domain` |
| .NET Class | PascalCase | `OrderService` |
| .NET Interface | IPascalCase | `IOrderRepository` |
| .NET Method | PascalCase | `CreateOrderAsync` |
| .NET Private field | _camelCase | `_orderRepository` |
| DB Table | snake_case plural | `orders`, `order_items` |
| DB Column | snake_case | `created_at`, `restaurant_id` |
| DB Index | idx_{table}_{columns} | `idx_orders_restaurant_id` |
| DB FK | fk_{table}_{ref_table} | `fk_orders_restaurants` |
| Flutter file | snake_case | `order_repository.dart` |
| Flutter class | PascalCase | `OrderRepository` |
| Flutter feature folder | snake_case | `dine_in_customer` |
| API Route | kebab-case | `/api/v1/dine-in-orders` |
| Kafka Topic | dot.separated | `order.created`, `payment.completed` |
| Docker service | kebab-case | `api-gateway`, `order-service` |

### Error Response Contract (All APIs)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "traceId": "00-abc123-def456-01",
  "errors": {
    "fieldName": ["Error message 1", "Error message 2"]
  }
}
```

### Folder Structure (Backend .NET 9)

```
src/
├── SwiggyClone.Api/                    # ASP.NET Core Web API host
│   ├── Controllers/                    # Thin controllers — delegate to MediatR
│   ├── Middleware/                      # Exception handling, correlation ID, request logging
│   ├── Filters/                        # Action filters, validation filters
│   ├── Extensions/                     # Service registration extension methods
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   └── Dockerfile
├── SwiggyClone.Application/            # Use cases, CQRS commands/queries, DTOs
│   ├── Common/                         # Behaviors (validation, logging, caching pipeline)
│   ├── Features/
│   │   ├── Auth/
│   │   │   ├── Commands/
│   │   │   │   ├── RegisterUser/
│   │   │   │   │   ├── RegisterUserCommand.cs
│   │   │   │   │   ├── RegisterUserCommandHandler.cs
│   │   │   │   │   └── RegisterUserCommandValidator.cs
│   │   │   │   └── LoginUser/
│   │   │   └── Queries/
│   │   ├── Restaurants/
│   │   ├── Orders/
│   │   ├── DineIn/
│   │   ├── Cart/
│   │   ├── Payments/
│   │   ├── Delivery/
│   │   ├── Search/
│   │   ├── Reviews/
│   │   ├── Promotions/
│   │   └── Admin/
│   └── Contracts/                      # Interfaces consumed by Infrastructure
├── SwiggyClone.Domain/                 # Entities, Value Objects, Domain Events, Enums
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Events/
│   ├── Enums/
│   ├── Exceptions/
│   └── Common/                         # BaseEntity, AuditableEntity, IAggregateRoot
├── SwiggyClone.Infrastructure/         # EF Core, Redis, Kafka, Elasticsearch, external APIs
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/             # EF Core fluent API entity configurations
│   │   ├── Migrations/
│   │   ├── Repositories/
│   │   └── Interceptors/              # Audit interceptor, soft-delete interceptor
│   ├── Caching/                        # Redis implementation
│   ├── Messaging/                      # Kafka producers and consumers
│   ├── Search/                         # Elasticsearch implementation
│   ├── Identity/                       # JWT, OAuth, token generation
│   ├── Notifications/                  # FCM, Email, SMS
│   ├── Storage/                        # S3/blob storage
│   └── Extensions/
└── SwiggyClone.Shared/                # Cross-cutting: Result<T>, pagination, constants
    ├── Result.cs
    ├── PagedResult.cs
    ├── CursorPagedResult.cs
    └── Constants/

tests/
├── SwiggyClone.UnitTests/
│   ├── Application/
│   ├── Domain/
│   └── Infrastructure/
├── SwiggyClone.IntegrationTests/
│   ├── Api/
│   └── Persistence/
└── SwiggyClone.ArchitectureTests/      # NetArchTest — enforce layer boundaries
```

### Folder Structure (Frontend Flutter)

```
lib/
├── main.dart
├── app/
│   ├── app.dart                        # MaterialApp with GoRouter
│   ├── di/                             # Riverpod providers, dependency injection
│   └── theme/                          # Material 3 theme, colors, typography
├── core/
│   ├── constants/
│   ├── errors/                         # Failure classes, exceptions
│   ├── network/                        # Dio client, interceptors, API config
│   ├── storage/                        # Hive, Secure Storage helpers
│   ├── utils/                          # Date formatters, validators, extensions
│   └── widgets/                        # Shared widgets (buttons, cards, loaders)
├── features/
│   ├── auth/
│   │   ├── data/
│   │   │   ├── datasources/            # Remote (API) + Local (Hive/Secure Storage)
│   │   │   ├── models/                 # JSON serializable models (fromJson/toJson)
│   │   │   └── repositories/           # Repository implementation
│   │   ├── domain/
│   │   │   ├── entities/               # Pure Dart entities (Freezed)
│   │   │   ├── repositories/           # Abstract repository interface
│   │   │   └── usecases/              # Single-responsibility use cases
│   │   └── presentation/
│   │       ├── providers/              # Riverpod providers (state + notifiers)
│   │       ├── pages/                  # Full screen pages
│   │       └── widgets/                # Feature-specific widgets
│   ├── home/
│   ├── restaurant_detail/
│   ├── cart/
│   ├── orders/
│   ├── dine_in_customer/              # QR scan, table session, in-seat ordering
│   │   ├── data/
│   │   ├── domain/
│   │   └── presentation/
│   │       ├── pages/
│   │       │   ├── qr_scan_page.dart
│   │       │   ├── dine_in_menu_page.dart
│   │       │   ├── dine_in_cart_page.dart
│   │       │   ├── dine_in_order_tracking_page.dart
│   │       │   └── dine_in_bill_page.dart
│   │       ├── providers/
│   │       └── widgets/
│   ├── dine_in_restaurant/            # Restaurant-facing dine-in management
│   │   ├── data/
│   │   ├── domain/
│   │   └── presentation/
│   │       ├── pages/
│   │       │   ├── table_management_page.dart
│   │       │   ├── incoming_orders_page.dart
│   │       │   ├── kitchen_display_page.dart
│   │       │   ├── order_history_page.dart
│   │       │   └── qr_code_generator_page.dart
│   │       ├── providers/
│   │       └── widgets/
│   ├── delivery_tracking/
│   ├── search/
│   ├── payments/
│   ├── reviews/
│   ├── promotions/
│   └── profile/
└── routing/
    ├── app_router.dart                 # GoRouter configuration
    ├── route_names.dart
    └── guards/                         # Auth guards, role-based guards
```

---

## 📦 MODULE 0: PROJECT SCAFFOLD & DOCKER BASE

### Objective
Set up the monorepo, all project files, Docker Compose for local development, and base configurations.

### Instructions

1. **Create the monorepo root structure:**

```
swiggy-clone/
├── backend/                            # .NET 9 solution
│   ├── SwiggyClone.sln
│   ├── src/
│   │   ├── SwiggyClone.Api/
│   │   ├── SwiggyClone.Application/
│   │   ├── SwiggyClone.Domain/
│   │   ├── SwiggyClone.Infrastructure/
│   │   └── SwiggyClone.Shared/
│   └── tests/
│       ├── SwiggyClone.UnitTests/
│       ├── SwiggyClone.IntegrationTests/
│       └── SwiggyClone.ArchitectureTests/
├── frontend/
│   └── swiggy_clone_app/              # Flutter project
├── infrastructure/
│   ├── docker/
│   │   ├── docker-compose.yml          # Local dev: Postgres, Redis, Kafka, ES, Zookeeper
│   │   ├── docker-compose.override.yml  # Dev overrides
│   │   ├── docker-compose.prod.yml      # Prod compose
│   │   ├── postgres/
│   │   │   └── init.sql                # Initial DB creation
│   │   ├── redis/
│   │   │   └── redis.conf
│   │   ├── kafka/
│   │   ├── elasticsearch/
│   │   │   └── elasticsearch.yml
│   │   └── nginx/
│   │       └── nginx.conf              # Reverse proxy config
│   ├── terraform/
│   │   ├── modules/
│   │   ├── environments/
│   │   │   ├── dev/
│   │   │   ├── staging/
│   │   │   └── prod/
│   │   └── backend.tf
│   └── k8s/
│       ├── base/
│       ├── overlays/
│       │   ├── dev/
│       │   ├── staging/
│       │   └── prod/
│       └── helm/
├── .github/
│   └── workflows/
│       ├── backend-ci.yml
│       ├── frontend-ci.yml
│       └── deploy.yml
├── docs/
│   ├── architecture/
│   ├── api/
│   └── runbooks/
├── context.md                          # 🔴 CRITICAL — Claude context preservation file
├── .gitignore
├── .editorconfig
└── README.md
```

2. **Create the .NET 9 solution with all projects and proper references:**
   - `SwiggyClone.Domain` → no project references (innermost layer)
   - `SwiggyClone.Application` → references `Domain`, `Shared`
   - `SwiggyClone.Infrastructure` → references `Application`, `Domain`, `Shared`
   - `SwiggyClone.Api` → references `Application`, `Infrastructure`, `Shared`
   - `SwiggyClone.Shared` → no project references

3. **Install NuGet packages:**

   **SwiggyClone.Api:**
   - `Swashbuckle.AspNetCore` (Swagger)
   - `Serilog.AspNetCore` (structured logging)
   - `AspNetCoreRateLimit`
   - `Microsoft.AspNetCore.Authentication.JwtBearer`

   **SwiggyClone.Application:**
   - `MediatR`
   - `FluentValidation`
   - `FluentValidation.DependencyInjectionExtensions`
   - `Mapster`

   **SwiggyClone.Domain:**
   - (No external dependencies — pure domain)

   **SwiggyClone.Infrastructure:**
   - `Npgsql.EntityFrameworkCore.PostgreSQL`
   - `Microsoft.EntityFrameworkCore.Tools`
   - `StackExchange.Redis`
   - `Confluent.Kafka` or `MassTransit.Kafka`
   - `NEST` (Elasticsearch)
   - `System.IdentityModel.Tokens.Jwt`
   - `BCrypt.Net-Next`
   - `FirebaseAdmin`

   **SwiggyClone.Shared:**
   - (Minimal — only shared primitives)

4. **Create Flutter project:**
   ```bash
   flutter create --org com.swiggy.clone --platforms web,android,ios swiggy_clone_app
   ```
   Install pub packages: `flutter_riverpod`, `riverpod_annotation`, `freezed_annotation`, `freezed`, `json_annotation`, `json_serializable`, `build_runner`, `go_router`, `dio`, `hive_flutter`, `flutter_secure_storage`, `firebase_messaging`, `google_maps_flutter`, `geolocator`, `lottie`, `cached_network_image`, `web_socket_channel`, `qr_code_scanner`, `qr_flutter`, `intl`, `flutter_svg`, `shimmer`

5. **Create Docker Compose for local development:**

```yaml
# docker-compose.yml
services:
  postgres:
    image: postgres:16-alpine
    container_name: swiggy-postgres
    environment:
      POSTGRES_USER: swiggy_admin
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-SuperSecure123!}
      POSTGRES_DB: swiggy_clone
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./postgres/init.sql:/docker-entrypoint-initdb.d/init.sql
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U swiggy_admin -d swiggy_clone"]
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - swiggy-network

  redis:
    image: redis:7-alpine
    container_name: swiggy-redis
    command: redis-server /usr/local/etc/redis/redis.conf
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
      - ./redis/redis.conf:/usr/local/etc/redis/redis.conf
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - swiggy-network

  zookeeper:
    image: confluentinc/cp-zookeeper:7.6.0
    container_name: swiggy-zookeeper
    environment:
      ZOOKEEPER_CLIENT_PORT: 2181
      ZOOKEEPER_TICK_TIME: 2000
    networks:
      - swiggy-network

  kafka:
    image: confluentinc/cp-kafka:7.6.0
    container_name: swiggy-kafka
    depends_on:
      - zookeeper
    ports:
      - "9092:9092"
    environment:
      KAFKA_BROKER_ID: 1
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka:29092,PLAINTEXT_HOST://localhost:9092
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: "false"
    networks:
      - swiggy-network

  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.12.0
    container_name: swiggy-elasticsearch
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false
      - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
    ports:
      - "9200:9200"
    volumes:
      - elasticsearch_data:/usr/share/elasticsearch/data
    healthcheck:
      test: ["CMD-SHELL", "curl -f http://localhost:9200/_cluster/health || exit 1"]
      interval: 30s
      timeout: 10s
      retries: 5
    networks:
      - swiggy-network

  api:
    build:
      context: ../../backend
      dockerfile: src/SwiggyClone.Api/Dockerfile
    container_name: swiggy-api
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=swiggy_clone;Username=swiggy_admin;Password=${POSTGRES_PASSWORD:-SuperSecure123!}
      - ConnectionStrings__Redis=redis:6379
      - Kafka__BootstrapServers=kafka:29092
      - Elasticsearch__Uri=http://elasticsearch:9200
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
      kafka:
        condition: service_started
      elasticsearch:
        condition: service_healthy
    networks:
      - swiggy-network

  nginx:
    image: nginx:alpine
    container_name: swiggy-nginx
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
    depends_on:
      - api
    networks:
      - swiggy-network

volumes:
  postgres_data:
  redis_data:
  elasticsearch_data:

networks:
  swiggy-network:
    driver: bridge
```

6. **Create the Dockerfile for .NET API (multi-stage, non-root, distroless):**

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["src/SwiggyClone.Api/SwiggyClone.Api.csproj", "src/SwiggyClone.Api/"]
COPY ["src/SwiggyClone.Application/SwiggyClone.Application.csproj", "src/SwiggyClone.Application/"]
COPY ["src/SwiggyClone.Domain/SwiggyClone.Domain.csproj", "src/SwiggyClone.Domain/"]
COPY ["src/SwiggyClone.Infrastructure/SwiggyClone.Infrastructure.csproj", "src/SwiggyClone.Infrastructure/"]
COPY ["src/SwiggyClone.Shared/SwiggyClone.Shared.csproj", "src/SwiggyClone.Shared/"]
RUN dotnet restore "src/SwiggyClone.Api/SwiggyClone.Api.csproj"
COPY . .
RUN dotnet publish "src/SwiggyClone.Api/SwiggyClone.Api.csproj" \
    -c Release -o /app/publish --no-restore \
    /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
COPY --from=build /app/publish .
USER appuser
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "SwiggyClone.Api.dll"]
```

7. **Create base classes and shared infrastructure:**
   - `BaseEntity` (Id, CreatedAt, UpdatedAt, IsDeleted — soft delete)
   - `AuditableEntity` extends `BaseEntity` (CreatedBy, UpdatedBy)
   - `IAggregateRoot` marker interface
   - `Result<T>` monad for error handling (no exceptions for business logic)
   - `PagedResult<T>` and `CursorPagedResult<T>`
   - `DomainEvent` base class
   - Global exception handling middleware
   - Correlation ID middleware
   - Request logging middleware (Serilog)
   - MediatR pipeline behaviors: `ValidationBehavior`, `LoggingBehavior`, `CachingBehavior`

8. **Update `context.md` upon completion.**

---

## 🗄️ MODULE 1: DATABASE FOUNDATION

### Objective
Design and create the complete PostgreSQL schema with EF Core migrations.

### Database Design Principles
- 3NF for all transactional tables
- Denormalize only for read-heavy views (materialized views)
- UUID v7 for all primary keys (time-sortable, index-friendly)
- `timestamptz` for all timestamps (UTC)
- Soft-delete via `is_deleted` + `deleted_at` columns
- Audit columns on all tables: `created_at`, `updated_at`, `created_by`, `updated_by`
- Enum types stored as `smallint` with CHECK constraints (not strings)
- All foreign keys must have explicit `ON DELETE` behavior
- Composite indexes: equality columns first, range columns last
- Partial indexes for hot queries (e.g., `WHERE is_deleted = false`)

### Complete Entity List & Schema

**Core Entities:**

```sql
-- 1. USERS & AUTH
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    phone_number VARCHAR(15) NOT NULL UNIQUE,
    email VARCHAR(255) UNIQUE,
    full_name VARCHAR(100) NOT NULL,
    avatar_url VARCHAR(500),
    role SMALLINT NOT NULL DEFAULT 1, -- 1=Customer, 2=RestaurantOwner, 3=DeliveryPartner, 4=Admin
    is_verified BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    last_login_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at TIMESTAMPTZ
);

CREATE TABLE user_addresses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    label VARCHAR(50) NOT NULL, -- Home, Work, Other
    address_line_1 VARCHAR(255) NOT NULL,
    address_line_2 VARCHAR(255),
    city VARCHAR(100) NOT NULL,
    state VARCHAR(100) NOT NULL,
    postal_code VARCHAR(10) NOT NULL,
    country VARCHAR(50) NOT NULL DEFAULT 'India',
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token_hash VARCHAR(512) NOT NULL UNIQUE,
    device_info VARCHAR(255),
    expires_at TIMESTAMPTZ NOT NULL,
    revoked_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 2. RESTAURANTS
CREATE TABLE restaurants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL REFERENCES users(id),
    name VARCHAR(200) NOT NULL,
    slug VARCHAR(200) NOT NULL UNIQUE,
    description TEXT,
    cuisine_types VARCHAR(500), -- comma-separated or use junction table
    phone_number VARCHAR(15) NOT NULL,
    email VARCHAR(255),
    logo_url VARCHAR(500),
    banner_url VARCHAR(500),
    address_line_1 VARCHAR(255) NOT NULL,
    address_line_2 VARCHAR(255),
    city VARCHAR(100) NOT NULL,
    state VARCHAR(100) NOT NULL,
    postal_code VARCHAR(10) NOT NULL,
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    average_rating DECIMAL(2,1) NOT NULL DEFAULT 0.0,
    total_ratings INT NOT NULL DEFAULT 0,
    avg_delivery_time_min INT, -- in minutes
    avg_cost_for_two INT, -- in smallest currency unit (paise)
    is_veg_only BOOLEAN NOT NULL DEFAULT FALSE,
    is_accepting_orders BOOLEAN NOT NULL DEFAULT TRUE,
    is_dine_in_enabled BOOLEAN NOT NULL DEFAULT FALSE,
    commission_rate DECIMAL(5,2) NOT NULL DEFAULT 15.00, -- percentage
    fssai_license VARCHAR(20),
    gst_number VARCHAR(20),
    status SMALLINT NOT NULL DEFAULT 0, -- 0=Pending, 1=Approved, 2=Suspended, 3=Rejected
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE restaurant_operating_hours (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    restaurant_id UUID NOT NULL REFERENCES restaurants(id) ON DELETE CASCADE,
    day_of_week SMALLINT NOT NULL, -- 0=Sunday, 6=Saturday
    open_time TIME NOT NULL,
    close_time TIME NOT NULL,
    is_closed BOOLEAN NOT NULL DEFAULT FALSE,
    UNIQUE(restaurant_id, day_of_week)
);

-- 3. MENU
CREATE TABLE menu_categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    restaurant_id UUID NOT NULL REFERENCES restaurants(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    description VARCHAR(500),
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE menu_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    category_id UUID NOT NULL REFERENCES menu_categories(id) ON DELETE CASCADE,
    restaurant_id UUID NOT NULL REFERENCES restaurants(id) ON DELETE CASCADE,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    price INT NOT NULL, -- in smallest currency unit (paise)
    discounted_price INT,
    image_url VARCHAR(500),
    is_veg BOOLEAN NOT NULL DEFAULT TRUE,
    is_available BOOLEAN NOT NULL DEFAULT TRUE,
    is_bestseller BOOLEAN NOT NULL DEFAULT FALSE,
    preparation_time_min INT NOT NULL DEFAULT 15,
    sort_order INT NOT NULL DEFAULT 0,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE menu_item_variants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    menu_item_id UUID NOT NULL REFERENCES menu_items(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL, -- e.g., "Small", "Medium", "Large"
    price_adjustment INT NOT NULL DEFAULT 0, -- additive to base price
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
    is_available BOOLEAN NOT NULL DEFAULT TRUE,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE menu_item_addons (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    menu_item_id UUID NOT NULL REFERENCES menu_items(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL, -- e.g., "Extra Cheese"
    price INT NOT NULL DEFAULT 0,
    is_veg BOOLEAN NOT NULL DEFAULT TRUE,
    is_available BOOLEAN NOT NULL DEFAULT TRUE,
    max_quantity INT NOT NULL DEFAULT 5,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 4. ORDERS (Delivery + Dine-In unified)
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_number VARCHAR(20) NOT NULL UNIQUE, -- human-readable: SWG-20260225-XXXX
    user_id UUID NOT NULL REFERENCES users(id),
    restaurant_id UUID NOT NULL REFERENCES restaurants(id),
    order_type SMALLINT NOT NULL, -- 1=Delivery, 2=DineIn, 3=Takeaway
    status SMALLINT NOT NULL DEFAULT 0,
    -- Delivery statuses: 0=Placed, 1=Confirmed, 2=Preparing, 3=ReadyForPickup, 4=OutForDelivery, 5=Delivered, 6=Cancelled
    -- DineIn statuses: 0=Placed, 1=Confirmed, 2=Preparing, 3=Ready, 4=Served, 5=Completed, 6=Cancelled
    subtotal INT NOT NULL, -- item total in paise
    tax_amount INT NOT NULL DEFAULT 0,
    delivery_fee INT NOT NULL DEFAULT 0,
    packaging_charge INT NOT NULL DEFAULT 0,
    discount_amount INT NOT NULL DEFAULT 0,
    total_amount INT NOT NULL,
    payment_status SMALLINT NOT NULL DEFAULT 0, -- 0=Pending, 1=Paid, 2=Failed, 3=Refunded, 4=PartialRefund
    payment_method SMALLINT, -- 1=UPI, 2=Card, 3=NetBanking, 4=Wallet, 5=COD, 6=PayAtRestaurant
    coupon_id UUID,
    delivery_address_id UUID REFERENCES user_addresses(id),
    delivery_partner_id UUID REFERENCES users(id),
    dine_in_table_id UUID, -- FK added after table creation
    estimated_delivery_time TIMESTAMPTZ,
    actual_delivery_time TIMESTAMPTZ,
    special_instructions TEXT,
    cancellation_reason VARCHAR(500),
    cancelled_by SMALLINT, -- 1=Customer, 2=Restaurant, 3=System
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    menu_item_id UUID NOT NULL REFERENCES menu_items(id),
    variant_id UUID REFERENCES menu_item_variants(id),
    item_name VARCHAR(200) NOT NULL, -- snapshot at order time
    quantity INT NOT NULL CHECK (quantity > 0),
    unit_price INT NOT NULL, -- price snapshot at order time
    total_price INT NOT NULL,
    special_instructions VARCHAR(500),
    status SMALLINT NOT NULL DEFAULT 0, -- for dine-in: 0=Pending, 1=Preparing, 2=Ready, 3=Served
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE order_item_addons (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_item_id UUID NOT NULL REFERENCES order_items(id) ON DELETE CASCADE,
    addon_id UUID NOT NULL REFERENCES menu_item_addons(id),
    addon_name VARCHAR(100) NOT NULL, -- snapshot
    quantity INT NOT NULL DEFAULT 1,
    price INT NOT NULL, -- snapshot
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE order_status_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    status SMALLINT NOT NULL,
    notes VARCHAR(500),
    changed_by UUID REFERENCES users(id),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 5. DINE-IN SPECIFIC
CREATE TABLE restaurant_tables (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    restaurant_id UUID NOT NULL REFERENCES restaurants(id) ON DELETE CASCADE,
    table_number VARCHAR(20) NOT NULL,
    capacity INT NOT NULL DEFAULT 4,
    floor_section VARCHAR(50), -- e.g., "Ground Floor", "Terrace"
    qr_code_data VARCHAR(500) NOT NULL UNIQUE, -- encoded QR identifier
    status SMALLINT NOT NULL DEFAULT 0, -- 0=Available, 1=Occupied, 2=Reserved, 3=Maintenance
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(restaurant_id, table_number)
);

CREATE TABLE dine_in_sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    restaurant_id UUID NOT NULL REFERENCES restaurants(id),
    table_id UUID NOT NULL REFERENCES restaurant_tables(id),
    customer_id UUID NOT NULL REFERENCES users(id),
    session_code VARCHAR(10) NOT NULL UNIQUE, -- shareable code for group ordering
    status SMALLINT NOT NULL DEFAULT 0, -- 0=Active, 1=BillRequested, 2=PaymentPending, 3=Completed, 4=Cancelled
    guest_count INT NOT NULL DEFAULT 1,
    started_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ended_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE dine_in_session_members (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    session_id UUID NOT NULL REFERENCES dine_in_sessions(id) ON DELETE CASCADE,
    user_id UUID NOT NULL REFERENCES users(id),
    role SMALLINT NOT NULL DEFAULT 1, -- 1=Host, 2=Guest
    joined_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Add FK from orders to restaurant_tables
ALTER TABLE orders ADD CONSTRAINT fk_orders_restaurant_tables
    FOREIGN KEY (dine_in_table_id) REFERENCES restaurant_tables(id);

-- 6. PAYMENTS
CREATE TABLE payments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id),
    payment_gateway VARCHAR(50) NOT NULL, -- razorpay, stripe
    gateway_payment_id VARCHAR(255) UNIQUE,
    gateway_order_id VARCHAR(255),
    gateway_signature VARCHAR(512),
    amount INT NOT NULL, -- in paise
    currency VARCHAR(3) NOT NULL DEFAULT 'INR',
    status SMALLINT NOT NULL DEFAULT 0, -- 0=Initiated, 1=Processing, 2=Success, 3=Failed, 4=Refunded
    method VARCHAR(50), -- upi, card, netbanking, wallet
    metadata JSONB, -- gateway response
    refund_amount INT DEFAULT 0,
    refund_reason VARCHAR(500),
    refunded_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 7. DELIVERY
CREATE TABLE delivery_assignments (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id),
    partner_id UUID NOT NULL REFERENCES users(id),
    status SMALLINT NOT NULL DEFAULT 0, -- 0=Assigned, 1=Accepted, 2=PickedUp, 3=EnRoute, 4=Delivered, 5=Cancelled
    assigned_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    accepted_at TIMESTAMPTZ,
    picked_up_at TIMESTAMPTZ,
    delivered_at TIMESTAMPTZ,
    current_latitude DOUBLE PRECISION,
    current_longitude DOUBLE PRECISION,
    distance_km DECIMAL(6,2),
    earnings INT NOT NULL DEFAULT 0, -- in paise
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE delivery_partner_locations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    partner_id UUID NOT NULL REFERENCES users(id),
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL,
    heading DOUBLE PRECISION,
    speed DOUBLE PRECISION,
    is_online BOOLEAN NOT NULL DEFAULT FALSE,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 8. REVIEWS & RATINGS
CREATE TABLE reviews (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id) UNIQUE,
    user_id UUID NOT NULL REFERENCES users(id),
    restaurant_id UUID NOT NULL REFERENCES restaurants(id),
    rating SMALLINT NOT NULL CHECK (rating >= 1 AND rating <= 5),
    review_text TEXT,
    delivery_rating SMALLINT CHECK (delivery_rating >= 1 AND delivery_rating <= 5),
    is_anonymous BOOLEAN NOT NULL DEFAULT FALSE,
    restaurant_reply TEXT,
    replied_at TIMESTAMPTZ,
    is_visible BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE review_photos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    review_id UUID NOT NULL REFERENCES reviews(id) ON DELETE CASCADE,
    photo_url VARCHAR(500) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 9. PROMOTIONS & COUPONS
CREATE TABLE coupons (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(20) NOT NULL UNIQUE,
    title VARCHAR(200) NOT NULL,
    description TEXT,
    discount_type SMALLINT NOT NULL, -- 1=Percentage, 2=FlatAmount
    discount_value INT NOT NULL, -- percentage*100 or paise
    max_discount INT, -- cap in paise
    min_order_amount INT NOT NULL DEFAULT 0,
    valid_from TIMESTAMPTZ NOT NULL,
    valid_until TIMESTAMPTZ NOT NULL,
    max_usages INT,
    max_usages_per_user INT NOT NULL DEFAULT 1,
    current_usages INT NOT NULL DEFAULT 0,
    applicable_order_types SMALLINT[] NOT NULL DEFAULT '{1,2,3}', -- delivery, dine-in, takeaway
    restaurant_ids UUID[], -- NULL = all restaurants
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE coupon_usages (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    coupon_id UUID NOT NULL REFERENCES coupons(id),
    user_id UUID NOT NULL REFERENCES users(id),
    order_id UUID NOT NULL REFERENCES orders(id),
    discount_amount INT NOT NULL,
    used_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 10. NOTIFICATIONS
CREATE TABLE user_devices (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    device_token VARCHAR(512) NOT NULL,
    platform SMALLINT NOT NULL, -- 1=Android, 2=iOS, 3=Web
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(user_id, device_token)
);

CREATE TABLE notifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id),
    title VARCHAR(200) NOT NULL,
    body TEXT NOT NULL,
    type SMALLINT NOT NULL, -- 1=OrderUpdate, 2=Promotion, 3=DineIn, 4=System
    data JSONB, -- deep link / metadata
    is_read BOOLEAN NOT NULL DEFAULT FALSE,
    read_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 11. RESTAURANT CUISINE JUNCTION
CREATE TABLE cuisine_types (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL UNIQUE,
    icon_url VARCHAR(500),
    sort_order INT NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE restaurant_cuisines (
    restaurant_id UUID NOT NULL REFERENCES restaurants(id) ON DELETE CASCADE,
    cuisine_id UUID NOT NULL REFERENCES cuisine_types(id) ON DELETE CASCADE,
    PRIMARY KEY (restaurant_id, cuisine_id)
);

-- 12. FAVORITES
CREATE TABLE user_favorites (
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    restaurant_id UUID NOT NULL REFERENCES restaurants(id) ON DELETE CASCADE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (user_id, restaurant_id)
);
```

### Index Strategy

```sql
-- USERS
CREATE INDEX idx_users_phone ON users(phone_number) WHERE is_deleted = FALSE;
CREATE INDEX idx_users_email ON users(email) WHERE email IS NOT NULL AND is_deleted = FALSE;
CREATE INDEX idx_users_role ON users(role) WHERE is_deleted = FALSE;

-- RESTAURANTS
CREATE INDEX idx_restaurants_location ON restaurants USING gist (
    ll_to_earth(latitude, longitude)
) WHERE is_deleted = FALSE AND status = 1;
CREATE INDEX idx_restaurants_owner ON restaurants(owner_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_restaurants_city ON restaurants(city) WHERE is_deleted = FALSE AND status = 1;
CREATE INDEX idx_restaurants_status ON restaurants(status) WHERE is_deleted = FALSE;
CREATE INDEX idx_restaurants_slug ON restaurants(slug);

-- MENU
CREATE INDEX idx_menu_items_restaurant ON menu_items(restaurant_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_menu_items_category ON menu_items(category_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_menu_categories_restaurant ON menu_categories(restaurant_id) WHERE is_active = TRUE;

-- ORDERS
CREATE INDEX idx_orders_user ON orders(user_id, created_at DESC) WHERE is_deleted = FALSE;
CREATE INDEX idx_orders_restaurant ON orders(restaurant_id, created_at DESC) WHERE is_deleted = FALSE;
CREATE INDEX idx_orders_status ON orders(restaurant_id, status) WHERE is_deleted = FALSE;
CREATE INDEX idx_orders_delivery_partner ON orders(delivery_partner_id) WHERE delivery_partner_id IS NOT NULL AND is_deleted = FALSE;
CREATE INDEX idx_orders_type_status ON orders(order_type, status) WHERE is_deleted = FALSE;
CREATE INDEX idx_orders_dine_in_table ON orders(dine_in_table_id) WHERE dine_in_table_id IS NOT NULL;

-- DINE-IN
CREATE INDEX idx_dine_in_sessions_table ON dine_in_sessions(table_id, status);
CREATE INDEX idx_dine_in_sessions_customer ON dine_in_sessions(customer_id, created_at DESC);
CREATE INDEX idx_restaurant_tables_restaurant ON restaurant_tables(restaurant_id) WHERE is_active = TRUE;
CREATE INDEX idx_restaurant_tables_qr ON restaurant_tables(qr_code_data);

-- DELIVERY
CREATE INDEX idx_delivery_assignments_order ON delivery_assignments(order_id);
CREATE INDEX idx_delivery_assignments_partner ON delivery_assignments(partner_id, status);
CREATE INDEX idx_delivery_partner_locations_partner ON delivery_partner_locations(partner_id);

-- REVIEWS
CREATE INDEX idx_reviews_restaurant ON reviews(restaurant_id, created_at DESC) WHERE is_visible = TRUE;
CREATE INDEX idx_reviews_user ON reviews(user_id, created_at DESC);

-- PAYMENTS
CREATE INDEX idx_payments_order ON payments(order_id);
CREATE INDEX idx_payments_gateway_id ON payments(gateway_payment_id) WHERE gateway_payment_id IS NOT NULL;

-- NOTIFICATIONS
CREATE INDEX idx_notifications_user ON notifications(user_id, created_at DESC);
CREATE INDEX idx_notifications_unread ON notifications(user_id) WHERE is_read = FALSE;

-- COUPONS
CREATE INDEX idx_coupons_code ON coupons(code) WHERE is_active = TRUE;
CREATE INDEX idx_coupon_usages_user ON coupon_usages(coupon_id, user_id);
```

### Instructions for Module 1

1. Create all EF Core entity configurations using Fluent API (not data annotations)
2. Create `AppDbContext` with all `DbSet<T>` properties
3. Implement `SaveChangesInterceptor` for automatic audit fields (`created_at`, `updated_at`)
4. Implement soft-delete query filter globally: `.HasQueryFilter(e => !e.IsDeleted)`
5. Generate initial EF Core migration
6. Create seed data for: cuisine types, admin user, sample restaurants (5), sample menu items (50+)
7. Verify migration applies cleanly to Dockerized PostgreSQL
8. **Update `context.md`**

---

## 🔐 MODULE 2: AUTH & IDENTITY

### Objective
Complete authentication system with phone OTP + email, JWT tokens, refresh token rotation, RBAC.

### Features
1. **Phone OTP registration/login** (simulate OTP for dev, integrate Twilio/MSG91 for prod)
2. **Email + password registration** (BCrypt hashing, min 8 chars, complexity requirements)
3. **JWT access tokens** (15 min expiry, RS256 signing)
4. **Refresh token rotation** (7-day sliding window, stored hashed in DB, detect token reuse)
5. **RBAC** — 4 roles: Customer, RestaurantOwner, DeliveryPartner, Admin
6. **Device management** — track active sessions, remote logout
7. **Rate limiting** — 5 OTP requests/hour per phone, 10 login attempts/15min per IP

### API Endpoints

```
POST   /api/v1/auth/register                  # Register with phone + OTP
POST   /api/v1/auth/register/email             # Register with email + password
POST   /api/v1/auth/login/phone                # Login with phone + OTP
POST   /api/v1/auth/login/email                # Login with email + password
POST   /api/v1/auth/otp/send                   # Send OTP to phone
POST   /api/v1/auth/otp/verify                 # Verify OTP
POST   /api/v1/auth/token/refresh              # Refresh access token
POST   /api/v1/auth/logout                     # Revoke current refresh token
POST   /api/v1/auth/logout/all                 # Revoke all refresh tokens (all devices)
GET    /api/v1/auth/me                         # Get current user profile
PUT    /api/v1/auth/me                         # Update profile
GET    /api/v1/auth/sessions                   # List active sessions
DELETE /api/v1/auth/sessions/{id}              # Revoke specific session
```

### Flutter Implementation
- Create `AuthRepository`, `AuthRemoteDataSource`, `AuthLocalDataSource`
- Store JWT in Flutter Secure Storage
- Dio interceptor for: auto-attach Bearer token, auto-refresh on 401, queue pending requests during refresh
- Riverpod `AuthNotifier` managing auth state: `Unauthenticated | Authenticating | Authenticated(User) | Error`
- GoRouter redirect guard: unauthenticated users → login, authenticated → home
- Login/Register screens with phone OTP flow and email flow

### Update `context.md` upon completion.

---

## 🍽️ MODULE 3: RESTAURANT MANAGEMENT

### Objective
Full restaurant onboarding, menu management, operating hours, and restaurant dashboard.

### Features
1. Restaurant registration & profile management
2. Menu category CRUD
3. Menu item CRUD with variants and addons
4. Operating hours management
5. Restaurant dashboard (orders summary, revenue)
6. Toggle accepting orders, dine-in availability
7. FSSAI / GST document upload (S3)

### API Endpoints

```
# Restaurant Management
POST   /api/v1/restaurants                     # Register restaurant
GET    /api/v1/restaurants/{id}                # Get restaurant details
PUT    /api/v1/restaurants/{id}                # Update restaurant
PATCH  /api/v1/restaurants/{id}/status         # Toggle accepting orders
PUT    /api/v1/restaurants/{id}/operating-hours # Set operating hours
POST   /api/v1/restaurants/{id}/logo           # Upload logo
POST   /api/v1/restaurants/{id}/banner         # Upload banner

# Menu Management
GET    /api/v1/restaurants/{id}/menu           # Full menu with categories
POST   /api/v1/restaurants/{id}/categories     # Create category
PUT    /api/v1/restaurants/{id}/categories/{cid}
DELETE /api/v1/restaurants/{id}/categories/{cid}
POST   /api/v1/restaurants/{id}/items          # Create menu item
PUT    /api/v1/restaurants/{id}/items/{iid}
DELETE /api/v1/restaurants/{id}/items/{iid}
PATCH  /api/v1/restaurants/{id}/items/{iid}/availability  # Toggle available
POST   /api/v1/restaurants/{id}/items/{iid}/variants
POST   /api/v1/restaurants/{id}/items/{iid}/addons

# Admin
GET    /api/v1/admin/restaurants               # List all (paginated, filterable)
PATCH  /api/v1/admin/restaurants/{id}/approve
PATCH  /api/v1/admin/restaurants/{id}/suspend
```

### Flutter Implementation
- Restaurant Owner dashboard with bottom navigation: Home, Menu, Orders, Dine-In, Profile
- Menu management with drag-and-drop reordering
- Image upload with compression before sending
- Real-time order count badge

### Update `context.md` upon completion.

---

## 🔍 MODULE 4: CUSTOMER DISCOVERY

### Objective
Home screen, restaurant listing, search, filters, restaurant detail page.

### Features
1. Home screen: banners, cuisines carousel, nearby restaurants, trending, new arrivals
2. Restaurant listing with filters (cuisine, rating, delivery time, cost, veg/non-veg)
3. Sort by: relevance, rating, delivery time, cost low-to-high, cost high-to-low
4. Restaurant detail page with full menu, grouped by category
5. Location-based restaurant discovery (within configurable radius)
6. Favorites / wishlisting

### API Endpoints

```
GET    /api/v1/home                            # Home feed (banners, curated lists)
GET    /api/v1/restaurants/nearby               # Nearby restaurants (lat, lng, radius)
GET    /api/v1/restaurants/search               # Search with filters
GET    /api/v1/restaurants/{slug}               # Restaurant detail (public)
GET    /api/v1/restaurants/{id}/menu            # Public menu
GET    /api/v1/cuisines                        # All cuisine types
POST   /api/v1/favorites/{restaurantId}        # Add to favorites
DELETE /api/v1/favorites/{restaurantId}        # Remove from favorites
GET    /api/v1/favorites                       # List favorites
```

### Redis Caching Strategy
- Cache home feed: 5 min TTL
- Cache restaurant detail: 2 min TTL, invalidate on update
- Cache menu: 2 min TTL, invalidate on item change
- Cache nearby restaurants result by geohash: 1 min TTL

### Flutter Implementation
- Home page with pull-to-refresh, shimmer loading
- Restaurant list with infinite scroll (cursor pagination)
- Filter bottom sheet
- Restaurant detail page: sticky header with parallax, tabbed menu categories
- Google Maps integration for location picker

### Update `context.md` upon completion.

---

## 🛒 MODULE 5: CART & ORDERING (DELIVERY)

### Objective
Cart management, checkout flow, order placement, order tracking with real-time updates.

### Features
1. Cart stored in Redis (fast, ephemeral) + sync to DB on order placement
2. Multi-restaurant cart prevention (or clear cart prompt)
3. Cart operations: add, update quantity, remove, clear
4. Checkout: address selection, payment method, coupon application, order summary
5. Order placement with stock/availability validation
6. Order status tracking with WebSocket real-time updates
7. Order history with reorder functionality
8. Order cancellation (with time window policy)

### API Endpoints

```
# Cart (Redis-backed)
GET    /api/v1/cart                            # Get current cart
POST   /api/v1/cart/items                      # Add item to cart
PUT    /api/v1/cart/items/{itemId}             # Update quantity
DELETE /api/v1/cart/items/{itemId}             # Remove item
DELETE /api/v1/cart                            # Clear cart
POST   /api/v1/cart/apply-coupon               # Apply coupon code
DELETE /api/v1/cart/coupon                     # Remove coupon

# Orders
POST   /api/v1/orders                         # Place order
GET    /api/v1/orders                          # Order history (cursor paginated)
GET    /api/v1/orders/{id}                     # Order detail
POST   /api/v1/orders/{id}/cancel              # Cancel order
POST   /api/v1/orders/{id}/reorder             # Reorder (copy items to cart)

# Restaurant Order Management
GET    /api/v1/restaurants/{id}/orders         # Incoming orders
PATCH  /api/v1/restaurants/{id}/orders/{oid}/accept
PATCH  /api/v1/restaurants/{id}/orders/{oid}/preparing
PATCH  /api/v1/restaurants/{id}/orders/{oid}/ready
PATCH  /api/v1/restaurants/{id}/orders/{oid}/reject

# WebSocket
WS     /ws/orders/{orderId}                   # Real-time order status
```

### Kafka Events

```
order.placed        → triggers: restaurant notification, payment initiation
order.confirmed     → triggers: delivery partner assignment
order.preparing     → triggers: customer notification
order.ready         → triggers: delivery partner notification
order.picked_up     → triggers: customer notification
order.delivered     → triggers: payment settlement, review prompt
order.cancelled     → triggers: refund, inventory rollback
```

### Flutter Implementation
- Cart page with item quantity stepper, variant/addon display
- Checkout flow: multi-step (address → payment → review → place)
- Order tracking page: stepper UI + map with delivery partner location
- WebSocket connection for real-time status updates
- Order history with search/filter

### Update `context.md` upon completion.

---

## 🪑 MODULE 6: DINE-IN (CUSTOMER EXPERIENCE)

### Objective
Complete customer-facing dine-in ordering experience from QR scan to bill payment.

### Features
1. **QR Code Scan** — Customer scans table QR code, opens restaurant menu
2. **Table Session** — Creates a dine-in session, generates shareable session code
3. **Group Ordering** — Multiple people join session via code, each orders independently
4. **In-Seat Ordering** — Browse menu, add to cart, place order from table
5. **Multi-Round Ordering** — Place additional orders during the same session
6. **Real-Time Order Status** — WebSocket updates: placed → preparing → ready → served
7. **Bill View** — Consolidated bill across all orders in session
8. **Bill Split** — Equal split, item-wise split, custom amount split
9. **Pay at Table** — Pay via UPI/Card without waiting for waiter

### API Endpoints

```
# Dine-In Session
POST   /api/v1/dine-in/scan                   # Scan QR → validate table → create/join session
GET    /api/v1/dine-in/session/{sessionId}     # Get session details
POST   /api/v1/dine-in/session/{sessionId}/join # Join session via code
GET    /api/v1/dine-in/session/{sessionId}/members
POST   /api/v1/dine-in/session/{sessionId}/leave

# Dine-In Ordering
GET    /api/v1/dine-in/session/{sessionId}/menu   # Menu for session's restaurant
POST   /api/v1/dine-in/session/{sessionId}/orders  # Place dine-in order
GET    /api/v1/dine-in/session/{sessionId}/orders  # All orders in session

# Bill & Payment
GET    /api/v1/dine-in/session/{sessionId}/bill    # Consolidated bill
POST   /api/v1/dine-in/session/{sessionId}/bill/split  # Calculate split
POST   /api/v1/dine-in/session/{sessionId}/bill/pay    # Pay bill
POST   /api/v1/dine-in/session/{sessionId}/complete    # End session

# WebSocket
WS     /ws/dine-in/{sessionId}                # Real-time session updates (orders, status)
```

### Flutter Implementation
- QR scan page (camera permission, scanner widget)
- Dine-in menu page (same menu component, dine-in context)
- Session lobby: show members, their orders, session code to share
- Multi-round cart: "Add more items" button persists session
- Real-time order cards: animated status transitions
- Bill summary page: itemized, with split options
- Payment integration for dine-in
- Deep linking: QR URL opens app directly to session

### WebSocket Events (Dine-In)

```
dine_in.member_joined       # New person joined the table
dine_in.member_left         # Person left the session
dine_in.order_placed        # New order placed by any member
dine_in.order_status_changed # Individual item status update
dine_in.bill_requested      # Host requested the bill
dine_in.payment_completed   # Payment received
dine_in.session_ended       # Session closed
```

### Update `context.md` upon completion.

---

## 👨‍🍳 MODULE 7: DINE-IN (RESTAURANT MANAGEMENT)

### Objective
Restaurant-facing dine-in management: table management, order queue, kitchen display system.

### Features
1. **Table Management** — Add/edit/remove tables, assign QR codes, view table status
2. **QR Code Generation** — Generate printable QR codes per table (with restaurant branding)
3. **Floor Plan View** — Visual grid/map of tables with color-coded status
4. **Incoming Dine-In Orders** — Real-time order queue from all active tables
5. **Kitchen Display System (KDS)** — Orders prioritized by time, mark items as preparing/ready/served
6. **Order Aggregation** — View all orders per table/session, consolidated view
7. **Table Session Management** — View active sessions, force-close session
8. **Bill Generation** — Generate and finalize bills for tables

### API Endpoints

```
# Table Management
POST   /api/v1/restaurants/{id}/tables         # Create table
GET    /api/v1/restaurants/{id}/tables         # List all tables with status
PUT    /api/v1/restaurants/{id}/tables/{tid}   # Update table
DELETE /api/v1/restaurants/{id}/tables/{tid}   # Deactivate table
POST   /api/v1/restaurants/{id}/tables/{tid}/qr  # Generate QR code
GET    /api/v1/restaurants/{id}/tables/{tid}/qr   # Get QR image

# Floor Plan
GET    /api/v1/restaurants/{id}/floor-plan     # All tables with positions + status

# Dine-In Order Management
GET    /api/v1/restaurants/{id}/dine-in/orders          # All active dine-in orders
GET    /api/v1/restaurants/{id}/dine-in/orders/kitchen   # KDS view (prioritized)
PATCH  /api/v1/restaurants/{id}/dine-in/orders/{oid}/items/{iid}/status  # Update item status
GET    /api/v1/restaurants/{id}/dine-in/sessions         # All active sessions
GET    /api/v1/restaurants/{id}/dine-in/sessions/{sid}   # Session detail with all orders
POST   /api/v1/restaurants/{id}/dine-in/sessions/{sid}/close  # Force close session

# WebSocket
WS     /ws/restaurant/{restaurantId}/dine-in   # Real-time dine-in updates for restaurant
```

### Flutter Implementation (Restaurant App)
- Table management page: grid view with color-coded status (green=available, red=occupied, yellow=reserved, grey=maintenance)
- QR code generation with restaurant branding, downloadable/printable
- Kitchen Display System: kanban-style columns (New → Preparing → Ready → Served)
- Real-time updates via WebSocket
- Table session detail: timeline of orders, member list
- Audio/haptic notification on new dine-in order

### Update `context.md` upon completion.

---

## 💳 MODULE 8: PAYMENTS

### Objective
Payment gateway integration, payment processing, refund handling, financial ledger.

### Features
1. Razorpay integration (primary) + Stripe (secondary)
2. Payment initiation → verification → confirmation flow
3. UPI, Credit/Debit Card, Net Banking, Wallet support
4. Automatic refund on cancellation
5. Payment ledger (double-entry bookkeeping pattern)
6. Restaurant settlement tracking
7. Delivery partner payout tracking

### API Endpoints

```
POST   /api/v1/payments/initiate               # Create payment order
POST   /api/v1/payments/verify                  # Verify payment signature
GET    /api/v1/payments/{id}                   # Payment status
POST   /api/v1/payments/{id}/refund            # Initiate refund

# Settlement (Admin)
GET    /api/v1/admin/settlements                # Restaurant settlements
POST   /api/v1/admin/settlements/process        # Process pending settlements
GET    /api/v1/admin/payouts                    # Delivery partner payouts
```

### Update `context.md` upon completion.

---

## 🔔 MODULE 9: NOTIFICATIONS

### Objective
Push notifications, in-app notifications, WebSocket real-time events, email/SMS.

### Features
1. FCM push notifications (Android, iOS, Web)
2. In-app notification center
3. WebSocket hub for real-time events
4. Email notifications (SendGrid/SES) for: welcome, order confirmation, receipts
5. SMS notifications for OTP + critical order updates
6. Notification preferences per user

### Key Implementation
- **SignalR** (or raw WebSocket) hub for .NET backend
- **WebSocket channel** in Flutter with auto-reconnect and heartbeat
- Kafka consumer for notification events → dispatch to FCM/email/SMS
- Notification deduplication and throttling

### Update `context.md` upon completion.

---

## 🚗 MODULE 10: DELIVERY PARTNER

### Objective
Delivery partner onboarding, order assignment, live tracking, earnings.

### Features
1. Delivery partner registration with document verification
2. Go online/offline toggle
3. Order assignment algorithm (nearest available partner, rating-weighted)
4. Accept/reject order assignment
5. Live location tracking (broadcast to customer via WebSocket)
6. Delivery proof (photo/signature)
7. Earnings dashboard

### API Endpoints

```
POST   /api/v1/delivery/register               # Register as delivery partner
PATCH  /api/v1/delivery/status                 # Toggle online/offline
POST   /api/v1/delivery/location               # Update current location
GET    /api/v1/delivery/orders/current          # Current active order
PATCH  /api/v1/delivery/orders/{id}/accept
PATCH  /api/v1/delivery/orders/{id}/reject
PATCH  /api/v1/delivery/orders/{id}/picked-up
PATCH  /api/v1/delivery/orders/{id}/delivered
POST   /api/v1/delivery/orders/{id}/proof       # Upload delivery proof
GET    /api/v1/delivery/earnings                # Earnings summary
GET    /api/v1/delivery/earnings/history        # Detailed earnings history
```

### Update `context.md` upon completion.

---

## 🛡️ MODULE 11: ADMIN PANEL

### Objective
Super admin dashboard for platform management, analytics, moderation.

### Features
1. Dashboard: total orders, revenue, active users, active restaurants (with time range filters)
2. User management: list, search, suspend, role changes
3. Restaurant management: approve/reject, commission management
4. Order management: view all orders, resolve disputes
5. Coupon management
6. Analytics: revenue charts, order volume, popular restaurants, peak hours
7. System health: API response times, error rates (integrate Prometheus data)

### API Endpoints

```
GET    /api/v1/admin/dashboard                  # Dashboard KPIs
GET    /api/v1/admin/users                     # User list (paginated, searchable)
GET    /api/v1/admin/analytics/revenue          # Revenue analytics
GET    /api/v1/admin/analytics/orders           # Order analytics
GET    /api/v1/admin/analytics/restaurants      # Restaurant analytics
# + CRUD endpoints for all entities
```

### Flutter Implementation
- Admin web dashboard (Flutter Web optimized with responsive layout)
- Charts using `fl_chart` package
- DataTable with server-side pagination, sorting, filtering

### Update `context.md` upon completion.

---

## 🔎 MODULE 12: SEARCH & DISCOVERY (ELASTICSEARCH)

### Objective
Full-text search with autocomplete, filters, and relevance ranking via Elasticsearch.

### Features
1. Index restaurants and menu items in Elasticsearch
2. Autocomplete (prefix + fuzzy matching)
3. Filters: cuisine, rating, delivery time, cost, veg/non-veg, dine-in available
4. Relevance ranking: distance + rating + popularity
5. Sync from PostgreSQL via Kafka events (eventual consistency)

### Elasticsearch Index Mappings

```json
{
  "restaurants": {
    "name": { "type": "text", "analyzer": "autocomplete" },
    "cuisine_types": { "type": "keyword" },
    "location": { "type": "geo_point" },
    "average_rating": { "type": "float" },
    "is_dine_in_enabled": { "type": "boolean" },
    "avg_cost_for_two": { "type": "integer" },
    "is_veg_only": { "type": "boolean" },
    "status": { "type": "keyword" }
  }
}
```

### Update `context.md` upon completion.

---

## ⭐ MODULE 13: RATINGS & REVIEWS

### Features
- Post-order review prompt (after delivery/dine-in completion)
- 1-5 star rating for food + separate delivery rating
- Text review + photo upload
- Restaurant can reply to reviews
- Review moderation (admin can hide inappropriate reviews)
- Average rating recalculation (background job)

### Update `context.md` upon completion.

---

## 🎟️ MODULE 14: PROMOTIONS & COUPONS

### Features
- Coupon engine: percentage discount, flat discount, max cap
- Validity period, usage limits (global + per user)
- Restaurant-specific or platform-wide coupons
- Applicable to specific order types (delivery, dine-in, takeaway)
- Auto-apply best coupon suggestion
- Referral codes

### Update `context.md` upon completion.

---

## 📊 MODULE 15: OBSERVABILITY

### Objective
Production-grade logging, metrics, tracing, alerting.

### Implementation

1. **Structured Logging (Serilog)**
   - JSON format with: timestamp, level, service, traceId, spanId, message, context
   - Log to console (dev) + Elasticsearch/ELK (prod)
   - Correlation ID propagation across all logs

2. **Metrics (Prometheus + Grafana)**
   - Expose `/metrics` endpoint
   - RED metrics: request rate, error rate, duration (p50, p95, p99)
   - Custom: orders/min, active dine-in sessions, payment success rate
   - Grafana dashboards for all services

3. **Tracing (OpenTelemetry)**
   - Auto-instrument HTTP, gRPC, EF Core, Redis, Kafka
   - Manual spans for business logic (order processing, payment verification)
   - Export to Jaeger/Zipkin

4. **Health Checks**
   - `/health` — liveness
   - `/health/ready` — readiness (DB, Redis, Kafka, ES connectivity)

5. **Alerting**
   - Error rate > 1% for 5 min → P1
   - p99 latency > 2s for 5 min → P2
   - Payment failure rate > 5% → P1
   - Kafka consumer lag > 1000 → P2

### Update `context.md` upon completion.

---

## 🚀 MODULE 16: DEVOPS & DEPLOYMENT

### Objective
CI/CD pipelines, Terraform IaC, Kubernetes manifests, Helm charts.

### GitHub Actions CI/CD

```yaml
# backend-ci.yml triggers: push to main, PR to main
# Steps: restore → build → test → lint → security scan → docker build → push to ECR → deploy

# frontend-ci.yml triggers: push to main, PR to main
# Steps: flutter analyze → flutter test → build web → build APK → build iOS → upload artifacts
```

### Terraform (AWS)

```
modules/
├── vpc/           # VPC, subnets, NAT gateway, security groups
├── eks/           # EKS cluster, node groups, IRSA
├── rds/           # PostgreSQL RDS (Multi-AZ)
├── elasticache/   # Redis cluster
├── msk/           # Managed Kafka (MSK)
├── opensearch/    # Elasticsearch (OpenSearch)
├── s3/            # Media storage + CloudFront CDN
├── ecr/           # Container registry
└── monitoring/    # CloudWatch, Prometheus workspace
```

### Kubernetes Manifests
- Deployment + Service + HPA for API
- ConfigMap + Secrets (External Secrets Operator)
- Ingress (ALB Ingress Controller)
- Network Policies (default deny + explicit allow)
- Pod Disruption Budgets
- Resource requests/limits on all pods

### Helm Chart

```
helm/swiggy-clone/
├── Chart.yaml
├── values.yaml
├── values-dev.yaml
├── values-staging.yaml
├── values-prod.yaml
├── templates/
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── hpa.yaml
│   ├── ingress.yaml
│   ├── configmap.yaml
│   ├── secrets.yaml
│   ├── pdb.yaml
│   ├── networkpolicy.yaml
│   └── _helpers.tpl
└── values.schema.json
```

### Update `context.md` upon completion.

---

## 📝 CONTEXT.MD FILE FORMAT

The `context.md` file must always maintain this structure:

```markdown
# Project Context — Swiggy Clone Enterprise Platform

## Global State
- **Project Root**: /path/to/swiggy-clone
- **Last Updated**: [ISO timestamp]
- **Current Phase**: Module [N] — [Name]
- **Overall Progress**: [N]/16 modules complete

## Environment
- **.NET SDK**: 9.x
- **Flutter SDK**: [version]
- **Docker Compose**: running / stopped
- **Database Migration**: [last migration name]
- **Seed Data**: applied / not applied

## Completed Modules
[Each module entry as defined in the context preservation protocol above]

## Active Module
- **Module**: [current module name]
- **Status**: IN_PROGRESS
- **Started At**: [timestamp]
- **Sub-tasks Completed**:
  - [x] Task 1
  - [ ] Task 2
  - [ ] Task 3

## Known Issues / Tech Debt
- [issue 1]
- [issue 2]

## Configuration Summary
- **API Base URL**: http://localhost:5000
- **WebSocket URL**: ws://localhost:5000/ws
- **PostgreSQL**: localhost:5432/swiggy_clone
- **Redis**: localhost:6379
- **Kafka**: localhost:9092
- **Elasticsearch**: localhost:9200

## Key Decisions Made
- [decision 1 with rationale]
- [decision 2 with rationale]
```

---

## 🔁 SESSION RESTART PROTOCOL

When Claude Code is restarted or begins a new session:

```
1. READ context.md → understand current state
2. VERIFY file structure → ls -la to confirm expected files exist
3. RUN tests → dotnet test (verify green state)
4. CHECK Docker → docker compose ps (verify services running)
5. READ last completed module's files → understand patterns established
6. CONTINUE from the exact point recorded in context.md
7. DO NOT recreate files that already exist unless explicitly asked
8. DO NOT change established patterns (naming, structure, architecture) without discussion
```

---

## ⚡ EXECUTION INSTRUCTIONS

1. Execute modules **strictly in order** (0 → 16). Each module depends on the previous.
2. After each module, **run all existing tests** to ensure no regression.
3. After each module, **update `context.md`** with full details.
4. Use **git commits** after each module: `git commit -m "feat(module-N): [module name] complete"`
5. If a module is too large for a single session, break it into sub-modules and track in `context.md`.
6. Ask for clarification if a business requirement is ambiguous — don't assume.
7. Prefer generating complete, runnable code over stubs or TODOs.
8. Every API endpoint must be testable via Swagger UI immediately after creation.

---

## 🎯 SUCCESS CRITERIA

Each module is complete when:
- [ ] All listed API endpoints are implemented and return correct responses
- [ ] All database tables/migrations are applied
- [ ] All Flutter pages/widgets are created with proper state management
- [ ] Unit test stubs are created (minimum happy path + 2 edge cases per service)
- [ ] Docker Compose services start without errors
- [ ] Swagger UI shows all endpoints with correct request/response schemas
- [ ] `context.md` is updated with all created files, endpoints, and tables
- [ ] No compilation errors or warnings in either backend or frontend
- [ ] Existing tests from previous modules still pass (no regression)

---

*This prompt is designed for a team of 1000 developers building an enterprise-grade food delivery + dine-in platform. Execute with precision.*
