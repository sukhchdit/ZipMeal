# Project Context — Swiggy Clone Enterprise Platform

## Global State
- **Project Root**: ./swiggy-clone
- **Last Updated**: 2026-02-26
- **Current Phase**: Module 11 — Admin Panel
- **Overall Progress**: 12/16 modules complete

## Environment
- **.NET SDK**: 9.x (required)
- **Flutter SDK**: latest stable (required)
- **Docker Compose**: configured (not yet started — run `docker compose up` to verify)
- **Database Migration**: pending (run `dotnet ef migrations add InitialCreate` after restore)
- **Seed Data**: DataSeeder.cs wired into Program.cs (auto-seeds in Development mode)

## Tech Stack Summary
- **Frontend**: Flutter + Dart 3+ | Riverpod | GoRouter | Dio | Freezed | Material 3
- **Backend**: .NET 9 ASP.NET Core Web API | Clean Architecture | CQRS + MediatR | EF Core 9
- **Database**: PostgreSQL 16 | Redis 7 | Elasticsearch 8 | Apache Kafka
- **Infrastructure**: Docker | Kubernetes (EKS) | Terraform | GitHub Actions | Nginx

## Completed Modules

---
### Module: Module 0 — Project Scaffold & Docker Base
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created**: ~84 files (see previous context for full list)
- **Summary**: Monorepo structure, .NET 9 solution (5 src + 3 test projects), Flutter project with core infrastructure, Docker Compose (7 services), CI/CD workflows, base classes (BaseEntity, Result<T>, MediatR behaviors, middleware)

---
### Module: Module 1 — Database Foundation
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created**:
  - **Domain Entities (28 files)** in `src/SwiggyClone.Domain/Entities/`:
    - User.cs, UserAddress.cs, RefreshToken.cs
    - Restaurant.cs, RestaurantOperatingHours.cs, CuisineType.cs
    - MenuCategory.cs, MenuItem.cs, MenuItemVariant.cs, MenuItemAddon.cs
    - Order.cs, OrderItem.cs, OrderItemAddon.cs, OrderStatusHistory.cs
    - RestaurantTable.cs, DineInSession.cs, DineInSessionMember.cs
    - Payment.cs, DeliveryAssignment.cs, DeliveryPartnerLocation.cs
    - Review.cs, ReviewPhoto.cs
    - Coupon.cs, CouponUsage.cs
    - UserDevice.cs, Notification.cs
    - RestaurantCuisine.cs (junction), UserFavorite.cs (junction)
  - **EF Core Configurations (28 files)** in `src/SwiggyClone.Infrastructure/Persistence/Configurations/`:
    - One `IEntityTypeConfiguration<T>` per entity with Fluent API
    - All string lengths, defaults, FK behaviors, indexes match SQL schema
  - **Updated Files**:
    - `AppDbContext.cs` — 28 DbSet<T> properties added
  - **New Files**:
    - `DataSeeder.cs` — Seed data with 15 cuisine types, 1 admin user, 5 restaurant owners, 5 restaurants, 35 operating hours, 10 tables, 15 menu categories, 55 menu items, 14 variants, 15 addons, 12 restaurant-cuisine junctions
- **Database Tables (28 tables)**:
  - users, user_addresses, refresh_tokens
  - restaurants, restaurant_operating_hours, cuisine_types, restaurant_cuisines
  - menu_categories, menu_items, menu_item_variants, menu_item_addons
  - orders, order_items, order_item_addons, order_status_history
  - restaurant_tables, dine_in_sessions, dine_in_session_members
  - payments
  - delivery_assignments, delivery_partner_locations
  - reviews, review_photos
  - coupons, coupon_usages
  - user_devices, notifications
  - user_favorites
- **Index Strategy**: 30+ indexes including partial indexes with WHERE filters
- **API Endpoints Created**: none new (Module 1 is DB-only)
- **Docker Services Added**: none new
- **Dependencies Added**: none new
- **Pending/Known Issues**:
  - EF Core migration not yet generated (requires `dotnet restore` first, then `dotnet ef migrations add InitialCreate`)
  - DataSeeder not yet wired into Program.cs startup (will be done in Module 2 or manually)
  - Bash shell not functional in Claude Code session
- **Next Module Prerequisites**:
  - Run `dotnet restore` in `backend/`
  - Run `dotnet ef migrations add InitialCreate -p src/SwiggyClone.Infrastructure -s src/SwiggyClone.Api`
  - Run `dotnet ef database update -s src/SwiggyClone.Api` (with Docker Postgres running)

---
### Module: Module 2 — Auth & Identity
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created/Modified**:
  - **Domain Changes (1 file)**:
    - `User.cs` — Added `PasswordHash` property (nullable string for email+password auth)
  - **Application Layer — Interfaces (4 files)** in `src/SwiggyClone.Application/Common/Interfaces/`:
    - `ITokenService.cs` — GenerateAccessToken, GenerateRefreshToken, HashToken
    - `IOtpService.cs` — GenerateAndSendOtpAsync, VerifyOtpAsync
    - `IPasswordHasher.cs` — Hash, Verify
    - `IAppDbContext.cs` — DbSet<User>, DbSet<RefreshToken>, SaveChangesAsync
  - **Application Layer — DTOs (3 files)** in `src/SwiggyClone.Application/Features/Auth/DTOs/`:
    - `AuthResponse.cs`, `UserDto.cs`, `SessionDto.cs`
  - **Application Layer — Commands (27 files)** in `src/SwiggyClone.Application/Features/Auth/Commands/`:
    - RegisterByPhone (Command + Handler + Validator)
    - RegisterByEmail (Command + Handler + Validator)
    - LoginByPhone (Command + Handler + Validator)
    - LoginByEmail (Command + Handler + Validator)
    - SendOtp (Command + Handler + Validator)
    - RefreshToken (Command + Handler + Validator) — with reuse detection
    - Logout (Command + Handler)
    - LogoutAll (Command + Handler)
    - UpdateProfile (Command + Handler + Validator)
    - RevokeSession (Command + Handler)
  - **Application Layer — Queries (4 files)** in `src/SwiggyClone.Application/Features/Auth/Queries/`:
    - GetCurrentUser (Query + Handler)
    - GetActiveSessions (Query + Handler)
  - **Infrastructure Layer — Services (3 files)** in `src/SwiggyClone.Infrastructure/Services/`:
    - `JwtTokenService.cs` — HMAC-SHA256 JWT generation, SHA-256 refresh token hashing
    - `DevOtpService.cs` — Fixed OTP "123456" for dev, Redis-backed with 5-min TTL
    - `BcryptPasswordHasher.cs` — BCrypt with work factor 12
  - **Infrastructure Layer — Updates (4 files)**:
    - `AppDbContext.cs` — Implements IAppDbContext
    - `UserConfiguration.cs` — Added PasswordHash column (max 255 chars)
    - `DependencyInjection.cs` — Registered ITokenService, IOtpService, IPasswordHasher, IAppDbContext
    - `SwiggyClone.Infrastructure.csproj` — Added Microsoft.IdentityModel.Tokens 8.3.0
  - **API Layer (4 files)**:
    - `Controllers/AuthController.cs` — 13 endpoints (see below)
    - `Contracts/Auth/AuthRequests.cs` — 8 request DTOs
    - `Authorization/AuthorizationPolicies.cs` — RBAC policies (Admin, RestaurantOwner, DeliveryPartner, Customer)
    - `Program.cs` — Added DataSeeder wiring, authorization policies, async RunAsync
  - **Application .csproj Update**:
    - Added Microsoft.EntityFrameworkCore 9.0.0 package reference
    - Changed Assembly.GetCallingAssembly() to GetExecutingAssembly() in DependencyInjection.cs
  - **Flutter Auth Feature (10 files)** in `frontend/swiggy_clone_app/lib/features/auth/`:
    - `data/models/auth_response_model.dart` — Freezed AuthResponseModel + UserModel
    - `data/models/session_model.dart` — Freezed SessionModel
    - `data/datasources/auth_remote_data_source.dart` — Dio-based remote data source
    - `data/repositories/auth_repository.dart` — Repository with error handling
    - `presentation/providers/auth_state.dart` — Freezed sealed AuthState (5 variants)
    - `presentation/providers/auth_notifier.dart` — Riverpod AuthNotifier (keepAlive)
    - `presentation/providers/otp_notifier.dart` — OTP send/resend with countdown timer
    - `presentation/screens/login_screen.dart` — Phone/Email toggle login
    - `presentation/screens/register_screen.dart` — Phone OTP / Email+password registration
    - `presentation/screens/otp_verification_screen.dart` — 6-digit OTP input with auto-verify
  - **Flutter Updates (3 files)**:
    - `core/constants/api_constants.dart` — Expanded to 12 auth endpoint constants
    - `core/network/api_interceptors.dart` — Updated public endpoints list
    - `routing/app_router.dart` — Wired real auth screens replacing placeholders
- **API Endpoints Created (13)**:
  ```
  POST   /api/v1/auth/register           [Anonymous] Register with phone + OTP
  POST   /api/v1/auth/register/email     [Anonymous] Register with email + password
  POST   /api/v1/auth/login/phone        [Anonymous] Login with phone + OTP
  POST   /api/v1/auth/login/email        [Anonymous] Login with email + password
  POST   /api/v1/auth/otp/send           [Anonymous] Send OTP to phone
  POST   /api/v1/auth/otp/verify         [Anonymous] Verify OTP
  POST   /api/v1/auth/token/refresh      [Anonymous] Refresh access token
  POST   /api/v1/auth/logout             [Authorized] Revoke current refresh token
  POST   /api/v1/auth/logout/all         [Authorized] Revoke all sessions
  GET    /api/v1/auth/me                 [Authorized] Get current user profile
  PUT    /api/v1/auth/me                 [Authorized] Update profile
  GET    /api/v1/auth/sessions           [Authorized] List active sessions
  DELETE /api/v1/auth/sessions/{id}      [Authorized] Revoke specific session
  ```
