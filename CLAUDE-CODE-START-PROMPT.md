# Claude Code — Start Building the Swiggy Clone Platform

## Read These Files First
Before writing ANY code, read these files in order:
1. `CLAUDE-CODE-MASTER-PROMPT.md` — full architecture, schema, API specs, folder structures, and module breakdown
2. `context.md` — current project state (read at start of EVERY session, update after EVERY module)
3. `SKILL.md` — coding standards, architecture principles, anti-patterns to avoid

## Who You Are
You are a principal-level engineer (25+ years experience) building an enterprise-grade, production-ready Swiggy clone — a hyperlocal food delivery + dine-in ordering platform supporting Web, Android, and iOS.

## Tech Stack (Non-Negotiable)
- **Frontend**: Flutter (Dart 3+) · Riverpod · GoRouter · Dio · Freezed · Material 3
- **Backend**: .NET 9 ASP.NET Core Web API · Clean Architecture · CQRS + MediatR · EF Core 9 · FluentValidation
- **Database**: PostgreSQL 16 · Redis 7 · Elasticsearch 8 · Apache Kafka
- **Infra**: Docker (multi-stage, non-root) · Docker Compose (local dev) · Kubernetes · Terraform · GitHub Actions

## What to Build
The platform has 3 apps in a single Flutter codebase:
1. **Customer App** — food delivery ordering + dine-in ordering (scan QR, order from table, split bill, pay)
2. **Restaurant App** — menu management + delivery order management + dine-in table/order/kitchen management
3. **Admin Panel** — platform analytics, user/restaurant moderation, settlements

## How to Build — Module Execution Order
Execute strictly in order. Each module depends on the previous. Full specs for each module are in `CLAUDE-CODE-MASTER-PROMPT.md`.

| # | Module | Key Deliverables |
|---|--------|-----------------|
| 0 | Project Scaffold | Monorepo, .NET solution, Flutter project, Docker Compose, base classes |
| 1 | Database Foundation | All EF Core entities, migrations, seed data, indexes |
| 2 | Auth & Identity | Phone OTP + email login, JWT + refresh tokens, RBAC |
| 3 | Restaurant Management | Restaurant CRUD, menu/category/variant/addon management |
| 4 | Customer Discovery | Home feed, nearby restaurants, search, filters, favorites |
| 5 | Cart & Ordering | Redis cart, checkout, order placement, real-time tracking (WebSocket) |
| 6 | Dine-In (Customer) | QR scan → table session → group ordering → bill split → pay at table |
| 7 | Dine-In (Restaurant) | Table management, QR generation, kitchen display system, order queue |
| 8 | Payments | Razorpay/Stripe integration, refunds, settlement ledger |
| 9 | Notifications | FCM push, WebSocket hub, in-app notifications, email/SMS |
| 10 | Delivery Partner | Onboarding, assignment algorithm, live tracking, earnings |
| 11 | Admin Panel | Dashboard KPIs, user/restaurant management, analytics charts |
| 12 | Search (Elasticsearch) | Full-text search, autocomplete, geo-filtered results |
| 13 | Ratings & Reviews | Post-order reviews, photos, restaurant replies |
| 14 | Promotions & Coupons | Coupon engine, referrals, auto-apply best discount |
| 15 | Observability | Serilog structured logging, Prometheus metrics, OpenTelemetry tracing |
| 16 | DevOps | CI/CD pipelines, Terraform AWS modules, K8s manifests, Helm chart |

## Critical Rules
1. **Context preservation** — After completing each module, update `context.md` with all files created, tables, endpoints, env vars, and pending issues. On session restart, ALWAYS read `context.md` first.
2. **No shortcuts** — Production-grade code only. SOLID principles, proper error handling, structured logging, database constraints, index strategies.
3. **Complete code** — Generate full, runnable implementations — not stubs or TODOs. Every endpoint must work in Swagger immediately.
4. **Test stubs** — Every service gets unit test stubs (happy path + edge cases + error cases) using xUnit (.NET) and Patrol (Flutter).
5. **Git commits** — After each module: `git commit -m "feat(module-N): [name] complete"`

## Start Now
Begin with **Module 0: Project Scaffold**. Create the full monorepo structure, .NET solution with all 5 projects, Flutter project, Docker Compose (Postgres + Redis + Kafka + Elasticsearch + Nginx), multi-stage Dockerfile, and all base classes (BaseEntity, Result<T>, MediatR pipeline behaviors, global exception middleware, correlation ID middleware).

Go.
