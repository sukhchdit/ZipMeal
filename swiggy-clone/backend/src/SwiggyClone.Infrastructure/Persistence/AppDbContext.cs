using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence;

/// <summary>
/// Central EF Core DbContext for the application. Implements <see cref="IUnitOfWork"/> to
/// coordinate transactional persistence and dispatches domain events after successful saves.
/// Uses PostgreSQL snake_case naming conventions throughout.
/// </summary>
public sealed class AppDbContext : DbContext, IUnitOfWork, IAppDbContext
{
    private readonly IPublisher _publisher;

    public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    // ── Users & Auth ────────────────────────────────────────────────────
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // ── Restaurants ────────────────────────────────────────────────────
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<RestaurantOperatingHours> RestaurantOperatingHours => Set<RestaurantOperatingHours>();
    public DbSet<CuisineType> CuisineTypes => Set<CuisineType>();
    public DbSet<RestaurantCuisine> RestaurantCuisines => Set<RestaurantCuisine>();

    // ── Menu ───────────────────────────────────────────────────────────
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemVariant> MenuItemVariants => Set<MenuItemVariant>();
    public DbSet<MenuItemAddon> MenuItemAddons => Set<MenuItemAddon>();

    // ── Orders ─────────────────────────────────────────────────────────
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemAddon> OrderItemAddons => Set<OrderItemAddon>();
    public DbSet<OrderStatusHistory> OrderStatusHistory => Set<OrderStatusHistory>();

    // ── Dine-In ────────────────────────────────────────────────────────
    public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
    public DbSet<DineInSession> DineInSessions => Set<DineInSession>();
    public DbSet<DineInSessionMember> DineInSessionMembers => Set<DineInSessionMember>();

    // ── Payments ───────────────────────────────────────────────────────
    public DbSet<Payment> Payments => Set<Payment>();

    // ── Delivery ───────────────────────────────────────────────────────
    public DbSet<DeliveryAssignment> DeliveryAssignments => Set<DeliveryAssignment>();
    public DbSet<DeliveryPartnerLocation> DeliveryPartnerLocations => Set<DeliveryPartnerLocation>();

    // ── Reviews ────────────────────────────────────────────────────────
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewPhoto> ReviewPhotos => Set<ReviewPhoto>();

    // ── Promotions ─────────────────────────────────────────────────────
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CouponUsage> CouponUsages => Set<CouponUsage>();

    // ── Notifications ──────────────────────────────────────────────────
    public DbSet<UserDevice> UserDevices => Set<UserDevice>();
    public DbSet<Notification> Notifications => Set<Notification>();

    // ── Favorites ──────────────────────────────────────────────────────
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
    public DbSet<UserFavoriteItem> UserFavoriteItems => Set<UserFavoriteItem>();

    // ── Banners & Config ─────────────────────────────────────────────
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<PlatformConfig> PlatformConfigs => Set<PlatformConfig>();

    // ── Wallet ─────────────────────────────────────────────────────────
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

    // ── Subscriptions ──────────────────────────────────────────────────
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

    // ── Chat Support ─────────────────────────────────────────────────
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<SupportMessage> SupportMessages => Set<SupportMessage>();
    public DbSet<CannedResponse> CannedResponses => Set<CannedResponse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> implementations from this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply PostgreSQL snake_case naming conventions to all tables, columns, keys,
        // foreign keys, and indexes so the schema is idiomatic for PostgreSQL.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Table name
            var tableName = entity.GetTableName();
            if (tableName is not null)
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            // Column names
            foreach (var property in entity.GetProperties())
            {
                var storeObjectIdentifier = StoreObjectIdentifier.Table(
                    entity.GetTableName()!, entity.GetSchema());
                var columnName = property.GetColumnName(storeObjectIdentifier);
                if (columnName is not null)
                {
                    property.SetColumnName(ToSnakeCase(columnName));
                }
            }

            // Primary and alternate key names
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (keyName is not null)
                {
                    key.SetName(ToSnakeCase(keyName));
                }
            }

            // Foreign key constraint names
            foreach (var fk in entity.GetForeignKeys())
            {
                var constraintName = fk.GetConstraintName();
                if (constraintName is not null)
                {
                    fk.SetConstraintName(ToSnakeCase(constraintName));
                }
            }

            // Index names
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (indexName is not null)
                {
                    index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }
        }

        // Apply a global query filter for soft-delete on every entity derived from BaseEntity.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var method = typeof(AppDbContext)
                .GetMethod(nameof(ApplySoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(null, [modelBuilder]);
        }
    }

    /// <summary>
    /// Persists all changes and then dispatches any domain events raised by tracked entities.
    /// Events are dispatched AFTER the save so that side-effects only fire when persistence succeeds.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Clear events before dispatching to avoid infinite loops if a handler
        // triggers another SaveChangesAsync call.
        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }

    private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    /// <summary>
    /// Converts a PascalCase or camelCase identifier to snake_case.
    /// </summary>
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var builder = new System.Text.StringBuilder(input.Length + 10);
        builder.Append(char.ToLowerInvariant(input[0]));

        for (var i = 1; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                builder.Append('_');
                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }
}