- **Auth Features**:
  - JWT access tokens: HMAC-SHA256, 15-min expiry, claims: sub, jti, role, phone, email
  - Refresh token rotation: 7-day sliding window, SHA-256 hashed in DB, reuse detection (revokes ALL tokens on reuse)
  - Phone OTP: Dev mode fixed "123456", Redis-cached with 5-min TTL
  - Email + password: BCrypt hashing (work factor 12), complexity validation
  - RBAC: 4 roles with authorization policies (AdminOnly, RestaurantOwner, DeliveryPartner, CustomerOnly)
  - Device tracking via User-Agent header on login
  - Session management: list/revoke active sessions
- **Dependencies Added**:
  - Backend: Microsoft.EntityFrameworkCore 9.0.0 (Application), Microsoft.IdentityModel.Tokens 8.3.0 (Infrastructure)
  - Flutter: No new dependencies (uses existing dio, riverpod, freezed, go_router, secure_storage)
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation: `dart run build_runner build --delete-conflicting-outputs`
  - EF Core migration needs regeneration to include PasswordHash column
  - Dev OTP service uses fixed "123456" — replace with Twilio/MSG91 for production
- **Next Module Prerequisites**:
  - Run `dotnet restore` in `backend/`
  - Generate new migration: `dotnet ef migrations add AddPasswordHash -p src/SwiggyClone.Infrastructure -s src/SwiggyClone.Api`
  - Run Flutter code gen: `cd frontend/swiggy_clone_app && dart run build_runner build --delete-conflicting-outputs`

---
### Module: Module 3 — Restaurant Management
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created/Modified**:
  - **Backend — Application Layer — Helpers (2 files)** in `src/SwiggyClone.Application/Common/Helpers/`:
    - `RestaurantOwnershipHelper.cs` — Validates restaurant belongs to current user
    - `SlugHelper.cs` — Generates URL-safe slugs from restaurant names
  - **Backend — Application Layer — Interfaces (1 file)**:
    - `IFileStorageService.cs` — File upload abstraction for S3/local storage
  - **Backend — Application Layer — DTOs (12 files)** in `src/SwiggyClone.Application/Features/Restaurants/DTOs/`:
    - `RestaurantDto.cs`, `RestaurantSummaryDto.cs`, `RestaurantDashboardDto.cs`
    - `MenuCategoryDto.cs`, `MenuItemDto.cs`, `MenuItemSummaryDto.cs`
    - `MenuItemVariantDto.cs`, `MenuItemAddonDto.cs`
    - `OperatingHoursDto.cs`, `CuisineTypeDto.cs`, `FileUploadResultDto.cs`
    - `CreateVariantDto.cs`, `CreateAddonDto.cs`
  - **Backend — Application Layer — Commands (18 command sets)** in `src/SwiggyClone.Application/Features/Restaurants/Commands/`:
    - RegisterRestaurant, UpdateRestaurantProfile, ToggleAcceptingOrders, ToggleDineIn
    - UpsertOperatingHours, UploadRestaurantFile
    - CreateMenuCategory, UpdateMenuCategory, DeleteMenuCategory
    - CreateMenuItem, UpdateMenuItem, DeleteMenuItem
    - AddMenuItemVariant, UpdateMenuItemVariant, DeleteMenuItemVariant
    - AddMenuItemAddon, UpdateMenuItemAddon, DeleteMenuItemAddon
  - **Backend — Application Layer — Queries (8 query sets)** in `src/SwiggyClone.Application/Features/Restaurants/Queries/`:
    - GetMyRestaurants, GetRestaurantById, GetRestaurantDashboard
    - GetMenuCategories, GetMenuItemsByCategory, GetMenuItemById
    - GetOperatingHours, GetCuisineTypes
  - **Backend — API Layer (4 files)**:
    - `Controllers/RestaurantsController.cs` — 20+ endpoints (full REST)
    - `Controllers/CuisinesController.cs` — Public cuisine types endpoint
    - `Contracts/Restaurants/RestaurantRequests.cs` — Request DTOs
    - `Contracts/Restaurants/MenuCategoryRequests.cs`, `MenuItemRequests.cs`, `OperatingHoursRequests.cs`
  - **Backend — Infrastructure (1 file)**:
    - `Services/LocalFileStorageService.cs` — Saves uploads to wwwroot/uploads
  - **Flutter — Data Layer (9 files)** in `features/restaurant_management/data/`:
    - `datasources/restaurant_remote_data_source.dart` — Full API coverage (20+ methods)
    - `repositories/restaurant_repository.dart` — DioException → Failure mapping
    - `models/restaurant_model.dart`, `restaurant_summary_model.dart`, `restaurant_dashboard_model.dart`
    - `models/menu_category_model.dart`, `menu_item_model.dart` (includes Variant/Addon models)
    - `models/operating_hours_model.dart`, `cuisine_type_model.dart`, `file_upload_result_model.dart`
  - **Flutter — Providers (14 files)** in `features/restaurant_management/presentation/providers/`:
    - `cuisine_types_provider.dart`
    - `dashboard_notifier.dart` / `dashboard_state.dart`
    - `menu_categories_notifier.dart` / `menu_categories_state.dart`
    - `menu_items_notifier.dart` / `menu_items_state.dart`
    - `operating_hours_notifier.dart` / `operating_hours_state.dart`
    - `restaurant_detail_notifier.dart` / `restaurant_detail_state.dart`
    - `restaurant_list_notifier.dart` / `restaurant_list_state.dart`
  - **Flutter — Screens (9 files)** in `features/restaurant_management/presentation/screens/`:
    - `restaurant_list_screen.dart` — My restaurants list with pull-to-refresh
    - `restaurant_register_screen.dart` — Full registration form with cuisine multi-select
    - `restaurant_dashboard_screen.dart` — Stats grid, toggles, management navigation tiles
    - `restaurant_edit_screen.dart` — Pre-filled edit form
    - `menu_categories_screen.dart` — Category CRUD with bottom-sheet dialogs
    - `menu_items_screen.dart` — Item list with swipe-to-delete
    - `menu_item_form_screen.dart` — Create/edit items with inline variants & addons management
    - `operating_hours_screen.dart` — Weekly schedule with time pickers and closed toggle
    - `document_upload_screen.dart` — Logo, banner, FSSAI, GST upload with camera/gallery picker
  - **Flutter — Widgets (7 files)** in `features/restaurant_management/presentation/widgets/`:
    - `restaurant_card.dart`, `menu_category_tile.dart`, `menu_item_tile.dart`
    - `dashboard_stat_card.dart`, `operating_hour_row.dart`
    - `image_upload_widget.dart`, `addon_chip.dart`, `variant_chip.dart`
  - **Flutter — Routing (2 files modified)**:
    - `routing/route_names.dart` — Added 9 route constants + 7 helper methods for restaurant management
    - `routing/app_router.dart` — Added GoRoute tree for all 9 restaurant management screens
  - **Flutter — API Constants** (already complete from earlier):
    - `core/constants/api_constants.dart` — 18 restaurant management endpoint methods
- **API Endpoints (Backend — 20+)**:
  ```
  POST   /api/v1/restaurants              [RestaurantOwner] Register new restaurant
  GET    /api/v1/restaurants/my            [RestaurantOwner] List owner's restaurants
  GET    /api/v1/restaurants/{id}          [RestaurantOwner] Get restaurant detail
  PUT    /api/v1/restaurants/{id}          [RestaurantOwner] Update restaurant profile
  PUT    /api/v1/restaurants/{id}/accepting-orders  [RestaurantOwner] Toggle accepting orders
  PUT    /api/v1/restaurants/{id}/dine-in  [RestaurantOwner] Toggle dine-in
  GET    /api/v1/restaurants/{id}/dashboard [RestaurantOwner] Dashboard stats
  POST   /api/v1/restaurants/{id}/upload/{fileType} [RestaurantOwner] Upload file
  GET    /api/v1/restaurants/{id}/menu-categories   [RestaurantOwner] List categories
  POST   /api/v1/restaurants/{id}/menu-categories   [RestaurantOwner] Create category
  PUT    /api/v1/restaurants/{id}/menu-categories/{cid} [RestaurantOwner] Update category
  DELETE /api/v1/restaurants/{id}/menu-categories/{cid} [RestaurantOwner] Delete category
  GET    /api/v1/restaurants/{id}/menu-categories/{cid}/items [RestaurantOwner] List items
  POST   /api/v1/restaurants/{id}/menu-items        [RestaurantOwner] Create item
  GET    /api/v1/restaurants/{id}/menu-items/{iid}   [RestaurantOwner] Get item detail
  PUT    /api/v1/restaurants/{id}/menu-items/{iid}   [RestaurantOwner] Update item
  DELETE /api/v1/restaurants/{id}/menu-items/{iid}   [RestaurantOwner] Delete item
  POST   /api/v1/restaurants/{id}/menu-items/{iid}/variants  [RestaurantOwner] Add variant
  PUT    /api/v1/restaurants/{id}/menu-items/{iid}/variants/{vid}  [RestaurantOwner] Update variant
  DELETE /api/v1/restaurants/{id}/menu-items/{iid}/variants/{vid}  [RestaurantOwner] Delete variant
  POST   /api/v1/restaurants/{id}/menu-items/{iid}/addons    [RestaurantOwner] Add addon
  PUT    /api/v1/restaurants/{id}/menu-items/{iid}/addons/{aid}    [RestaurantOwner] Update addon
  DELETE /api/v1/restaurants/{id}/menu-items/{iid}/addons/{aid}    [RestaurantOwner] Delete addon
  GET    /api/v1/restaurants/{id}/operating-hours    [RestaurantOwner] Get hours
  PUT    /api/v1/restaurants/{id}/operating-hours    [RestaurantOwner] Upsert hours
  GET    /api/v1/cuisines                            [AllowAnonymous] List cuisine types
  ```
- **Flutter Routes Added (9)**:
  ```
  /my-restaurants                                          → RestaurantListScreen
  /my-restaurants/register                                 → RestaurantRegisterScreen
  /my-restaurants/:restaurantId/dashboard                  → RestaurantDashboardScreen
  /my-restaurants/:restaurantId/edit                       → RestaurantEditScreen
  /my-restaurants/:restaurantId/menu-categories            → MenuCategoriesScreen
  /my-restaurants/:restaurantId/menu-categories/:cid/items → MenuItemsScreen
  /my-restaurants/:restaurantId/menu-item-form             → MenuItemFormScreen
  /my-restaurants/:restaurantId/operating-hours            → OperatingHoursScreen
  /my-restaurants/:restaurantId/documents                  → DocumentUploadScreen
  ```
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation for restaurant_management feature
  - `image_picker` package may need to be added to pubspec.yaml if not already present
  - LocalFileStorageService saves to wwwroot/uploads — switch to S3 for production
- **Next Module Prerequisites**:
  - Run Flutter code gen: `cd frontend/swiggy_clone_app && dart run build_runner build --delete-conflicting-outputs`
  - Verify `image_picker` in pubspec.yaml

---
### Module: Module 4 — Customer Discovery
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created/Modified**:
  - **Backend — Application Layer — IAppDbContext Update (1 file)**:
    - `IAppDbContext.cs` — Added `DbSet<UserFavorite> UserFavorites` and `DbSet<Review> Reviews`
  - **Backend — Application Layer — Discovery DTOs (4 files)** in `src/SwiggyClone.Application/Features/Discovery/DTOs/`:
    - `CustomerRestaurantDto.cs` — Restaurant summary for customer listings (Id, Name, Slug, LogoUrl, BannerUrl, City, AverageRating, TotalRatings, AvgDeliveryTimeMin, AvgCostForTwo, IsVegOnly, IsAcceptingOrders, IsDineInEnabled, Cuisines)
    - `PublicRestaurantDetailDto.cs` — Full detail with menu tree, includes `MenuSectionDto` (CategoryId, CategoryName, SortOrder, Items)
    - `HomeFeedDto.cs` — Aggregated home feed with `BannerDto`, `CuisineChipDto`, `RestaurantSectionDto`
    - `FavouriteDto.cs` — Favourite record DTO
  - **Backend — Application Layer — Discovery Queries (8 files)** in `src/SwiggyClone.Application/Features/Discovery/Queries/`:
    - `BrowseRestaurantsQuery.cs` + Handler + Validator — Cursor-paginated, filterable by City/Cuisine/Veg/Rating/Cost, 4 sort modes
    - `SearchRestaurantsQuery.cs` + Handler + Validator — LIKE search on Name, Description, CuisineType.Name
    - `GetPublicRestaurantDetailQuery.cs` + Handler — Full restaurant with operating hours + menu tree (categories → items → variants → addons)
    - `GetHomeFeedQuery.cs` + Handler — Top rated (10), popular (10), quick delivery (10) sections + banners + cuisine chips; uses `Expression<Func<>>` projection for EF Core compatibility
  - **Backend — Application Layer — Favourites (8 files)** in `src/SwiggyClone.Application/Features/Favourites/`:
    - `AddFavouriteCommand.cs` + Handler — Idempotent, verifies restaurant is Approved
    - `RemoveFavouriteCommand.cs` + Handler — Idempotent
    - `GetFavouritesQuery.cs` + Handler — Returns favourites as `List<CustomerRestaurantDto>`
    - `CheckFavouriteQuery.cs` + Handler — Returns `Result<bool>`
  - **Backend — API Layer — Controllers (2 files)** in `src/SwiggyClone.Api/Controllers/`:
    - `DiscoveryController.cs` — Route `api/v1/discovery`, AllowAnonymous, 4 endpoints
    - `FavouritesController.cs` — Route `api/v1/favourites`, Authorize, 4 endpoints
  - **Flutter — Data Layer (6 files)** in `features/customer_discovery/data/`:
    - `models/customer_restaurant_model.dart` — Freezed model matching CustomerRestaurantDto
    - `models/public_restaurant_detail_model.dart` — Includes `MenuSectionModel`, reuses MenuItemModel from restaurant_management
    - `models/home_feed_model.dart` — `HomeFeedModel` with `BannerModel`, `CuisineChipModel`, `RestaurantSectionModel`
    - `models/browse_result_model.dart` — Cursor pagination result wrapper
    - `datasources/discovery_remote_data_source.dart` — All API calls for discovery + favourites
    - `repositories/discovery_repository.dart` — DioException → Failure mapping
  - **Flutter — Providers (10 files)** in `features/customer_discovery/presentation/providers/`:
    - `home_feed_notifier.dart` / `home_feed_state.dart` — 4 states: initial, loading, loaded(feed), error
    - `restaurant_browse_notifier.dart` / `restaurant_browse_state.dart` — Filters + `loadMore()` cursor pagination
    - `restaurant_search_notifier.dart` / `restaurant_search_state.dart` — 5 states including empty, with `clear()` method
    - `public_restaurant_detail_notifier.dart` / `public_restaurant_detail_state.dart` — Loads detail + favourite check/toggle
    - `favourites_notifier.dart` / `favourites_state.dart` — Load and remove favourites
  - **Flutter — Screens (5 files)** in `features/customer_discovery/presentation/screens/`:
    - `home_screen.dart` — PageView banners, horizontal cuisine chips, vertical restaurant sections
    - `search_screen.dart` — Debounced text input (400ms), live results, clear button
    - `restaurant_detail_screen.dart` — CustomScrollView with SliverAppBar (banner + favourite), info chips, menu sections with ADD button
    - `menu_item_detail_sheet.dart` — DraggableScrollableSheet with variant RadioListTile, addon CheckboxListTile, quantity selector, "Add ₹X" button
    - `favourites_screen.dart` — List with swipe-to-remove via Dismissible
  - **Flutter — Widgets (1 file)** in `features/customer_discovery/presentation/widgets/`:
    - `restaurant_card.dart` — `CustomerRestaurantCard` with banner, name, veg badge, cuisines, rating, delivery time, cost for two
  - **Flutter — Routing (1 file modified)**:
    - `routing/app_router.dart` — Replaced 4 placeholders with real screens (HomeScreen, SearchScreen, RestaurantDetailScreen, FavouritesScreen)
- **API Endpoints (Backend — 8)**:
  ```
  GET    /api/v1/discovery/home                        [AllowAnonymous] Home feed (banners, cuisines, sections)
  GET    /api/v1/discovery/restaurants                  [AllowAnonymous] Browse restaurants (paginated, filterable)
  GET    /api/v1/discovery/restaurants/search           [AllowAnonymous] Search restaurants by name/cuisine
  GET    /api/v1/discovery/restaurants/{restaurantId}   [AllowAnonymous] Restaurant detail with full menu
  GET    /api/v1/favourites                             [Authorize] List user's favourites
  GET    /api/v1/favourites/{restaurantId}              [Authorize] Check if restaurant is favourited
  POST   /api/v1/favourites/{restaurantId}              [Authorize] Add favourite
  DELETE /api/v1/favourites/{restaurantId}              [Authorize] Remove favourite
  ```
- **Flutter Routes Wired (4 placeholders replaced)**:
  ```
  /home                              → HomeScreen
  /search                            → SearchScreen
  /restaurant/:restaurantId          → RestaurantDetailScreen
  /favourites                        → FavouritesScreen
  ```
- **Key Design Decisions**:
  - Customer queries completely separate from owner queries (no ownership checks, Status==Approved filter, no sensitive fields)
  - HomeFeed uses 3 separate DB queries (top rated, popular, quick delivery) — optimize later with caching
  - `Expression<Func<>>` projections used for EF Core LINQ translation compatibility in GetHomeFeedQueryHandler
  - MenuItemDetailSheet has TODO: wire to cart in Module 5
  - BrowseRestaurants uses cursor pagination via Guid comparison for consistent ordering
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation for customer_discovery feature
  - MenuItemDetailSheet "Add to cart" button shows snackbar only — needs wiring to cart provider in Module 5
  - HomeFeed banners/cuisine chips are hardcoded in handler — will be DB-driven or CMS-driven later
- **Next Module Prerequisites**:
  - Run Flutter code gen: `cd frontend/swiggy_clone_app && dart run build_runner build --delete-conflicting-outputs`

---
### Module: Module 5 — Cart & Ordering (Delivery)
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created/Modified**:
  - **Backend — IAppDbContext Update (1 file)**:
    - `IAppDbContext.cs` — Added 7 missing DbSets (OrderItems, OrderItemAddons, OrderStatusHistory, Payments, UserAddresses, Coupons, CouponUsages)
  - **Backend — Cart Service (3 files)**:
    - `Application/Features/Cart/DTOs/CartDto.cs` — CartDto, CartItemDto, CartItemAddonDto
    - `Application/Common/Interfaces/ICartService.cs` — Cart service interface + AddToCartItem, CartAddonInput records
    - `Infrastructure/Services/RedisCartService.cs` — Redis JSON storage, 7-day TTL, deterministic CartItemId, different-restaurant guard
  - **Backend — Cart CQRS (12 files)** in `Application/Features/Cart/`:
    - AddToCartCommand + Handler + Validator
    - UpdateCartItemQuantityCommand + Handler + Validator
    - RemoveCartItemCommand + Handler
    - ClearCartCommand + Handler
    - GetCartQuery + Handler
  - **Backend — Cart API (2 files)**:
    - `Api/Contracts/Cart/CartRequests.cs` — AddToCartRequest, CartAddonRequest, UpdateCartItemQuantityRequest
    - `Api/Controllers/CartController.cs` — 5 endpoints (GET /, POST /items, PUT /items/{id}, DELETE /items/{id}, DELETE /)
  - **Backend — Order DTOs (4 files)** in `Application/Features/Orders/DTOs/`:
    - OrderDto, OrderItemDto, OrderItemAddonDto, OrderSummaryDto
  - **Backend — Order Commands (9 files)** in `Application/Features/Orders/Commands/`:
    - PlaceOrderCommand + Handler + Validator — Cart→Order conversion, price revalidation, order number generation
    - CancelOrderCommand + Handler + Validator — Placed/Confirmed only
    - UpdateOrderStatusCommand + Handler + Validator — Valid transitions, restaurant owner auth
  - **Backend — Order Queries (4 files)** in `Application/Features/Orders/Queries/`:
    - GetMyOrdersQuery + Handler — Cursor-paginated, OrderSummaryDto projection
    - GetOrderDetailQuery + Handler — Full order with items, addons, status history
  - **Backend — Order API (2 files)**:
    - `Api/Contracts/Orders/OrderRequests.cs` — PlaceOrderRequest, CancelOrderRequest, UpdateOrderStatusRequest
    - `Api/Controllers/OrdersController.cs` — 5 endpoints
  - **Backend — DI Update (1 file)**:
    - `Infrastructure/DependencyInjection.cs` — Registered ICartService → RedisCartService
  - **Flutter — Cart Data Layer (3 files)** in `features/cart/data/`:
    - `models/cart_model.dart` — Freezed CartModel, CartItemModel, CartItemAddonModel
    - `datasources/cart_remote_data_source.dart` — 5 Dio methods
    - `repositories/cart_repository.dart` — DioException → Failure mapping, DIFFERENT_RESTAURANT errorCode
  - **Flutter — Cart Providers (2 files)** in `features/cart/presentation/providers/`:
    - `cart_state.dart` — 5 sealed states (initial, loading, loaded, empty, error)
    - `cart_notifier.dart` — keepAlive, addToCart returns (success, errorCode) record
  - **Flutter — Orders Data Layer (4 files)** in `features/orders/data/`:
    - `models/order_model.dart`, `order_summary_model.dart`, `order_item_model.dart`
    - `datasources/order_remote_data_source.dart`, `repositories/order_repository.dart`
  - **Flutter — Orders Providers (6 files)** in `features/orders/presentation/providers/`:
    - `my_orders_notifier.dart` / `my_orders_state.dart` — Paginated with loadMore
    - `order_detail_notifier.dart` / `order_detail_state.dart` — Family provider by orderId, cancel support
    - `place_order_notifier.dart` / `place_order_state.dart` — placing → placed / error
  - **Flutter — Screens (6 files)**:
    - `cart/presentation/screens/cart_screen.dart` — Item list, quantity +/-, clear, proceed to checkout
    - `cart/presentation/widgets/cart_item_card.dart` — Reusable cart item card
    - `orders/presentation/screens/checkout_screen.dart` — Price breakdown, place order button
    - `orders/presentation/screens/order_success_screen.dart` — Success animation, view order / home buttons
    - `orders/presentation/screens/order_history_screen.dart` — Paginated order list with status badges
    - `orders/presentation/screens/order_detail_screen.dart` — Full detail with cancel button
  - **Flutter — Route Wiring (1 file modified)**:
    - `routing/app_router.dart` — Replaced 5 placeholders (Cart, Orders, OrderDetail, Checkout, OrderSuccess)
  - **Flutter — MenuItemDetailSheet Wiring (2 files modified)**:
    - `menu_item_detail_sheet.dart` — Converted to ConsumerStatefulWidget, added restaurantId param, wired addToCart with DIFFERENT_RESTAURANT dialog
    - `restaurant_detail_screen.dart` — Passes restaurant.id to MenuItemDetailSheet
- **API Endpoints (Backend — 10)**:
  ```
  GET    /api/v1/cart                    [Authorize] Get cart
  POST   /api/v1/cart/items              [Authorize] Add item to cart
  PUT    /api/v1/cart/items/{cartItemId} [Authorize] Update cart item quantity
  DELETE /api/v1/cart/items/{cartItemId} [Authorize] Remove cart item
  DELETE /api/v1/cart                    [Authorize] Clear cart
  POST   /api/v1/orders                 [Authorize] Place order
  GET    /api/v1/orders                 [Authorize] Get my orders (paginated)
  GET    /api/v1/orders/{orderId}       [Authorize] Get order detail
  PUT    /api/v1/orders/{orderId}/cancel [Authorize] Cancel order
  PUT    /api/v1/orders/{orderId}/status [Authorize] Update order status (restaurant owner)
  ```
- **Flutter Routes Wired (5 placeholders replaced)**:
  ```
  /cart            → CartScreen
  /orders          → OrderHistoryScreen
  /orders/:orderId → OrderDetailScreen
  /checkout        → CheckoutScreen
  /order-success   → OrderSuccessScreen
  ```
- **Key Design Decisions**:
  - Cart stored as JSON in Redis with `cart:{userId}` key, 7-day TTL
  - Deterministic CartItemId = `{menuItemId}_{variantId}_{sortedAddonIds}` enables item merging
  - DIFFERENT_RESTAURANT error triggers clear-and-retry dialog in Flutter
  - Order number format: `SWG-YYYYMMDD-XXXX`
  - Pricing: Subtotal + 5% tax + ₹49 delivery + ₹15 packaging
  - Payment: CashOnDelivery by default (gateway integration in Module 8)
  - Status transitions validated: Placed→Confirmed→Preparing→ReadyForPickup→OutForDelivery→Delivered
  - Cancel only from Placed/Confirmed states
  - Order tracking placeholder remains for Module 10
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation for cart and orders features
  - Checkout uses placeholder address ID — real address selection pending
  - Payment gateway integration deferred to Module 8
  - Delivery partner assignment deferred to Module 10
  - Real-time order tracking deferred to Module 10
  - Coupon application deferred to Module 14

---
### Module: Module 6 — Dine-In (Customer)
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created/Modified**:
  - **Backend — Domain Update (1 file)**:
    - `Order.cs` — Added `DineInSessionId` FK (Guid?) and `DineInSession` navigation property
  - **Backend — Infrastructure Update (1 file)**:
    - `OrderConfiguration.cs` — Added DineInSession relationship + partial index on DineInSessionId
  - **Backend — IAppDbContext Update (1 file)**:
    - `IAppDbContext.cs` — Added `DbSet<DineInSession>` and `DbSet<DineInSessionMember>`
  - **Backend — DTOs (5 files)** in `src/SwiggyClone.Application/Features/DineIn/DTOs/`:
    - `DineInTableDto.cs`, `DineInSessionMemberDto.cs`, `DineInOrderSummaryDto.cs`, `DineInSessionSummaryDto.cs`, `DineInSessionDto.cs`
  - **Backend — Commands (18 files)** in `src/SwiggyClone.Application/Features/DineIn/Commands/`:
    - StartSession (Command + Handler + Validator) — QR scan → find table → create session + host member → set table Occupied
    - JoinSession (Command + Handler + Validator) — Session code → add guest member
    - PlaceDineInOrder (Command + Handler + Validator) — OrderType=DineIn, no delivery/packaging, PayAtRestaurant
    - RequestBill (Command + Handler + Validator) — Host only → BillRequested status
    - EndSession (Command + Handler + Validator) — Host → Completed + free table
    - LeaveSession (Command + Handler + Validator) — Guest only → remove member
  - **Backend — Queries (8 files)** in `src/SwiggyClone.Application/Features/DineIn/Queries/`:
    - GetMyActiveSession (Query + Handler) — Find active session membership
    - GetSessionDetail (Query + Handler) — Full session with members, table, orders
    - GetDineInMenu (Query + Handler) — Reuses PublicRestaurantDetailDto projection
    - GetSessionOrders (Query + Handler) — Orders by DineInSessionId → List<OrderDto>
  - **Backend — API Layer (2 files)**:
    - `Controllers/DineInController.cs` — 10 endpoints, all [Authorize], route prefix `api/v1/dine-in`
    - `Contracts/DineIn/DineInRequests.cs` — StartSessionRequest, JoinSessionRequest, PlaceDineInOrderRequest + sub-records
  - **Flutter — Models (4 files)** in `features/dine_in/data/models/`:
    - `dine_in_table_model.dart`, `dine_in_member_model.dart`, `dine_in_order_summary_model.dart`, `dine_in_session_model.dart`
  - **Flutter — Data Sources (1 file)**:
    - `dine_in_remote_data_source.dart` — 9 Dio methods matching all backend endpoints
  - **Flutter — Repository (1 file)**:
    - `dine_in_repository.dart` — DioException → Failure mapping, record-tuple returns
  - **Flutter — Providers (9 files)** in `features/dine_in/presentation/providers/`:
    - `active_session_state.dart` + `active_session_notifier.dart` (keepAlive) — checkActiveSession, startSession, joinSession
    - `dine_in_session_state.dart` + `dine_in_session_notifier.dart` — loadSession, requestBill, leaveSession, endSession
    - `session_orders_state.dart` + `session_orders_notifier.dart` — loadOrders, placeOrder
    - `dine_in_menu_state.dart` + `dine_in_menu_notifier.dart` — loadMenu via session endpoint
    - `dine_in_websocket_notifier.dart` — WebSocket coordinator for real-time events
  - **Flutter — Screens (6 files)** in `features/dine_in/presentation/screens/`:
    - `dine_in_tab_screen.dart` — Bottom nav tab: active session or scan/join prompt
    - `qr_scanner_screen.dart` — Camera scanner using `mobile_scanner`
    - `dine_in_session_screen.dart` — Session hub: info card, session code, members, menu, orders, bill actions
    - `dine_in_menu_screen.dart` — Menu browsing with ADD button → DineInMenuItemDetailSheet
    - `session_orders_screen.dart` — All session orders with status badges
    - `bill_summary_screen.dart` — Aggregated bill with order breakdown, subtotal/tax/total
  - **Flutter — Widgets (5 files)** in `features/dine_in/presentation/widgets/`:
    - `session_info_card.dart`, `session_code_share_widget.dart`, `member_list_widget.dart`, `dine_in_order_card.dart`, `dine_in_menu_item_detail_sheet.dart`
  - **Flutter — Routing (3 files modified)**:
    - `routing/route_names.dart` — Added dineInMenu, dineInSessionOrders, dineInBill constants + helper methods
    - `routing/app_router.dart` — Replaced 3 placeholders (DineInTabScreen, QrScannerScreen, DineInSessionScreen + sub-routes)
    - `core/constants/api_constants.dart` — Added 8 dine-in session helper methods/constants
- **API Endpoints (Backend — 10)**:
  ```
  POST   /api/v1/dine-in/sessions              [Authorize] Start session (scan QR)
  POST   /api/v1/dine-in/sessions/join          [Authorize] Join via session code
  GET    /api/v1/dine-in/sessions/active        [Authorize] My active session
  GET    /api/v1/dine-in/sessions/{id}          [Authorize] Session detail
  GET    /api/v1/dine-in/sessions/{id}/menu     [Authorize] Browse dine-in menu
  POST   /api/v1/dine-in/sessions/{id}/orders   [Authorize] Place dine-in order
  GET    /api/v1/dine-in/sessions/{id}/orders   [Authorize] List session orders
  PUT    /api/v1/dine-in/sessions/{id}/request-bill  [Authorize] Request bill (host only)
  PUT    /api/v1/dine-in/sessions/{id}/end      [Authorize] End session (host only)
  PUT    /api/v1/dine-in/sessions/{id}/leave    [Authorize] Leave session (guest only)
  ```
- **Flutter Routes Wired (3 placeholders replaced + 3 sub-routes)**:
  ```
  /dine-in                                → DineInTabScreen
  /dine-in/scan                           → QrScannerScreen
  /dine-in/session/:sessionId             → DineInSessionScreen
  /dine-in/session/:sessionId/menu        → DineInMenuScreen
  /dine-in/session/:sessionId/orders      → SessionOrdersScreen
  /dine-in/session/:sessionId/bill        → BillSummaryScreen
  ```
- **Key Design Decisions**:
  - Session code: 6-char alphanumeric (excludes ambiguous I,O,0,1), unique per session
  - Host/Guest roles: host can request bill/end session, guests can only leave
  - Dine-in orders: OrderType=DineIn (2), no delivery fee, no packaging charge, PaymentMethod=PayAtRestaurant
  - DineInSessionId FK on Order entity links orders to sessions for aggregated billing
  - Menu browsing reuses PublicRestaurantDetailDto projection (no duplication)
  - WebSocket notifier coordinates real-time updates across session/orders notifiers
  - DineInMenuItemDetailSheet forked from MenuItemDetailSheet (decoupled from CartNotifier)
  - Parameterized Riverpod notifiers via `build(String sessionId)` pattern
  - ActiveSessionNotifier is keepAlive (global singleton for bottom nav tab state)
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation for dine_in feature
  - `mobile_scanner` package may need to be added to pubspec.yaml
  - `web_socket_channel` package may need to be added to pubspec.yaml
  - WebSocket backend hub not yet implemented (needs SignalR or raw WebSocket endpoint in Module 7+)
  - Bill payment flow deferred to Module 8
  - EF Core migration needs regeneration to include DineInSessionId FK on orders table

---
### Module: Module 7 — Dine-In (Restaurant Owner)
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created**:
  - **Backend DTOs (3 new)** in `src/SwiggyClone.Application/Features/DineIn/DTOs/`:
    - RestaurantTableDetailDto.cs — owner-facing table DTO with ActiveSessionCount
    - OwnerSessionDto.cs — active session DTO with aggregated member/order counts
    - OwnerDineInOrderDto.cs + OwnerDineInOrderItemDto — dine-in order DTO with item details
  - **Backend Table CRUD Commands (9 files)** in `src/SwiggyClone.Application/Features/DineIn/Commands/`:
    - CreateTable{Command,CommandHandler,CommandValidator}.cs — create table with auto QR code generation
    - UpdateTable{Command,CommandHandler,CommandValidator}.cs — update table with active session guards
    - DeleteTable{Command,CommandHandler,CommandValidator}.cs — soft-delete (IsActive=false, Status=Maintenance)
  - **Backend Queries (6 files)** in `src/SwiggyClone.Application/Features/DineIn/Queries/`:
    - GetRestaurantTables{Query,QueryHandler}.cs — list all tables with active session count
    - GetRestaurantSessions{Query,QueryHandler}.cs — list active sessions with aggregates
    - GetRestaurantDineInOrders{Query,QueryHandler}.cs — list dine-in orders with optional status filter
  - **Backend Order Status Command (3 files)** in `src/SwiggyClone.Application/Features/DineIn/Commands/`:
    - UpdateDineInOrderStatus{Command,CommandHandler,CommandValidator}.cs — advance order status with valid transition checks
  - **Backend Controller + Contracts (2 files)**:
    - src/SwiggyClone.Api/Contracts/DineIn/TableRequests.cs (CreateTableRequest, UpdateTableRequest, UpdateDineInOrderStatusRequest)
    - src/SwiggyClone.Api/Controllers/RestaurantDineInController.cs (7 endpoints)
  - **Flutter Models (3 new)** in `features/restaurant_management/data/models/`:
    - restaurant_table_model.dart — Freezed: id, tableNumber, capacity, floorSection, qrCodeData, status, isActive, activeSessionCount
    - owner_session_model.dart — Freezed: id, tableNumber, floorSection, sessionCode, status, memberCount, orderCount, totalAmount
    - owner_dine_in_order_model.dart — Freezed: OwnerDineInOrderModel + OwnerDineInOrderItemModel
  - **Flutter Providers (6 files)** in `features/restaurant_management/presentation/providers/`:
    - table_management_state.dart + table_management_notifier.dart (loadTables, createTable, updateTable, deleteTable)
    - active_sessions_state.dart + active_sessions_notifier.dart (loadSessions)
    - dine_in_orders_state.dart + dine_in_orders_notifier.dart (loadOrders, updateOrderStatus)
  - **Flutter Screens (3 new)** in `features/restaurant_management/presentation/screens/`:
    - table_management_screen.dart — ListView of tables with FAB to add, edit/delete, IsActive toggle
    - active_sessions_screen.dart — list active sessions with table, session code, member/order counts
    - dine_in_orders_screen.dart — tab-filtered (All/New/In Progress/Ready/Served) with action buttons
  - **Flutter Widgets (2 new)** in `features/restaurant_management/presentation/widgets/`:
    - table_form_dialog.dart — bottom sheet for create/edit table
    - owner_dine_in_order_card.dart — order card with items, status badge, action button
- **Modified Files**:
  - RestaurantDashboardDto.cs — added TotalTables, ActiveTables, ActiveSessions, PendingDineInOrders
  - GetRestaurantDashboardQueryHandler.cs — added 4 dine-in stat queries
  - restaurant_dashboard_model.dart — added 4 dine-in fields with @Default(0)
  - restaurant_dashboard_screen.dart — added 3 dine-in stat cards + 2 navigation tiles (Tables, Dine-In Orders)
  - api_constants.dart — added 5 helpers: restaurantTables, restaurantTable, restaurantDineInSessions, restaurantDineInOrders, restaurantDineInOrderStatus
  - restaurant_remote_data_source.dart — added 7 methods for table/session/order APIs
  - restaurant_repository.dart — added 7 corresponding methods with record-tuple returns
  - route_names.dart — added tableManagement, activeSessions, dineInOrders constants + path helpers
  - app_router.dart — added 3 GoRoute entries + 3 screen imports
- **API Endpoints Added** (RestaurantDineInController, prefix: `api/v1/restaurants/{restaurantId}`):
  | Method | Path | Description |
  |--------|------|-------------|
  | GET | `/{restaurantId}/tables` | List all tables |
  | POST | `/{restaurantId}/tables` | Create table |
  | PUT | `/{restaurantId}/tables/{tableId}` | Update table |
  | DELETE | `/{restaurantId}/tables/{tableId}` | Soft-delete table |
  | GET | `/{restaurantId}/dine-in-sessions` | Active sessions |
  | GET | `/{restaurantId}/dine-in-orders` | Dine-in orders (optional ?status= filter) |
  | PUT | `/{restaurantId}/dine-in-orders/{orderId}/status` | Update order status |
- **Flutter Routes Added**:
  | Route | Screen |
  |-------|--------|
  | `/my-restaurants/:restaurantId/tables` | TableManagementScreen |
  | `/my-restaurants/:restaurantId/active-sessions` | ActiveSessionsScreen |
  | `/my-restaurants/:restaurantId/dine-in-orders` | DineInOrdersScreen |
- **Key Design Decisions**:
  - DineInOrderStatus values (0-6) map 1:1 to OrderStatus numerically — cast via `(OrderStatus)(short)newStatus`, no schema change
  - Table soft-delete: IsActive=false + Status=Maintenance preserves FK integrity with existing sessions/orders
  - QrCodeData auto-generated as `"DINE-{restaurantId:N}-T{tableNumber}"`
  - Separate RestaurantDineInController to avoid bloating RestaurantsController
  - All owner commands use RestaurantOwnershipHelper.VerifyOwnership() guard
  - Valid order transitions: Placed→Confirmed→Preparing→Ready→Served→Completed
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation for restaurant_management dine-in providers/models
  - EF Core migration needs regeneration to reflect any new queries (no schema changes in Module 7)

---
### Module: Module 8 — Payments
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created**:
  - **Backend Gateway Abstraction (2 new):**
    - Application/Common/Interfaces/IPaymentGatewayService.cs — interface with CreateOrderAsync, VerifyPaymentAsync, RefundAsync + 3 result records
    - Infrastructure/Services/DevPaymentGatewayService.cs — dev implementation, all operations succeed with simulated delays
  - **Backend DTOs (2 new)** in `Features/Payments/DTOs/`:
    - PaymentDto.cs — full payment details
    - CreatePaymentOrderResponseDto.cs — gateway order creation response
  - **Backend Payment Commands (12 new)** in `Features/Payments/Commands/`:
    - CreatePaymentOrder{Command,CommandHandler,CommandValidator}.cs — create gateway order for an order
    - VerifyPayment{Command,CommandHandler,CommandValidator}.cs — verify payment, bulk-update dine-in session orders
    - InitiateRefund{Command,CommandHandler,CommandValidator}.cs — full refund via gateway
    - PayDineInSession{Command,CommandHandler,CommandValidator}.cs — create gateway order for session total
  - **Backend Query (2 new)** in `Features/Payments/Queries/`:
    - GetPaymentByOrderId{Query,QueryHandler}.cs — get payment details by order
  - **Backend Controller + Contracts (2 new):**
    - Api/Contracts/Payments/PaymentRequests.cs (CreatePaymentOrderRequest, VerifyPaymentRequest, InitiateRefundRequest, PayDineInSessionRequest)
    - Api/Controllers/PaymentsController.cs (5 endpoints)
  - **Flutter Models (2 new)** in `features/payments/data/models/`:
    - payment_model.dart — Freezed: id, orderId, paymentGateway, amount, currency, status, method, refund fields
    - payment_order_model.dart — Freezed: paymentId, gatewayOrderId, amount, currency, gateway
  - **Flutter Data Layer (2 new):**
    - features/payments/data/datasources/payment_remote_data_source.dart (4 methods)
    - features/payments/data/repositories/payment_repository.dart (4 methods with record-tuple returns)
  - **Flutter Providers (2 new):**
    - features/payments/presentation/providers/payment_state.dart (7 sealed states)
    - features/payments/presentation/providers/payment_notifier.dart (initiatePayment, initiateDineInPayment, _simulateDevPayment)
  - **Flutter Screen (1 new):**
    - features/payments/presentation/screens/payment_processing_screen.dart — progress states, auto-verify, success/failure handling
- **Modified Files**:
  - DependencyInjection.cs — registered DevPaymentGatewayService
  - PlaceOrderCommand.cs — added PaymentMethod parameter
  - PlaceOrderCommandHandler.cs — dynamic payment method + gateway selection
  - PlaceOrderCommandValidator.cs — PaymentMethod validation (1-6)
  - OrderRequests.cs — added PaymentMethod to PlaceOrderRequest
  - OrdersController.cs — pass PaymentMethod to command
  - CancelOrderCommandHandler.cs — added IPaymentGatewayService injection + auto-refund for prepaid orders
  - order_remote_data_source.dart — added paymentMethod to placeOrder
  - order_repository.dart — pass paymentMethod through
  - place_order_notifier.dart — added paymentMethod parameter
  - api_constants.dart — added paymentDineInSession, paymentByOrder, paymentRefund helpers
  - checkout_screen.dart — replaced hardcoded COD with selectable payment method radio list
  - bill_summary_screen.dart — added "Pay Now" button + payment method selection bottom sheet
  - order_detail_screen.dart — added payment status badge (Pending/Paid/Failed/Refunded)
  - app_router.dart — replaced placeholder /payment route with PaymentProcessingScreen
- **API Endpoints Added** (PaymentsController, prefix: `api/v1/payments`):
  | Method | Path | Description |
  |--------|------|-------------|
  | POST | `/payments` | Create payment order on gateway |
  | POST | `/payments/verify` | Verify payment after processing |
  | POST | `/payments/{orderId}/refund` | Initiate full refund |
  | GET | `/payments/{orderId}` | Get payment details by order |
  | POST | `/payments/dine-in-session` | Pay for entire dine-in session |
- **Key Design Decisions**:
  - Dev payment gateway: all operations succeed with 100ms simulated delay, generates `dev_order_*` / `dev_refund_*` IDs
  - Payment record created at order placement (Pending), updated with gateway details when online payment initiated
  - PlaceOrderCommand gateway set to "CashOnDelivery" for COD (method=5) or "Pending" for online (methods 1-4)
  - VerifyPayment bulk-updates all session orders on dine-in payment: marks all Paid, session→Completed, table→Available
  - CancelOrderCommand auto-refunds prepaid orders via gateway
  - Flutter PaymentNotifier simulates 2-second processing delay client-side before calling verify
  - IPaymentGatewayService interface allows swapping to Razorpay/Stripe in production
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation for payments feature
  - No partial refund support (full refund only)
  - Dev gateway always succeeds — production needs real Razorpay/Stripe integration
  - Wallet balance tracking not implemented (just a payment method option)

---
### Module: Module 9 — Notifications
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created/Modified**:
  - **Backend — Interfaces (2 files)**:
    - `IAppDbContext.cs` (MODIFIED) — Added `DbSet<Notification>` and `DbSet<UserDevice>`
    - `INotificationService.cs` (NEW) — Push notification abstraction with `PushResult` record
  - **Backend — Infrastructure (2 files)**:
    - `DevNotificationService.cs` (NEW) — Dev push service, logs to console with 50ms delay
    - `DependencyInjection.cs` (MODIFIED) — Registered `INotificationService → DevNotificationService`
  - **Backend — DTOs (2 files)** in `Features/Notifications/DTOs/`:
    - `NotificationDto.cs` — Id, Title, Body, Type, Data?, IsRead, ReadAt?, CreatedAt
    - `UnreadCountDto.cs` — Count
  - **Backend — Commands (15 files)** in `Features/Notifications/Commands/`:
    - SendNotification (Command + Handler + Validator) — Creates notification + fires push
    - MarkNotificationAsRead (Command + Handler + Validator) — Marks single as read
    - MarkAllNotificationsAsRead (Command + Handler + Validator) — Bulk update via ExecuteUpdateAsync
    - RegisterDevice (Command + Handler + Validator) — Upserts UserDevice
    - UnregisterDevice (Command + Handler + Validator) — Sets IsActive=false
  - **Backend — Queries (4 files)** in `Features/Notifications/Queries/`:
    - GetMyNotificationsQuery + Handler — Cursor-based pagination
    - GetUnreadCountQuery + Handler — Count of unread
  - **Backend — Controller (1 file)**:
    - `NotificationsController.cs` — 6 endpoints, route prefix `api/v1/notifications`
  - **Backend — Contracts (1 file)**:
    - `NotificationRequests.cs` — RegisterDeviceRequest, UnregisterDeviceRequest
  - **Flutter — Data Layer (4 files)**:
    - `notification_model.dart` (Freezed) — id, title, body, type, data?, isRead, readAt?, createdAt
    - `notification_remote_data_source.dart` — 6 methods (getMyNotifications, getUnreadCount, markAsRead, markAllAsRead, registerDevice, unregisterDevice)
    - `notification_repository.dart` — 6 methods with `({T? data, Failure? failure})` returns
    - `api_constants.dart` (MODIFIED) — Added notificationUnreadCount, notificationMarkRead, notificationReadAll, notificationDevices
  - **Flutter — Providers (4 files)**:
    - `notifications_state.dart` — Sealed: initial, loading, loaded(notifications, hasMore, nextCursor, isLoadingMore), error
    - `notifications_notifier.dart` — loadNotifications, loadMore, markAsRead, markAllAsRead
    - `unread_count_notifier.dart` — fetchCount, decrement, reset
    - `device_registration_notifier.dart` — registerDevice, unregisterDevice
  - **Flutter — Screens (2 files)**:
    - `notifications_screen.dart` (NEW) — Pull-to-refresh, infinite scroll, type-based icons, relative timestamps, unread dot indicator, empty state, tap-to-read + navigate
    - `home_screen.dart` (MODIFIED) — Added notification bell icon with Badge widget showing unread count
  - **Flutter — Routing (2 files)**:
    - `route_names.dart` (MODIFIED) — Added `notifications = '/notifications'`
    - `app_router.dart` (MODIFIED) — Added NotificationsScreen import + GoRoute entry
- **API Endpoints**:
  | Method | Path | Action |
  |--------|------|--------|
  | GET | `/notifications` | Get paginated notifications (cursor-based) |
  | GET | `/notifications/unread-count` | Get unread count |
  | PUT | `/notifications/{id}/read` | Mark single notification as read |
  | PUT | `/notifications/read-all` | Mark all notifications as read |
  | POST | `/notifications/devices` | Register device for push notifications |
  | DELETE | `/notifications/devices` | Unregister device |
- **Key Design Decisions**:
  - SendNotificationCommand is internal (used by other command handlers, not exposed via API)
  - MarkAllAsRead uses EF Core `ExecuteUpdateAsync` for efficient bulk update
  - RegisterDevice upserts: if (UserId, DeviceToken) exists, updates IsActive+Platform; else creates new
  - DevNotificationService logs push to console with 50ms delay — production will use FCM/APNs
  - Unread count badge on home screen AppBar with optimistic decrement on mark-as-read
  - Notification tiles show type-specific icons and colors (OrderUpdate=orange, Promotion=purple, DineIn=teal, System=blue)
  - Tap notification → mark as read + navigate to order detail if Data contains orderId
  - Firebase messaging configured in pubspec but not initialized (needs google-services.json)
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation for notifications feature
  - Firebase not initialized — needs google-services.json and FlutterFire CLI setup for real push
  - No WebSocket/SignalR for real-time notification delivery (uses polling/manual refresh)
  - SendNotificationCommand not yet called from other handlers (order status changes, etc.) — will wire in future modules
  - No notification preferences/settings (mute, filter by type) yet

---
### Module: Module 10 — Delivery Partner
- **Status**: COMPLETED
- **Completed At**: 2026-02-25
- **Files Created**:
  - **Backend — IAppDbContext Fix (1 modified)**:
    - `IAppDbContext.cs` (MODIFIED) — Added `DbSet<DeliveryAssignment>` and `DbSet<DeliveryPartnerLocation>`
  - **Backend — DTOs (4 new)** in `Features/Deliveries/DTOs/`:
    - `DeliveryAssignmentDto.cs` — Id, OrderId, OrderNumber, RestaurantName, RestaurantAddress?, CustomerAddress?, Status, AssignedAt, AcceptedAt?, PickedUpAt?, DeliveredAt?, DistanceKm?, Earnings
    - `DeliveryTrackingDto.cs` — OrderId, DeliveryStatus, PartnerName?, PartnerPhone?, CurrentLatitude?, CurrentLongitude?, AssignedAt?, AcceptedAt?, PickedUpAt?, EstimatedDeliveryTime?
    - `PartnerDashboardDto.cs` — IsOnline, TotalDeliveries, TodayDeliveries, TodayEarnings, TotalEarnings
    - `PartnerLocationDto.cs` — Latitude, Longitude, Heading?, Speed?, IsOnline, UpdatedAt
  - **Backend — Commands (15 new)** in `Features/Deliveries/Commands/`:
    - ToggleOnlineStatus (Command + Handler + Validator) — upserts DeliveryPartnerLocation
    - AcceptDelivery (Command + Handler + Validator) — verifies ownership + status, sets Accepted, links Order.DeliveryPartnerId
    - UpdateDeliveryStatus (Command + Handler + Validator) — validates transitions (Accepted→PickedUp→EnRoute→Delivered), syncs Order.Status
    - UpdatePartnerLocation (Command + Handler + Validator) — upserts location + updates active assignment coords
    - AssignDeliveryPartner (Command + Handler + Validator) — finds nearest online partner, creates DeliveryAssignment with earnings
  - **Backend — Queries (8 new)** in `Features/Deliveries/Queries/`:
    - GetMyDeliveriesQuery + Handler — cursor-based pagination on partner's deliveries
    - GetActiveDeliveryQuery + Handler — partner's current active delivery with order/restaurant details
    - GetDeliveryTrackingQuery + Handler — customer-facing tracking with partner info
    - GetPartnerDashboardQuery + Handler — aggregate stats (today/total deliveries/earnings, online status)
  - **Backend — Controller & Contracts (2 new)**:
    - `DeliveryRequests.cs` — ToggleOnlineRequest, UpdateDeliveryStatusRequest, UpdateLocationRequest
    - `DeliveriesController.cs` — 8 endpoints with DeliveryPartner policy
  - **Backend — UpdateOrderStatusCommandHandler (1 modified)**:
    - Injected ISender, auto-triggers AssignDeliveryPartnerCommand when order transitions to Confirmed (delivery orders only)
  - **Flutter — Models (3 new)** in `features/deliveries/data/models/`:
    - `delivery_assignment_model.dart` (Freezed)
    - `delivery_tracking_model.dart` (Freezed)
    - `partner_dashboard_model.dart` (Freezed)
  - **Flutter — Data Layer (2 new + 1 modified)**:
    - `delivery_remote_data_source.dart` — 8 methods (toggle online, get deliveries, get active, get dashboard, accept, update status, update location, get tracking)
    - `delivery_repository.dart` — 8 methods with Failure error mapping
    - `api_constants.dart` (MODIFIED) — 8 new delivery endpoint constants
  - **Flutter — Providers (8 new)** in `features/deliveries/presentation/providers/`:
    - `partner_online_state.dart` + `partner_online_notifier.dart` — online/offline toggle
    - `active_delivery_state.dart` + `active_delivery_notifier.dart` — active delivery management (load, accept, update status, update location)
    - `partner_dashboard_notifier.dart` — dashboard stats + state (combined file with Freezed state)
    - `my_deliveries_state.dart` + `my_deliveries_notifier.dart` — paginated delivery history
    - `delivery_tracking_notifier.dart` — customer-side delivery tracking + state (combined file with Freezed state)
  - **Flutter — Screens (3 new + 1 modified)**:
    - `delivery_dashboard_screen.dart` — Online toggle, stats cards, active delivery card
    - `active_delivery_screen.dart` — Order info, status stepper, action buttons (Accept/PickedUp/StartDelivery/Delivered)
    - `order_tracking_screen.dart` (NEW) — Customer-facing: partner info, map placeholder, status timeline, pull-to-refresh
    - `order_detail_screen.dart` (MODIFIED) — Added "Track Order" button for OutForDelivery status
  - **Flutter — Routing (2 modified)**:
    - `route_names.dart` (MODIFIED) — Added `deliveryDashboard = '/delivery-dashboard'`, `activeDelivery = '/delivery-dashboard/active'`
    - `app_router.dart` (MODIFIED) — Added DeliveryDashboardScreen, ActiveDeliveryScreen, OrderTrackingScreen routes; replaced tracking placeholder
- **API Endpoints**:
  | Method | Path | Auth | Action |
  |--------|------|------|--------|
  | PUT | `/deliveries/online-status` | DeliveryPartner | Toggle online/offline |
  | GET | `/deliveries` | DeliveryPartner | Get my deliveries (paginated) |
  | GET | `/deliveries/active` | DeliveryPartner | Get current active delivery |
  | GET | `/deliveries/dashboard` | DeliveryPartner | Get partner dashboard stats |
  | PUT | `/deliveries/{id}/accept` | DeliveryPartner | Accept delivery |
  | PUT | `/deliveries/{id}/status` | DeliveryPartner | Update delivery status |
  | PUT | `/deliveries/location` | DeliveryPartner | Update partner location |
  | GET | `/deliveries/tracking/{orderId}` | Authorized | Get delivery tracking info |
- **Key Design Decisions**:
  - Delivery partner auto-assigned on order Confirmed via MediatR ISender dispatch in UpdateOrderStatusCommandHandler
  - Nearest available partner selection: exclude busy partners (active non-completed/non-cancelled assignments), then pick first online partner
  - Earnings calculation: ₹30 base (3000 paise) + ₹5/km (500 paise) × 5km default estimate = ₹55
  - Valid delivery status transitions: Assigned→Accepted (via accept), Accepted→PickedUp, PickedUp→EnRoute, EnRoute→Delivered
  - Delivery status syncs to Order status: EnRoute→OutForDelivery, Delivered→Delivered+ActualDeliveryTime
  - AcceptDelivery sets Order.DeliveryPartnerId to link partner to order
  - GetDeliveryTracking verifies customer ownership of order before returning tracking info
  - Dashboard aggregates use separate count/sum queries for today vs total stats
  - Map placeholder in OrderTrackingScreen — real Google Maps integration available via google_maps_flutter in pubspec
- **Pending/Known Issues**:
  - Flutter `.g.dart` and `.freezed.dart` files need code generation for deliveries feature
  - Real geolocation-based nearest partner selection not implemented (uses simple proxy ordering)
  - Distance is hardcoded at 5km — real distance calculation needs Google Maps Distance Matrix API
  - No push notification sent when delivery partner is assigned (wire SendNotificationCommand later)
  - No Kafka event publishing for delivery status changes (delivery.assigned, delivery.location.updated topics defined but not used)
  - Google Maps not rendered in OrderTrackingScreen — needs API key configuration
  - No background location tracking service in Flutter for delivery partner

### Module: Module 11 — Admin Panel
- **Status**: COMPLETED
- **Completed At**: 2026-02-26
- **Files Created**:
  - **Domain Entity Modified (1 file)**:
    - `src/SwiggyClone.Domain/Entities/Restaurant.cs` — Added `StatusReason` (string?, max 500) for admin reject/suspend reason
  - **Backend DTOs (9 files)** in `src/SwiggyClone.Application/Features/Admin/DTOs/`:
    - UserCountsDto.cs, RestaurantCountsDto.cs, OrderCountsDto.cs, RevenueDto.cs
    - AdminOrderSummaryDto.cs, AdminDashboardDto.cs, AdminUserDto.cs, AdminRestaurantDto.cs
    - AdminOrderDetailDto.cs (reuses OrderItemDto + OrderItemAddonDto from Orders module)
  - **Backend Commands (18 files)** in `src/SwiggyClone.Application/Features/Admin/Commands/`:
    - ToggleUserActiveCommand + Handler + Validator
    - ChangeUserRoleCommand + Handler + Validator
    - ApproveRestaurantCommand + Handler + Validator
    - RejectRestaurantCommand + Handler + Validator
    - SuspendRestaurantCommand + Handler + Validator
    - ReactivateRestaurantCommand + Handler + Validator
  - **Backend Queries (14 files)** in `src/SwiggyClone.Application/Features/Admin/Queries/`:
    - GetAdminDashboardQuery + Handler
    - GetAdminUsersQuery + Handler (paginated with search + role filter)
    - GetAdminUserDetailQuery + Handler
    - GetAdminRestaurantsQuery + Handler (paginated with status filter + search)
    - GetAdminRestaurantDetailQuery + Handler
    - GetAdminOrdersQuery + Handler (paginated with status/date filters)
    - GetAdminOrderDetailQuery + Handler
  - **Backend Controller + Contracts (2 files)**:
    - `src/SwiggyClone.Api/Contracts/Admin/AdminRequests.cs` — ToggleUserActiveRequest, ChangeUserRoleRequest, RejectRestaurantRequest, SuspendRestaurantRequest
    - `src/SwiggyClone.Api/Controllers/AdminController.cs` — 13 endpoints under `api/v1/admin` with AdminOnly policy
  - **Flutter Models (4 files)** in `features/admin/data/models/`:
    - admin_dashboard_model.dart (6 Freezed classes: UserCountsModel, RestaurantCountsModel, OrderCountsModel, RevenueModel, AdminOrderSummaryModel, AdminDashboardModel)
    - admin_user_model.dart, admin_restaurant_model.dart, admin_order_detail_model.dart (3 classes)
  - **Flutter Data Layer (2 new + 1 modified)**:
    - admin_remote_data_source.dart — 13 methods matching all API endpoints
    - admin_repository.dart — 13 methods with record-tuple returns
    - api_constants.dart — Added 14 admin endpoint constants
  - **Flutter Providers (10 files)** in `features/admin/presentation/providers/`:
    - admin_dashboard_notifier.dart (combined state + notifier)
    - admin_users_state.dart + admin_users_notifier.dart (paginated, search/role filter, toggle active, change role)
    - admin_user_detail_notifier.dart (combined state + notifier, parameterized by userId)
    - admin_restaurants_state.dart + admin_restaurants_notifier.dart (paginated, status/search filter, approve/reject/suspend/reactivate)
    - admin_restaurant_detail_notifier.dart (combined state + notifier, parameterized by restaurantId)
    - admin_orders_state.dart + admin_orders_notifier.dart (paginated, status/date filter)
    - admin_order_detail_notifier.dart (combined state + notifier, parameterized by orderId)
  - **Flutter Screens (7 files)** in `features/admin/presentation/screens/`:
    - admin_dashboard_screen.dart — Stats cards, recent orders, nav tiles
    - admin_users_screen.dart — Search + role filter chips + user list
    - admin_user_detail_screen.dart — User info + toggle active + change role dialog
    - admin_restaurants_screen.dart — Status filter + search + restaurant cards
    - admin_restaurant_detail_screen.dart — Full info + approve/reject/suspend/reactivate actions
    - admin_orders_screen.dart — Status filter + order cards
    - admin_order_detail_screen.dart — Full order info, items, price breakdown
  - **Flutter Routing (2 modified)**:
    - route_names.dart — Added 7 admin route constants + 3 path helpers
    - app_router.dart — Added nested admin GoRoute tree (/admin, /admin/users, /admin/restaurants, /admin/orders)
- **API Endpoints (13 total)**:
  | # | Method | Path | Action |
  |---|--------|------|--------|
  | 1 | GET | /admin/dashboard | Platform dashboard stats |
  | 2 | GET | /admin/users | List users (paginated, search, role filter) |
  | 3 | GET | /admin/users/{userId} | User detail |
  | 4 | PUT | /admin/users/{userId}/active | Toggle user active/inactive |
  | 5 | PUT | /admin/users/{userId}/role | Change user role |
  | 6 | GET | /admin/restaurants | List restaurants (paginated, status filter, search) |
  | 7 | GET | /admin/restaurants/{id} | Restaurant detail |
  | 8 | PUT | /admin/restaurants/{id}/approve | Approve pending restaurant |
  | 9 | PUT | /admin/restaurants/{id}/reject | Reject pending restaurant |
  | 10 | PUT | /admin/restaurants/{id}/suspend | Suspend active restaurant |
  | 11 | PUT | /admin/restaurants/{id}/reactivate | Reactivate suspended restaurant |
  | 12 | GET | /admin/orders | List all orders (paginated, status/date filter) |
  | 13 | GET | /admin/orders/{orderId} | Order detail |
- **Design Decisions**:
  - All admin endpoints use AdminOnly authorization policy (UserRole.Admin = 4)
  - Offset-based pagination (PagedResult<T>) for admin panels (jump-to-page capability)
  - StatusReason on Restaurant entity for admin reject/suspend audit trail
  - Restaurant status workflow: Pending→Approved/Rejected, Approved→Suspended, Suspended→Approved
  - Reuses OrderItemDto/OrderItemAddonDto from Orders module in AdminOrderDetailDto
  - EF.Functions.Like for case-insensitive search (translates to SQL LIKE)
  - Revenue aggregated as long (paise sums can exceed int range)
- **Pending/Known Issues**:
  - EF Core migration needs regeneration to include StatusReason column on restaurants table
  - Flutter code generation needed: `dart run build_runner build --delete-conflicting-outputs`
  - No admin role guard in Flutter routing (any authenticated user can navigate to /admin — add client-side role check)
  - No pagination controls in admin list screens (load-more exists, but no page-number selector)
  - Dashboard queries issue multiple CountAsync calls — could be optimized with GroupBy in single query

## Active Module
- **Module**: Module 12 — Search & Discovery (Elasticsearch)
- **Status**: NOT_STARTED

## Known Issues / Tech Debt
- Bash shell EINVAL error in Claude Code session (temp directory issue on Windows)
- Docker Compose and build not yet verified end-to-end
- EF Core migration not yet generated (run after `dotnet restore`)
- Flutter code generation needed for all features: `dart run build_runner build --delete-conflicting-outputs`
- Riverpod `.g.dart` files not yet generated for auth, restaurant_management, and customer_discovery providers
- `image_picker` dependency may need adding to pubspec.yaml for DocumentUploadScreen
- LocalFileStorageService saves to wwwroot/uploads — switch to S3/Azure Blob for production
- HomeFeed banners/cuisine chips hardcoded in GetHomeFeedQueryHandler — make DB/CMS-driven later
- Checkout screen uses placeholder delivery address ID — wire real address selection
- Payment integration pending Module 8 (currently CashOnDelivery only)
- `mobile_scanner` and `web_socket_channel` packages may need adding to pubspec.yaml for dine-in feature
- WebSocket backend hub for dine-in real-time updates not yet implemented
- EF Core migration needs regeneration to include DineInSessionId FK on orders table
- Bill payment flow for dine-in deferred to Module 8

## Configuration Summary
- **API Base URL**: http://localhost:5000
- **WebSocket URL**: ws://localhost:5000/ws
- **PostgreSQL**: localhost:5432/swiggy_clone (user: swiggy_admin)
- **Redis**: localhost:6379
- **Kafka**: localhost:9092
- **Elasticsearch**: localhost:9200
- **Swagger**: http://localhost:5000/swagger
- **Nginx**: http://localhost:80

## Key Decisions Made
- UUID v7 for all primary keys (time-sortable, index-friendly)
- Soft-delete pattern on entities with `is_deleted` (global query filter + SoftDeleteInterceptor)
- Non-soft-delete entities use their own Id property (not BaseEntity)
- Junction tables use composite PKs (no surrogate Guid Id)
- Cursor-based pagination (never offset-based) via CursorPagedResult<T>
- Redis for cart storage (ephemeral, fast)
- Kafka for event-driven communication between services
- CQRS with MediatR for command/query separation
- Clean Architecture with strict layer dependency rules (enforced by NetArchTest)
- Result<T> monad for error handling (no exceptions for business logic)
- MediatR pipeline: Logging -> Validation -> Caching -> Handler
- Snake_case PostgreSQL naming convention via AppDbContext.ToSnakeCase()
- Domain events dispatched AFTER SaveChanges via IPublisher
- AuditableEntityInterceptor for automatic created_at/updated_at
- Material 3 with Swiggy orange (#FC8019) theme
- Dio with auto token refresh on 401 + request queuing
- All monetary values stored in paise (smallest currency unit) as INT
- Enum columns stored as SMALLINT (not strings)
- Partial indexes for hot queries (WHERE is_deleted = false, WHERE is_active = true)
- Deterministic GUIDs for seed data (idempotent seeding)
- 3NF normalization for all transactional tables
- Aggregate roots: User, Restaurant, Order (implement IAggregateRoot)
- JWT uses HMAC-SHA256 (symmetric key) not RS256 — simpler for monolith, switch to RS256 for microservices
- IAppDbContext interface decouples Application from Infrastructure (DbSet access without EF Core dependency leak)
- Refresh token reuse detection: if revoked token is reused, ALL user tokens are revoked (security measure)
- Dev OTP service with fixed "123456" and Redis cache — production will use Twilio/MSG91
- BCrypt work factor 12 for password hashing
- Authorization policies registered via extension method (AddAuthorizationPolicies)

## Entity Inheritance Strategy
| Base Class | Used By |
|---|---|
| AuditableEntity (BaseEntity + CreatedBy/UpdatedBy) | User |
| BaseEntity (Id, CreatedAt, UpdatedAt, IsDeleted, DeletedAt) | UserAddress, Restaurant, MenuItem, Order |
| No base class (own Guid Id) | RefreshToken, RestaurantOperatingHours, CuisineType, MenuCategory, MenuItemVariant, MenuItemAddon, OrderItem, OrderItemAddon, OrderStatusHistory, RestaurantTable, DineInSession, DineInSessionMember, Payment, DeliveryAssignment, DeliveryPartnerLocation, Review, ReviewPhoto, Coupon, CouponUsage, UserDevice, Notification |
| Composite PK (no Guid Id) | RestaurantCuisine, UserFavorite |

## Module Execution Order
| # | Module | Status |
|---|--------|--------|
| 0 | Project Scaffold & Docker Base | COMPLETED |
| 1 | Database Foundation | COMPLETED |
| 2 | Auth & Identity | COMPLETED |
| 3 | Restaurant Management | COMPLETED |
| 4 | Customer Discovery | COMPLETED |
| 5 | Cart & Ordering (Delivery) | COMPLETED |
| 6 | Dine-In (Customer) | COMPLETED |
| 7 | Dine-In (Restaurant) | COMPLETED |
| 8 | Payments | COMPLETED |
| 9 | Notifications | COMPLETED |
| 10 | Delivery Partner | COMPLETED |
| 11 | Admin Panel | COMPLETED |
| 12 | Search & Discovery (Elasticsearch) | ⏳ Pending |
| 13 | Ratings & Reviews | ⏳ Pending |
| 14 | Promotions & Coupons | ⏳ Pending |
| 15 | Observability | ⏳ Pending |
| 16 | DevOps & Deployment | ⏳ Pending |
