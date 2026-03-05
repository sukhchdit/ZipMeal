using Microsoft.EntityFrameworkCore;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence;

/// <summary>
/// Seeds the database with reference data (cuisine types), an admin user, sample restaurants,
/// and menu items for development and demonstration purposes. All entities use deterministic
/// GUIDs to ensure idempotency -- re-running the seeder will not create duplicates.
/// </summary>
public static class DataSeeder
{
    // ────────────────────────────────────────────────────────────────────────
    //  Fixed GUIDs — deterministic so the seeder is idempotent.
    // ────────────────────────────────────────────────────────────────────────

    // Admin user
    private static readonly Guid AdminUserId = new("a0000000-0000-0000-0000-000000000001");

    // Restaurant owner users (one per restaurant for clean ownership)
    private static readonly Guid Owner1Id = new("a0000000-0000-0000-0000-000000000011");
    private static readonly Guid Owner2Id = new("a0000000-0000-0000-0000-000000000012");
    private static readonly Guid Owner3Id = new("a0000000-0000-0000-0000-000000000013");
    private static readonly Guid Owner4Id = new("a0000000-0000-0000-0000-000000000014");
    private static readonly Guid Owner5Id = new("a0000000-0000-0000-0000-000000000015");
    private static readonly Guid Owner6Id = new("a0000000-0000-0000-0000-000000000016");
    private static readonly Guid Owner7Id = new("a0000000-0000-0000-0000-000000000017");
    private static readonly Guid Owner8Id = new("a0000000-0000-0000-0000-000000000018");

    // Cuisine types
    private static readonly Guid CuisineNorthIndianId = new("c0000000-0000-0000-0000-000000000001");
    private static readonly Guid CuisineSouthIndianId = new("c0000000-0000-0000-0000-000000000002");
    private static readonly Guid CuisineChineseId     = new("c0000000-0000-0000-0000-000000000003");
    private static readonly Guid CuisineItalianId     = new("c0000000-0000-0000-0000-000000000004");
    private static readonly Guid CuisineMexicanId     = new("c0000000-0000-0000-0000-000000000005");
    private static readonly Guid CuisineJapaneseId    = new("c0000000-0000-0000-0000-000000000006");
    private static readonly Guid CuisineThaiId        = new("c0000000-0000-0000-0000-000000000007");
    private static readonly Guid CuisineContinentalId = new("c0000000-0000-0000-0000-000000000008");
    private static readonly Guid CuisineBiryaniId     = new("c0000000-0000-0000-0000-000000000009");
    private static readonly Guid CuisinePizzaId       = new("c0000000-0000-0000-0000-00000000000a");
    private static readonly Guid CuisineBurgerId      = new("c0000000-0000-0000-0000-00000000000b");
    private static readonly Guid CuisineDessertsId    = new("c0000000-0000-0000-0000-00000000000c");
    private static readonly Guid CuisineBeveragesId   = new("c0000000-0000-0000-0000-00000000000d");
    private static readonly Guid CuisineStreetFoodId  = new("c0000000-0000-0000-0000-00000000000e");
    private static readonly Guid CuisineHealthyId     = new("c0000000-0000-0000-0000-00000000000f");

    // Restaurants
    private static readonly Guid Restaurant1Id = new("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid Restaurant2Id = new("b0000000-0000-0000-0000-000000000002");
    private static readonly Guid Restaurant3Id = new("b0000000-0000-0000-0000-000000000003");
    private static readonly Guid Restaurant4Id = new("b0000000-0000-0000-0000-000000000004");
    private static readonly Guid Restaurant5Id = new("b0000000-0000-0000-0000-000000000005");
    private static readonly Guid Restaurant6Id = new("b0000000-0000-0000-0000-000000000006");
    private static readonly Guid Restaurant7Id = new("b0000000-0000-0000-0000-000000000007");
    private static readonly Guid Restaurant8Id = new("b0000000-0000-0000-0000-000000000008");

    // Menu categories (3 per restaurant = 15 total; restaurants 1–5 get prefix d1..d5)
    private static readonly Guid Cat1Starters   = new("d1000000-0000-0000-0000-000000000001");
    private static readonly Guid Cat1MainCourse = new("d1000000-0000-0000-0000-000000000002");
    private static readonly Guid Cat1Biryani    = new("d1000000-0000-0000-0000-000000000003");

    private static readonly Guid Cat2Pizza      = new("d2000000-0000-0000-0000-000000000001");
    private static readonly Guid Cat2Pasta      = new("d2000000-0000-0000-0000-000000000002");
    private static readonly Guid Cat2Desserts   = new("d2000000-0000-0000-0000-000000000003");

    private static readonly Guid Cat3Starters   = new("d3000000-0000-0000-0000-000000000001");
    private static readonly Guid Cat3MainCourse = new("d3000000-0000-0000-0000-000000000002");
    private static readonly Guid Cat3Noodles    = new("d3000000-0000-0000-0000-000000000003");

    private static readonly Guid Cat4Burgers    = new("d4000000-0000-0000-0000-000000000001");
    private static readonly Guid Cat4Sides      = new("d4000000-0000-0000-0000-000000000002");
    private static readonly Guid Cat4Beverages  = new("d4000000-0000-0000-0000-000000000003");

    private static readonly Guid Cat5Salads     = new("d5000000-0000-0000-0000-000000000001");
    private static readonly Guid Cat5Bowls      = new("d5000000-0000-0000-0000-000000000002");
    private static readonly Guid Cat5Smoothies  = new("d5000000-0000-0000-0000-000000000003");

    // Restaurant 6 — Taco Loco
    private static readonly Guid Cat6Tacos     = new("d6000000-0000-0000-0000-000000000001");
    private static readonly Guid Cat6Burritos  = new("d6000000-0000-0000-0000-000000000002");
    private static readonly Guid Cat6Sides     = new("d6000000-0000-0000-0000-000000000003");

    // Restaurant 7 — Sushi Sen
    private static readonly Guid Cat7Sushi     = new("d7000000-0000-0000-0000-000000000001");
    private static readonly Guid Cat7HotDishes = new("d7000000-0000-0000-0000-000000000002");
    private static readonly Guid Cat7Bento     = new("d7000000-0000-0000-0000-000000000003");

    // Restaurant 8 — Dosa Factory
    private static readonly Guid Cat8Dosas     = new("d8000000-0000-0000-0000-000000000001");
    private static readonly Guid Cat8IdliVada  = new("d8000000-0000-0000-0000-000000000002");
    private static readonly Guid Cat8Beverages = new("d8000000-0000-0000-0000-000000000003");

    private static readonly DateTimeOffset SeedTimestamp = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    /// <summary>
    /// Seeds the database with reference data, sample users, restaurants, and menu items.
    /// Skips seeding entirely if any <see cref="CuisineType"/> records already exist.
    /// </summary>
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Idempotency guard: skip if seed data is already present.
        if (await context.CuisineTypes.AnyAsync())
        {
            // Incremental: seed restaurants added after initial seeding.
            if (!await context.Restaurants.AnyAsync(r => r.Id == Restaurant6Id))
            {
                SeedNewRestaurantsBatch2(context);
                await context.SaveChangesAsync();
            }

            return;
        }

        SeedCuisineTypes(context);
        SeedUsers(context);
        SeedRestaurants(context);
        SeedRestaurantCuisines(context);
        SeedOperatingHours(context);
        SeedTables(context);
        SeedMenuCategoriesAndItems(context);
        SeedNewRestaurantsBatch2(context);

        await context.SaveChangesAsync();
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Unsplash image helper — builds a permanent direct-link URL.
    // ────────────────────────────────────────────────────────────────────────

    private static string Img(string photoId, int w, int h)
        => $"https://images.unsplash.com/{photoId}?w={w}&h={h}&fit=crop&q=80";

    // ────────────────────────────────────────────────────────────────────────
    //  Cuisine Types
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedCuisineTypes(AppDbContext context)
    {
        CuisineType[] cuisines =
        [
            new() { Id = CuisineNorthIndianId, Name = "North Indian",  SortOrder = 1,  IsActive = true, IconUrl = Img("photo-1585937421612-70a008356fbe", 200, 200) },
            new() { Id = CuisineSouthIndianId, Name = "South Indian",  SortOrder = 2,  IsActive = true, IconUrl = Img("photo-1630383249896-424e482df921", 200, 200) },
            new() { Id = CuisineChineseId,     Name = "Chinese",       SortOrder = 3,  IsActive = true, IconUrl = Img("photo-1525755662778-989d0524087e", 200, 200) },
            new() { Id = CuisineItalianId,     Name = "Italian",       SortOrder = 4,  IsActive = true, IconUrl = Img("photo-1498579150354-977475b7ea0b", 200, 200) },
            new() { Id = CuisineMexicanId,     Name = "Mexican",       SortOrder = 5,  IsActive = true, IconUrl = Img("photo-1565299585323-38d6b0865b47", 200, 200) },
            new() { Id = CuisineJapaneseId,    Name = "Japanese",      SortOrder = 6,  IsActive = true, IconUrl = Img("photo-1579871494447-9811cf80d66c", 200, 200) },
            new() { Id = CuisineThaiId,        Name = "Thai",          SortOrder = 7,  IsActive = true, IconUrl = Img("photo-1559314809-0d155014e29e", 200, 200) },
            new() { Id = CuisineContinentalId, Name = "Continental",   SortOrder = 8,  IsActive = true, IconUrl = Img("photo-1414235077428-338989a2e8c0", 200, 200) },
            new() { Id = CuisineBiryaniId,     Name = "Biryani",       SortOrder = 9,  IsActive = true, IconUrl = Img("photo-1563379091339-03b21ab4a4f8", 200, 200) },
            new() { Id = CuisinePizzaId,       Name = "Pizza",         SortOrder = 10, IsActive = true, IconUrl = Img("photo-1565299624946-b28f40a0ae38", 200, 200) },
            new() { Id = CuisineBurgerId,      Name = "Burger",        SortOrder = 11, IsActive = true, IconUrl = Img("photo-1568901346375-23c9450c58cd", 200, 200) },
            new() { Id = CuisineDessertsId,    Name = "Desserts",      SortOrder = 12, IsActive = true, IconUrl = Img("photo-1551024506-0bccd828d307", 200, 200) },
            new() { Id = CuisineBeveragesId,   Name = "Beverages",     SortOrder = 13, IsActive = true, IconUrl = Img("photo-1544145945-f90425340c7e", 200, 200) },
            new() { Id = CuisineStreetFoodId,  Name = "Street Food",   SortOrder = 14, IsActive = true, IconUrl = Img("photo-1601050690597-df0568f70950", 200, 200) },
            new() { Id = CuisineHealthyId,     Name = "Healthy",       SortOrder = 15, IsActive = true, IconUrl = Img("photo-1512621776951-a57141f2eefd", 200, 200) },
        ];

        context.CuisineTypes.AddRange(cuisines);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Users (Admin + 5 Restaurant Owners)
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedUsers(AppDbContext context)
    {
        User[] users =
        [
            new()
            {
                Id           = AdminUserId,
                PhoneNumber  = "+919999999999",
                Email        = "admin@swiggyclone.com",
                FullName     = "System Admin",
                Role         = UserRole.Admin,
                IsVerified   = true,
                IsActive     = true,
                ReferralCode = "ADMN0001",
                CreatedAt    = SeedTimestamp,
                UpdatedAt    = SeedTimestamp,
            },
            CreateOwner(Owner1Id, "+919800000001", "owner1@swiggyclone.com", "Rajesh Kumar"),
            CreateOwner(Owner2Id, "+919800000002", "owner2@swiggyclone.com", "Marco Rossi"),
            CreateOwner(Owner3Id, "+919800000003", "owner3@swiggyclone.com", "Wei Chen"),
            CreateOwner(Owner4Id, "+919800000004", "owner4@swiggyclone.com", "James Miller"),
            CreateOwner(Owner5Id, "+919800000005", "owner5@swiggyclone.com", "Priya Sharma"),
        ];

        context.Users.AddRange(users);
    }

    private static User CreateOwner(Guid id, string phone, string email, string name)
    {
        return new User
        {
            Id           = id,
            PhoneNumber  = phone,
            Email        = email,
            FullName     = name,
            Role         = UserRole.RestaurantOwner,
            IsVerified   = true,
            IsActive     = true,
            ReferralCode = $"OWN{phone[^4..]}",
            CreatedAt    = SeedTimestamp,
            UpdatedAt    = SeedTimestamp,
        };
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Restaurants
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedRestaurants(AppDbContext context)
    {
        Restaurant[] restaurants =
        [
            new()
            {
                Id                 = Restaurant1Id,
                OwnerId            = Owner1Id,
                Name               = "Spice Garden",
                Slug               = "spice-garden",
                Description        = "Authentic North Indian and South Indian cuisine with signature biryanis prepared in traditional dum style.",
                PhoneNumber        = "+918000000001",
                Email              = "contact@spicegarden.in",
                LogoUrl            = Img("photo-1596040033229-a9821ebd058d", 200, 200),
                BannerUrl          = Img("photo-1585937421612-70a008356fbe", 800, 400),
                AddressLine1       = "42, MG Road",
                AddressLine2       = "Near Brigade Gateway",
                City               = "Bangalore",
                State              = "Karnataka",
                PostalCode         = "560001",
                Latitude           = 12.9716,
                Longitude          = 77.5946,
                AverageRating      = 4.3m,
                TotalRatings       = 1240,
                AvgDeliveryTimeMin = 35,
                AvgCostForTwo      = 60000, // INR 600.00
                IsVegOnly          = false,
                IsAcceptingOrders  = true,
                IsDineInEnabled    = true,
                CommissionRate     = 15.00m,
                FssaiLicense       = "11520999000123",
                GstNumber          = "29AABCU9603R1ZP",
                Status             = RestaurantStatus.Approved,
                CreatedAt          = SeedTimestamp,
                UpdatedAt          = SeedTimestamp,
            },
            new()
            {
                Id                 = Restaurant2Id,
                OwnerId            = Owner2Id,
                Name               = "Pizza Paradise",
                Slug               = "pizza-paradise",
                Description        = "Wood-fired Italian pizzas and handmade pasta crafted with imported ingredients.",
                PhoneNumber        = "+918000000002",
                Email              = "contact@pizzaparadise.in",
                LogoUrl            = Img("photo-1565299624946-b28f40a0ae38", 200, 200),
                BannerUrl          = Img("photo-1513104890138-7c749659a591", 800, 400),
                AddressLine1       = "15, Linking Road",
                AddressLine2       = "Bandra West",
                City               = "Mumbai",
                State              = "Maharashtra",
                PostalCode         = "400050",
                Latitude           = 19.0596,
                Longitude          = 72.8295,
                AverageRating      = 4.5m,
                TotalRatings       = 2100,
                AvgDeliveryTimeMin = 30,
                AvgCostForTwo      = 80000, // INR 800.00
                IsVegOnly          = false,
                IsAcceptingOrders  = true,
                IsDineInEnabled    = false,
                CommissionRate     = 12.50m,
                FssaiLicense       = "11521999000456",
                GstNumber          = "27AABCU9603R1ZM",
                Status             = RestaurantStatus.Approved,
                CreatedAt          = SeedTimestamp,
                UpdatedAt          = SeedTimestamp,
            },
            new()
            {
                Id                 = Restaurant3Id,
                OwnerId            = Owner3Id,
                Name               = "Dragon Wok",
                Slug               = "dragon-wok",
                Description        = "Pan-Asian flavors featuring authentic Chinese, Thai, and Japanese dishes with a modern twist.",
                PhoneNumber        = "+918000000003",
                Email              = "contact@dragonwok.in",
                LogoUrl            = Img("photo-1569718212165-3a8278d5f624", 200, 200),
                BannerUrl          = Img("photo-1526318896980-cf78c088247c", 800, 400),
                AddressLine1       = "88, Connaught Place",
                AddressLine2       = "Block C, Inner Circle",
                City               = "Delhi",
                State              = "Delhi",
                PostalCode         = "110001",
                Latitude           = 28.6315,
                Longitude          = 77.2167,
                AverageRating      = 4.1m,
                TotalRatings       = 890,
                AvgDeliveryTimeMin = 40,
                AvgCostForTwo      = 70000, // INR 700.00
                IsVegOnly          = false,
                IsAcceptingOrders  = true,
                IsDineInEnabled    = true,
                CommissionRate     = 15.00m,
                FssaiLicense       = "11522999000789",
                GstNumber          = "07AABCU9603R1ZD",
                Status             = RestaurantStatus.Approved,
                CreatedAt          = SeedTimestamp,
                UpdatedAt          = SeedTimestamp,
            },
            new()
            {
                Id                 = Restaurant4Id,
                OwnerId            = Owner4Id,
                Name               = "Burger Barn",
                Slug               = "burger-barn",
                Description        = "Gourmet burgers and continental sides made with premium grass-fed beef and artisan buns.",
                PhoneNumber        = "+918000000004",
                Email              = "contact@burgerbarn.in",
                LogoUrl            = Img("photo-1568901346375-23c9450c58cd", 200, 200),
                BannerUrl          = Img("photo-1550547660-d9450f859349", 800, 400),
                AddressLine1       = "23, Jubilee Hills",
                AddressLine2       = "Road No. 36",
                City               = "Hyderabad",
                State              = "Telangana",
                PostalCode         = "500033",
                Latitude           = 17.4326,
                Longitude          = 78.4071,
                AverageRating      = 4.4m,
                TotalRatings       = 1560,
                AvgDeliveryTimeMin = 25,
                AvgCostForTwo      = 50000, // INR 500.00
                IsVegOnly          = false,
                IsAcceptingOrders  = true,
                IsDineInEnabled    = false,
                CommissionRate     = 14.00m,
                FssaiLicense       = "11523999000321",
                GstNumber          = "36AABCU9603R1ZT",
                Status             = RestaurantStatus.Approved,
                CreatedAt          = SeedTimestamp,
                UpdatedAt          = SeedTimestamp,
            },
            new()
            {
                Id                 = Restaurant5Id,
                OwnerId            = Owner5Id,
                Name               = "Green Bowl",
                Slug               = "green-bowl",
                Description        = "Farm-to-table healthy bowls, fresh salads, and cold-pressed smoothies for the health-conscious diner.",
                PhoneNumber        = "+918000000005",
                Email              = "contact@greenbowl.in",
                LogoUrl            = Img("photo-1540420773420-3366772f4999", 200, 200),
                BannerUrl          = Img("photo-1512621776951-a57141f2eefd", 800, 400),
                AddressLine1       = "7, Koregaon Park",
                AddressLine2       = "Lane 5, North Main Road",
                City               = "Pune",
                State              = "Maharashtra",
                PostalCode         = "411001",
                Latitude           = 18.5362,
                Longitude          = 73.8939,
                AverageRating      = 4.6m,
                TotalRatings       = 780,
                AvgDeliveryTimeMin = 20,
                AvgCostForTwo      = 45000, // INR 450.00
                IsVegOnly          = true,
                IsAcceptingOrders  = true,
                IsDineInEnabled    = true,
                CommissionRate     = 13.00m,
                FssaiLicense       = "11524999000654",
                GstNumber          = "27AABCU9603R1ZP",
                Status             = RestaurantStatus.Approved,
                CreatedAt          = SeedTimestamp,
                UpdatedAt          = SeedTimestamp,
            },
        ];

        context.Restaurants.AddRange(restaurants);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Restaurant ↔ Cuisine junction records
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedRestaurantCuisines(AppDbContext context)
    {
        RestaurantCuisine[] junctions =
        [
            // Spice Garden: North Indian, South Indian, Biryani
            new() { RestaurantId = Restaurant1Id, CuisineId = CuisineNorthIndianId },
            new() { RestaurantId = Restaurant1Id, CuisineId = CuisineSouthIndianId },
            new() { RestaurantId = Restaurant1Id, CuisineId = CuisineBiryaniId },

            // Pizza Paradise: Italian, Pizza
            new() { RestaurantId = Restaurant2Id, CuisineId = CuisineItalianId },
            new() { RestaurantId = Restaurant2Id, CuisineId = CuisinePizzaId },

            // Dragon Wok: Chinese, Thai, Japanese
            new() { RestaurantId = Restaurant3Id, CuisineId = CuisineChineseId },
            new() { RestaurantId = Restaurant3Id, CuisineId = CuisineThaiId },
            new() { RestaurantId = Restaurant3Id, CuisineId = CuisineJapaneseId },

            // Burger Barn: Burger, Continental
            new() { RestaurantId = Restaurant4Id, CuisineId = CuisineBurgerId },
            new() { RestaurantId = Restaurant4Id, CuisineId = CuisineContinentalId },

            // Green Bowl: Healthy, Continental
            new() { RestaurantId = Restaurant5Id, CuisineId = CuisineHealthyId },
            new() { RestaurantId = Restaurant5Id, CuisineId = CuisineContinentalId },
        ];

        context.RestaurantCuisines.AddRange(junctions);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Operating Hours (Mon–Sun for all 5 restaurants)
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedOperatingHours(AppDbContext context)
    {
        // Guid seed offsets per restaurant (0xE1..0xE5 prefix range).
        var hoursGuidBase = new[]
        {
            (RestId: Restaurant1Id, Prefix: "e1000000-0000-0000-0000-00000000000"),
            (RestId: Restaurant2Id, Prefix: "e2000000-0000-0000-0000-00000000000"),
            (RestId: Restaurant3Id, Prefix: "e3000000-0000-0000-0000-00000000000"),
            (RestId: Restaurant4Id, Prefix: "e4000000-0000-0000-0000-00000000000"),
            (RestId: Restaurant5Id, Prefix: "e5000000-0000-0000-0000-00000000000"),
        };

        var openTime  = new TimeOnly(9, 0);   // 09:00
        var closeTime = new TimeOnly(23, 0);   // 23:00

        var hours = new List<RestaurantOperatingHours>();

        foreach (var (restId, prefix) in hoursGuidBase)
        {
            for (short day = 0; day <= 6; day++) // 0 = Sunday .. 6 = Saturday
            {
                hours.Add(new RestaurantOperatingHours
                {
                    Id           = Guid.Parse($"{prefix}{day:x1}"),
                    RestaurantId = restId,
                    DayOfWeek    = day,
                    OpenTime     = openTime,
                    CloseTime    = closeTime,
                    IsClosed     = false,
                });
            }
        }

        context.RestaurantOperatingHours.AddRange(hours);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Restaurant Tables (dine-in enabled: restaurants 1, 3, 5)
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedTables(AppDbContext context)
    {
        var tables = new List<RestaurantTable>();

        // Restaurant 1 — Spice Garden: 4 tables
        AddTables(tables, Restaurant1Id, "f1000000-0000-0000-0000-00000000000", 4, "Ground Floor");

        // Restaurant 3 — Dragon Wok: 3 tables
        AddTables(tables, Restaurant3Id, "f3000000-0000-0000-0000-00000000000", 3, "Main Hall");

        // Restaurant 5 — Green Bowl: 3 tables
        AddTables(tables, Restaurant5Id, "f5000000-0000-0000-0000-00000000000", 3, "Garden Area");

        context.RestaurantTables.AddRange(tables);
    }

    private static void AddTables(
        List<RestaurantTable> tables,
        Guid restaurantId,
        string guidPrefix,
        int count,
        string floorSection)
    {
        for (var i = 1; i <= count; i++)
        {
            tables.Add(new RestaurantTable
            {
                Id           = Guid.Parse($"{guidPrefix}{i:x1}"),
                RestaurantId = restaurantId,
                TableNumber  = $"T{i}",
                Capacity     = i <= 2 ? 2 : 4, // first two tables seat 2, rest seat 4
                FloorSection = floorSection,
                QrCodeData   = $"DINE-{restaurantId:N}-T{i}",
                Status       = TableStatus.Available,
                IsActive     = true,
                CreatedAt    = SeedTimestamp,
                UpdatedAt    = SeedTimestamp,
            });
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Menu Categories & Items (50+ items across 5 restaurants)
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedMenuCategoriesAndItems(AppDbContext context)
    {
        // ── Categories ─────────────────────────────────────────────────────
        MenuCategory[] categories =
        [
            // Restaurant 1 — Spice Garden
            Cat(Cat1Starters,   Restaurant1Id, "Starters",    "Crispy appetizers and chaats", 1),
            Cat(Cat1MainCourse, Restaurant1Id, "Main Course", "Rich gravies and tandoori specials", 2),
            Cat(Cat1Biryani,    Restaurant1Id, "Biryani",     "Dum-cooked biryanis and rice dishes", 3),

            // Restaurant 2 — Pizza Paradise
            Cat(Cat2Pizza,    Restaurant2Id, "Pizza",    "Hand-tossed and thin-crust pizzas", 1),
            Cat(Cat2Pasta,    Restaurant2Id, "Pasta",    "Fresh pasta with authentic Italian sauces", 2),
            Cat(Cat2Desserts, Restaurant2Id, "Desserts", "Classic Italian sweets", 3),

            // Restaurant 3 — Dragon Wok
            Cat(Cat3Starters,   Restaurant3Id, "Starters",    "Dim sum, spring rolls, and soups", 1),
            Cat(Cat3MainCourse, Restaurant3Id, "Main Course", "Signature wok-fried entrees", 2),
            Cat(Cat3Noodles,    Restaurant3Id, "Noodles",     "Hand-pulled and stir-fried noodles", 3),

            // Restaurant 4 — Burger Barn
            Cat(Cat4Burgers,   Restaurant4Id, "Burgers",    "Juicy gourmet burgers", 1),
            Cat(Cat4Sides,     Restaurant4Id, "Sides",      "Fries, wings, and onion rings", 2),
            Cat(Cat4Beverages, Restaurant4Id, "Beverages",  "Shakes, coolers, and sodas", 3),

            // Restaurant 5 — Green Bowl
            Cat(Cat5Salads,    Restaurant5Id, "Salads",     "Garden-fresh salads with house dressings", 1),
            Cat(Cat5Bowls,     Restaurant5Id, "Bowls",      "Protein-packed grain and noodle bowls", 2),
            Cat(Cat5Smoothies, Restaurant5Id, "Smoothies",  "Cold-pressed and blended smoothies", 3),
        ];

        context.MenuCategories.AddRange(categories);

        // ── Menu Items ─────────────────────────────────────────────────────
        // Guid counter — we use a helper to generate sequential fixed GUIDs per restaurant.
        var items    = new List<MenuItem>();
        var variants = new List<MenuItemVariant>();
        var addons   = new List<MenuItemAddon>();

        // ---------- Restaurant 1: Spice Garden ----------
        var r1Seq = new GuidSequence("10000000-0000-0000-0000-");

        // Starters
        items.Add(Item(r1Seq.Next(), Cat1Starters, Restaurant1Id, "Paneer Tikka",          "Marinated cottage cheese grilled in a tandoor",       17900, true,  true,  20, 1, Img("photo-1567188040759-fb8a883dc6d8", 400, 300)));
        items.Add(Item(r1Seq.Next(), Cat1Starters, Restaurant1Id, "Chicken Seekh Kebab",   "Minced chicken spiced and skewered on charcoal",      19900, false, false, 25, 2, Img("photo-1599487488170-d11ec9c172f0", 400, 300)));
        items.Add(Item(r1Seq.Next(), Cat1Starters, Restaurant1Id, "Samosa (2 pcs)",        "Crispy pastry stuffed with spiced potatoes and peas", 6900,  true,  true,  10, 3, Img("photo-1601050690597-df0568f70950", 400, 300)));
        items.Add(Item(r1Seq.Next(), Cat1Starters, Restaurant1Id, "Dahi Puri",             "Crispy puris filled with yogurt and tangy chutney",   8900,  true,  false, 10, 4, Img("photo-1606491956689-2ea866880049", 400, 300)));

        // Main Course
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Butter Chicken",      "Tender chicken in rich tomato-butter gravy",          29900, false, true,  30, 1, Img("photo-1603894584373-5ac82b2ae398", 400, 300)));
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Dal Makhani",         "Slow-cooked black lentils with cream and butter",     19900, true,  true,  25, 2, Img("photo-1546833999-b9f581a1996d", 400, 300)));
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Palak Paneer",        "Cottage cheese cubes in smooth spinach gravy",        22900, true,  false, 25, 3, Img("photo-1588166524941-3bf61a9c41db", 400, 300)));
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Rogan Josh",          "Kashmiri-style slow-braised lamb curry",              34900, false, false, 35, 4, Img("photo-1545247181-516773cae754", 400, 300)));
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Garlic Naan",         "Soft naan bread topped with garlic and coriander",    5900,  true,  true,  8,  5, Img("photo-1565557623262-b51c2513a641", 400, 300)));

        // Biryani
        var biryaniChickenId = r1Seq.Next();
        items.Add(Item(biryaniChickenId, Cat1Biryani, Restaurant1Id, "Chicken Dum Biryani", "Aromatic basmati rice layered with spiced chicken",  25900, false, true,  40, 1, Img("photo-1563379091339-03b21ab4a4f8", 400, 300)));
        items.Add(Item(r1Seq.Next(),     Cat1Biryani, Restaurant1Id, "Veg Biryani",         "Fragrant rice with seasonal vegetables and saffron", 19900, true,  false, 35, 2, Img("photo-1645177628172-a94c1f96e6db", 400, 300)));
        items.Add(Item(r1Seq.Next(),     Cat1Biryani, Restaurant1Id, "Mutton Biryani",      "Tender mutton pieces in slow-cooked hyderabadi rice",32900, false, false, 45, 3, Img("photo-1642821373181-696a54913e93", 400, 300)));

        // Variants for Chicken Dum Biryani
        var vSeq1 = new GuidSequence("11000000-0000-0000-0000-");
        variants.Add(Variant(vSeq1.Next(), biryaniChickenId, "Half",    0,      true,  1));
        variants.Add(Variant(vSeq1.Next(), biryaniChickenId, "Full",    10000,  false, 2)); // +INR 100
        variants.Add(Variant(vSeq1.Next(), biryaniChickenId, "Family",  25000,  false, 3)); // +INR 250

        // Addons for Paneer Tikka (first item)
        var aSeq1 = new GuidSequence("12000000-0000-0000-0000-");
        var paneerTikkaId = items[0].Id;
        addons.Add(Addon(aSeq1.Next(), paneerTikkaId, "Extra Mint Chutney", 2900,  true,  1));
        addons.Add(Addon(aSeq1.Next(), paneerTikkaId, "Extra Cheese",       4900,  true,  2));

        // ---------- Restaurant 2: Pizza Paradise ----------
        var r2Seq = new GuidSequence("20000000-0000-0000-0000-");

        // Pizza
        var margheritaId = r2Seq.Next();
        items.Add(Item(margheritaId,  Cat2Pizza, Restaurant2Id, "Margherita",         "Classic tomato, mozzarella, and fresh basil",         19900, true,  true,  20, 1, Img("photo-1574071318508-1cdbab80d002", 400, 300)));
        items.Add(Item(r2Seq.Next(),  Cat2Pizza, Restaurant2Id, "Pepperoni",          "Spicy pepperoni with mozzarella on tomato base",      29900, false, true,  20, 2, Img("photo-1628840042765-356cda07504e", 400, 300)));
        items.Add(Item(r2Seq.Next(),  Cat2Pizza, Restaurant2Id, "BBQ Chicken",        "Grilled chicken, BBQ sauce, red onions, mozzarella",  32900, false, false, 25, 3, Img("photo-1565299624946-b28f40a0ae38", 400, 300)));
        items.Add(Item(r2Seq.Next(),  Cat2Pizza, Restaurant2Id, "Farm Fresh Veggie",  "Bell peppers, olives, mushrooms, sweet corn",         24900, true,  false, 20, 4, Img("photo-1511689660979-10d2b1aada49", 400, 300)));
        items.Add(Item(r2Seq.Next(),  Cat2Pizza, Restaurant2Id, "Truffle Mushroom",   "Wild mushrooms with truffle oil and parmesan",        37900, true,  false, 25, 5, Img("photo-1573821663912-569905455b1c", 400, 300)));

        // Pasta
        items.Add(Item(r2Seq.Next(), Cat2Pasta, Restaurant2Id, "Spaghetti Carbonara", "Creamy egg and parmesan sauce with crispy pancetta",  27900, false, true,  18, 1, Img("photo-1612874742237-6526221588e3", 400, 300)));
        items.Add(Item(r2Seq.Next(), Cat2Pasta, Restaurant2Id, "Penne Arrabbiata",    "Spicy tomato sauce with garlic and red chili flakes", 21900, true,  false, 15, 2, Img("photo-1563379926898-05f4575a45d8", 400, 300)));
        items.Add(Item(r2Seq.Next(), Cat2Pasta, Restaurant2Id, "Alfredo Pasta",       "Rich cream and parmesan sauce with fettuccine",       25900, true,  true,  18, 3, Img("photo-1645112411341-6c4fd023714a", 400, 300)));

        // Desserts
        items.Add(Item(r2Seq.Next(), Cat2Desserts, Restaurant2Id, "Tiramisu",         "Classic Italian coffee-flavored layered dessert",     19900, true,  true,  5,  1, Img("photo-1571877227200-a0d98ea607e9", 400, 300)));
        items.Add(Item(r2Seq.Next(), Cat2Desserts, Restaurant2Id, "Panna Cotta",      "Silky vanilla cream topped with berry coulis",        17900, true,  false, 5,  2, Img("photo-1488477181946-6428a0291777", 400, 300)));
        items.Add(Item(r2Seq.Next(), Cat2Desserts, Restaurant2Id, "Chocolate Lava Cake","Warm dark chocolate cake with molten center",       21900, true,  false, 12, 3, Img("photo-1624353365286-3f8d62daad51", 400, 300)));

        // Variants for Margherita (Small/Medium/Large)
        var vSeq2 = new GuidSequence("21000000-0000-0000-0000-");
        variants.Add(Variant(vSeq2.Next(), margheritaId, "Small (7\")",   0,     true,  1));
        variants.Add(Variant(vSeq2.Next(), margheritaId, "Medium (10\")", 8000,  false, 2)); // +INR 80
        variants.Add(Variant(vSeq2.Next(), margheritaId, "Large (12\")",  15000, false, 3)); // +INR 150

        // Addons for Margherita
        var aSeq2 = new GuidSequence("22000000-0000-0000-0000-");
        addons.Add(Addon(aSeq2.Next(), margheritaId, "Extra Cheese",     5900, true,  1));
        addons.Add(Addon(aSeq2.Next(), margheritaId, "Jalapenos",        3900, true,  2));
        addons.Add(Addon(aSeq2.Next(), margheritaId, "Garlic Bread (4)", 7900, true,  3));

        // ---------- Restaurant 3: Dragon Wok ----------
        var r3Seq = new GuidSequence("30000000-0000-0000-0000-");

        // Starters
        items.Add(Item(r3Seq.Next(), Cat3Starters, Restaurant3Id, "Veg Spring Rolls (4 pcs)",   "Crispy rolls stuffed with julienned vegetables",    14900, true,  true,  12, 1, Img("photo-1548507200-b4ef8a49abd5", 400, 300)));
        items.Add(Item(r3Seq.Next(), Cat3Starters, Restaurant3Id, "Chicken Dimsum (6 pcs)",     "Steamed dumplings with minced chicken filling",      17900, false, true,  15, 2, Img("photo-1496116218417-1a781b1c416c", 400, 300)));
        items.Add(Item(r3Seq.Next(), Cat3Starters, Restaurant3Id, "Tom Yum Soup",               "Spicy and sour Thai soup with mushrooms and herbs",  13900, true,  false, 10, 3, Img("photo-1548943487-a2e4e43b4853", 400, 300)));
        items.Add(Item(r3Seq.Next(), Cat3Starters, Restaurant3Id, "Edamame",                    "Steamed Japanese soybeans with sea salt",             9900, true,  false, 8,  4, Img("photo-1564834724105-918b73d1b8e0", 400, 300)));

        // Main Course
        var kungPaoId = r3Seq.Next();
        items.Add(Item(kungPaoId,    Cat3MainCourse, Restaurant3Id, "Kung Pao Chicken",     "Wok-tossed chicken with peanuts and dried chili",       26900, false, true,  25, 1, Img("photo-1525755662778-989d0524087e", 400, 300)));
        items.Add(Item(r3Seq.Next(), Cat3MainCourse, Restaurant3Id, "Thai Green Curry",     "Creamy coconut curry with thai basil and vegetables",    24900, true,  false, 25, 2, Img("photo-1559314809-0d155014e29e", 400, 300)));
        items.Add(Item(r3Seq.Next(), Cat3MainCourse, Restaurant3Id, "Teriyaki Salmon",      "Grilled salmon fillet glazed with teriyaki sauce",      39900, false, false, 30, 3, Img("photo-1467003909585-2f8a72700288", 400, 300)));
        items.Add(Item(r3Seq.Next(), Cat3MainCourse, Restaurant3Id, "Mapo Tofu",            "Silken tofu in spicy Sichuan bean sauce",               19900, true,  false, 20, 4, Img("photo-1582452919408-aca4a0ed3889", 400, 300)));

        // Noodles
        items.Add(Item(r3Seq.Next(), Cat3Noodles, Restaurant3Id, "Hakka Noodles",           "Stir-fried noodles with mixed vegetables",              17900, true,  true,  15, 1, Img("photo-1569718212165-3a8278d5f624", 400, 300)));
        items.Add(Item(r3Seq.Next(), Cat3Noodles, Restaurant3Id, "Pad Thai",                "Thai rice noodles with tamarind sauce and peanuts",      21900, false, true,  20, 2, Img("photo-1559314809-0d155014e29e", 400, 300)));
        items.Add(Item(r3Seq.Next(), Cat3Noodles, Restaurant3Id, "Ramen",                   "Rich pork broth with wheat noodles and soft-boiled egg", 24900, false, false, 25, 3, Img("photo-1557872943-16a5ac26437e", 400, 300)));

        // Variants for Kung Pao Chicken
        var vSeq3 = new GuidSequence("31000000-0000-0000-0000-");
        variants.Add(Variant(vSeq3.Next(), kungPaoId, "Regular",  0,     true,  1));
        variants.Add(Variant(vSeq3.Next(), kungPaoId, "Large",    8000,  false, 2));

        // Addons for Kung Pao Chicken
        var aSeq3 = new GuidSequence("32000000-0000-0000-0000-");
        addons.Add(Addon(aSeq3.Next(), kungPaoId, "Extra Peanuts",     2900, true,  1));
        addons.Add(Addon(aSeq3.Next(), kungPaoId, "Steamed Rice",      5900, true,  2));
        addons.Add(Addon(aSeq3.Next(), kungPaoId, "Fried Rice Upgrade",7900, true,  3));

        // ---------- Restaurant 4: Burger Barn ----------
        var r4Seq = new GuidSequence("40000000-0000-0000-0000-");

        // Burgers
        var classicBurgerId = r4Seq.Next();
        items.Add(Item(classicBurgerId, Cat4Burgers, Restaurant4Id, "Classic Smash Burger",  "Double-smashed beef patties with American cheese",  24900, false, true,  15, 1, Img("photo-1568901346375-23c9450c58cd", 400, 300)));
        items.Add(Item(r4Seq.Next(),    Cat4Burgers, Restaurant4Id, "BBQ Bacon Burger",      "Beef patty with crispy bacon and smoky BBQ sauce",  29900, false, true,  15, 2, Img("photo-1553979459-d2229ba7433b", 400, 300)));
        items.Add(Item(r4Seq.Next(),    Cat4Burgers, Restaurant4Id, "Mushroom Swiss Burger", "Sauteed mushrooms and Swiss cheese on beef patty",  27900, false, false, 15, 3, Img("photo-1572802419224-296b0aeee0d9", 400, 300)));
        items.Add(Item(r4Seq.Next(),    Cat4Burgers, Restaurant4Id, "Veggie Crunch Burger",  "Crispy vegetable patty with lettuce and sriracha",  19900, true,  false, 12, 4, Img("photo-1520072959219-c595e6cdc07a", 400, 300)));
        items.Add(Item(r4Seq.Next(),    Cat4Burgers, Restaurant4Id, "Chicken Zinger",        "Crispy fried chicken fillet with spicy mayo",       22900, false, true,  15, 5, Img("photo-1606755962773-d324e0a13086", 400, 300)));

        // Sides
        items.Add(Item(r4Seq.Next(), Cat4Sides, Restaurant4Id, "French Fries",          "Golden crispy fries with ketchup",                   9900, true,  true,  10, 1, Img("photo-1573080496219-bb080dd4f877", 400, 300)));
        items.Add(Item(r4Seq.Next(), Cat4Sides, Restaurant4Id, "Onion Rings",           "Beer-battered onion rings with ranch dip",           12900, true,  false, 12, 2, Img("photo-1639024471283-03518883512d", 400, 300)));
        items.Add(Item(r4Seq.Next(), Cat4Sides, Restaurant4Id, "Chicken Wings (6 pcs)", "Tossed in your choice of buffalo or garlic parmesan",18900, false, true,  15, 3, Img("photo-1527477396000-e27163b481c2", 400, 300)));
        items.Add(Item(r4Seq.Next(), Cat4Sides, Restaurant4Id, "Coleslaw",              "Creamy cabbage and carrot slaw",                      6900, true,  false, 5,  4, Img("photo-1625938145744-e380515399bf", 400, 300)));

        // Beverages
        items.Add(Item(r4Seq.Next(), Cat4Beverages, Restaurant4Id, "Chocolate Milkshake", "Thick shake with Belgian chocolate and cream",      14900, true,  true,  8,  1, Img("photo-1572490122747-3968b75cc699", 400, 300)));
        items.Add(Item(r4Seq.Next(), Cat4Beverages, Restaurant4Id, "Oreo Shake",          "Blended Oreo cookies with vanilla ice cream",       16900, true,  false, 8,  2, Img("photo-1577805947697-89e18249d767", 400, 300)));
        items.Add(Item(r4Seq.Next(), Cat4Beverages, Restaurant4Id, "Fresh Lime Soda",     "Freshly squeezed lime with soda water",              7900, true,  false, 5,  3, Img("photo-1513558161293-cdaf765ed2fd", 400, 300)));

        // Variants for Classic Smash Burger
        var vSeq4 = new GuidSequence("41000000-0000-0000-0000-");
        variants.Add(Variant(vSeq4.Next(), classicBurgerId, "Single Patty", 0,     true,  1));
        variants.Add(Variant(vSeq4.Next(), classicBurgerId, "Double Patty", 7000,  false, 2)); // +INR 70
        variants.Add(Variant(vSeq4.Next(), classicBurgerId, "Triple Patty", 14000, false, 3)); // +INR 140

        // Addons for Classic Smash Burger
        var aSeq4 = new GuidSequence("42000000-0000-0000-0000-");
        addons.Add(Addon(aSeq4.Next(), classicBurgerId, "Extra Cheese Slice",  3900, true,  1));
        addons.Add(Addon(aSeq4.Next(), classicBurgerId, "Bacon Strip",         5900, false, 2));
        addons.Add(Addon(aSeq4.Next(), classicBurgerId, "Jalapenos",           2900, true,  3));
        addons.Add(Addon(aSeq4.Next(), classicBurgerId, "Fried Egg",           3900, false, 4));

        // ---------- Restaurant 5: Green Bowl ----------
        var r5Seq = new GuidSequence("50000000-0000-0000-0000-");

        // Salads
        items.Add(Item(r5Seq.Next(), Cat5Salads, Restaurant5Id, "Caesar Salad",           "Romaine, croutons, parmesan with creamy Caesar dressing", 17900, true,  true,  10, 1, Img("photo-1550304943-4f24f54ddde9", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Salads, Restaurant5Id, "Greek Salad",            "Cucumber, tomato, olives, feta with herb vinaigrette",    16900, true,  false, 10, 2, Img("photo-1540420773420-3366772f4999", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Salads, Restaurant5Id, "Quinoa Tabbouleh",       "Quinoa with parsley, mint, tomato, and lemon dressing",   18900, true,  false, 10, 3, Img("photo-1505576399279-0d754c0d8c53", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Salads, Restaurant5Id, "Asian Sesame Salad",     "Mixed greens with edamame, tofu, and sesame ginger dressing", 19900, true, false, 10, 4, Img("photo-1512621776951-a57141f2eefd", 400, 300)));

        // Bowls
        var buddhaId = r5Seq.Next();
        items.Add(Item(buddhaId,     Cat5Bowls, Restaurant5Id, "Buddha Bowl",             "Brown rice, roasted vegetables, hummus, and tahini drizzle", 22900, true,  true,  15, 1, Img("photo-1512621776951-a57141f2eefd", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Bowls, Restaurant5Id, "Mexican Burrito Bowl",    "Brown rice, black beans, corn salsa, guacamole, and lime crema", 24900, true, true, 15, 2, Img("photo-1543339308-d595a4e2f5e9", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Bowls, Restaurant5Id, "Teriyaki Tofu Bowl",      "Soba noodles, teriyaki-glazed tofu, steamed broccoli",    21900, true,  false, 18, 3, Img("photo-1546069901-ba9599a7e63c", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Bowls, Restaurant5Id, "Mediterranean Bowl",      "Falafel, hummus, tabbouleh, pickled turnip, pita chips",  23900, true,  false, 15, 4, Img("photo-1540914124281-342587941389", 400, 300)));

        // Smoothies
        items.Add(Item(r5Seq.Next(), Cat5Smoothies, Restaurant5Id, "Green Detox",          "Spinach, apple, ginger, lemon, and celery",           14900, true,  true,  5,  1, Img("photo-1610970881699-44a5587cabec", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Smoothies, Restaurant5Id, "Berry Blast",          "Mixed berries, banana, and almond milk",              15900, true,  true,  5,  2, Img("photo-1553530666-ba11a7da3888", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Smoothies, Restaurant5Id, "Tropical Mango",      "Alphonso mango, pineapple, and coconut water",        13900, true,  false, 5,  3, Img("photo-1623065422902-30a2d299bbe4", 400, 300)));
        items.Add(Item(r5Seq.Next(), Cat5Smoothies, Restaurant5Id, "Peanut Butter Banana", "Banana, peanut butter, oat milk, and cocoa nibs",    16900, true,  false, 5,  4, Img("photo-1577805947697-89e18249d767", 400, 300)));

        // Variants for Buddha Bowl
        var vSeq5 = new GuidSequence("51000000-0000-0000-0000-");
        variants.Add(Variant(vSeq5.Next(), buddhaId, "Regular",     0,     true,  1));
        variants.Add(Variant(vSeq5.Next(), buddhaId, "Large",       6000,  false, 2)); // +INR 60
        variants.Add(Variant(vSeq5.Next(), buddhaId, "Extra Protein", 9000, false, 3)); // +INR 90

        // Addons for Buddha Bowl
        var aSeq5 = new GuidSequence("52000000-0000-0000-0000-");
        addons.Add(Addon(aSeq5.Next(), buddhaId, "Avocado",         4900, true, 1));
        addons.Add(Addon(aSeq5.Next(), buddhaId, "Extra Hummus",    2900, true, 2));
        addons.Add(Addon(aSeq5.Next(), buddhaId, "Grilled Paneer",  5900, true, 3));

        context.MenuItems.AddRange(items);
        context.MenuItemVariants.AddRange(variants);
        context.MenuItemAddons.AddRange(addons);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Batch 2: Additional restaurants (Taco Loco, Sushi Sen, Dosa Factory)
    //  Called both from the full-seed path and the incremental path.
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedNewRestaurantsBatch2(AppDbContext context)
    {
        // ── Owners ───────────────────────────────────────────────────────
        context.Users.AddRange(
            CreateOwner(Owner6Id, "+919800000006", "owner6@swiggyclone.com", "Sofia Martinez"),
            CreateOwner(Owner7Id, "+919800000007", "owner7@swiggyclone.com", "Yuki Tanaka"),
            CreateOwner(Owner8Id, "+919800000008", "owner8@swiggyclone.com", "Lakshmi Iyer")
        );

        // ── Restaurants ──────────────────────────────────────────────────
        context.Restaurants.AddRange(
            new Restaurant
            {
                Id                 = Restaurant6Id,
                OwnerId            = Owner6Id,
                Name               = "Taco Loco",
                Slug               = "taco-loco",
                Description        = "Vibrant Mexican street food with authentic tacos, burritos, and fresh salsas made daily.",
                PhoneNumber        = "+918000000006",
                Email              = "contact@tacoloco.in",
                LogoUrl            = Img("photo-1565299585323-38d6b0865b47", 200, 200),
                BannerUrl          = Img("photo-1613514785940-daed07799d9b", 800, 400),
                AddressLine1       = "12, Cathedral Road",
                AddressLine2       = "Near Gemini Flyover",
                City               = "Chennai",
                State              = "Tamil Nadu",
                PostalCode         = "600086",
                Latitude           = 13.0827,
                Longitude          = 80.2707,
                AverageRating      = 4.2m,
                TotalRatings       = 650,
                AvgDeliveryTimeMin = 30,
                AvgCostForTwo      = 40000, // INR 400.00
                IsVegOnly          = false,
                IsAcceptingOrders  = true,
                IsDineInEnabled    = false,
                CommissionRate     = 14.50m,
                FssaiLicense       = "11525999000987",
                GstNumber          = "33AABCU9603R1ZT",
                Status             = RestaurantStatus.Approved,
                CreatedAt          = SeedTimestamp,
                UpdatedAt          = SeedTimestamp,
            },
            new Restaurant
            {
                Id                 = Restaurant7Id,
                OwnerId            = Owner7Id,
                Name               = "Sushi Sen",
                Slug               = "sushi-sen",
                Description        = "Premium Japanese dining with fresh sushi, sashimi, and traditional bento boxes crafted by master chefs.",
                PhoneNumber        = "+918000000007",
                Email              = "contact@sushisen.in",
                LogoUrl            = Img("photo-1579871494447-9811cf80d66c", 200, 200),
                BannerUrl          = Img("photo-1553621042-f6e147245754", 800, 400),
                AddressLine1       = "8, Park Street",
                AddressLine2       = "Near Flurys",
                City               = "Kolkata",
                State              = "West Bengal",
                PostalCode         = "700016",
                Latitude           = 22.5726,
                Longitude          = 88.3639,
                AverageRating      = 4.7m,
                TotalRatings       = 520,
                AvgDeliveryTimeMin = 35,
                AvgCostForTwo      = 90000, // INR 900.00
                IsVegOnly          = false,
                IsAcceptingOrders  = true,
                IsDineInEnabled    = true,
                CommissionRate     = 16.00m,
                FssaiLicense       = "11526999000456",
                GstNumber          = "19AABCU9603R1ZW",
                Status             = RestaurantStatus.Approved,
                CreatedAt          = SeedTimestamp,
                UpdatedAt          = SeedTimestamp,
            },
            new Restaurant
            {
                Id                 = Restaurant8Id,
                OwnerId            = Owner8Id,
                Name               = "Dosa Factory",
                Slug               = "dosa-factory",
                Description        = "Authentic South Indian breakfast and snacks — crispy dosas, fluffy idlis, and piping hot filter coffee.",
                PhoneNumber        = "+918000000008",
                Email              = "contact@dosafactory.in",
                LogoUrl            = Img("photo-1630383249896-424e482df921", 200, 200),
                BannerUrl          = Img("photo-1589301760014-d929f3979dbc", 800, 400),
                AddressLine1       = "56, 4th Block, Jayanagar",
                AddressLine2       = "Near Cool Joint",
                City               = "Bangalore",
                State              = "Karnataka",
                PostalCode         = "560011",
                Latitude           = 12.9352,
                Longitude          = 77.6245,
                AverageRating      = 4.4m,
                TotalRatings       = 1850,
                AvgDeliveryTimeMin = 20,
                AvgCostForTwo      = 25000, // INR 250.00
                IsVegOnly          = true,
                IsAcceptingOrders  = true,
                IsDineInEnabled    = true,
                CommissionRate     = 12.00m,
                FssaiLicense       = "11527999000789",
                GstNumber          = "29AABCU9603R2ZK",
                Status             = RestaurantStatus.Approved,
                CreatedAt          = SeedTimestamp,
                UpdatedAt          = SeedTimestamp,
            }
        );

        // ── Cuisine junctions ────────────────────────────────────────────
        context.RestaurantCuisines.AddRange(
            // Taco Loco: Mexican, Street Food
            new RestaurantCuisine { RestaurantId = Restaurant6Id, CuisineId = CuisineMexicanId },
            new RestaurantCuisine { RestaurantId = Restaurant6Id, CuisineId = CuisineStreetFoodId },
            // Sushi Sen: Japanese
            new RestaurantCuisine { RestaurantId = Restaurant7Id, CuisineId = CuisineJapaneseId },
            // Dosa Factory: South Indian, Street Food
            new RestaurantCuisine { RestaurantId = Restaurant8Id, CuisineId = CuisineSouthIndianId },
            new RestaurantCuisine { RestaurantId = Restaurant8Id, CuisineId = CuisineStreetFoodId }
        );

        // ── Operating hours (all open 09:00–23:00, 7 days) ──────────────
        var openTime  = new TimeOnly(9, 0);
        var closeTime = new TimeOnly(23, 0);
        var hours = new List<RestaurantOperatingHours>();

        var newHoursGuidBase = new[]
        {
            (RestId: Restaurant6Id, Prefix: "e6000000-0000-0000-0000-00000000000"),
            (RestId: Restaurant7Id, Prefix: "e7000000-0000-0000-0000-00000000000"),
            (RestId: Restaurant8Id, Prefix: "e8000000-0000-0000-0000-00000000000"),
        };

        foreach (var (restId, prefix) in newHoursGuidBase)
        {
            for (short day = 0; day <= 6; day++)
            {
                hours.Add(new RestaurantOperatingHours
                {
                    Id           = Guid.Parse($"{prefix}{day:x1}"),
                    RestaurantId = restId,
                    DayOfWeek    = day,
                    OpenTime     = openTime,
                    CloseTime    = closeTime,
                    IsClosed     = false,
                });
            }
        }

        context.RestaurantOperatingHours.AddRange(hours);

        // ── Tables (dine-in: Sushi Sen, Dosa Factory) ────────────────────
        var tables = new List<RestaurantTable>();
        AddTables(tables, Restaurant7Id, "f7000000-0000-0000-0000-00000000000", 3, "Main Hall");
        AddTables(tables, Restaurant8Id, "f8000000-0000-0000-0000-00000000000", 4, "Ground Floor");
        context.RestaurantTables.AddRange(tables);

        // ── Categories ───────────────────────────────────────────────────
        context.MenuCategories.AddRange(
            // Restaurant 6 — Taco Loco
            Cat(Cat6Tacos,     Restaurant6Id, "Tacos",                  "Freshly made tacos with house salsas",         1),
            Cat(Cat6Burritos,  Restaurant6Id, "Burritos & Quesadillas", "Loaded burritos and cheesy quesadillas",       2),
            Cat(Cat6Sides,     Restaurant6Id, "Sides & Drinks",         "Shareable sides and refreshing drinks",        3),

            // Restaurant 7 — Sushi Sen
            Cat(Cat7Sushi,     Restaurant7Id, "Sushi & Rolls",          "Fresh sushi and signature maki rolls",         1),
            Cat(Cat7HotDishes, Restaurant7Id, "Hot Dishes",             "Authentic Japanese hot entrees",               2),
            Cat(Cat7Bento,     Restaurant7Id, "Bento Boxes",            "Complete Japanese meal boxes",                 3),

            // Restaurant 8 — Dosa Factory
            Cat(Cat8Dosas,     Restaurant8Id, "Dosas",                  "Crispy and soft dosas with variety of fillings", 1),
            Cat(Cat8IdliVada,  Restaurant8Id, "Idli & Vada",            "Steamed idlis and crispy vadas",                2),
            Cat(Cat8Beverages, Restaurant8Id, "South Indian Beverages", "Traditional drinks and lassis",                 3)
        );

        // ── Menu Items ───────────────────────────────────────────────────
        var items    = new List<MenuItem>();
        var variants = new List<MenuItemVariant>();
        var addons   = new List<MenuItemAddon>();

        // ---------- Restaurant 6: Taco Loco ----------
        var r6Seq = new GuidSequence("60000000-0000-0000-0000-");

        // Tacos
        var chickenTacoId = r6Seq.Next();
        items.Add(Item(chickenTacoId, Cat6Tacos, Restaurant6Id, "Classic Chicken Taco",      "Grilled chicken, pico de gallo, cilantro-lime crema",         12900, false, true,  12, 1, Img("photo-1565299585323-38d6b0865b47", 400, 300)));
        items.Add(Item(r6Seq.Next(),  Cat6Tacos, Restaurant6Id, "Al Pastor Taco",            "Marinated pork with caramelized pineapple and onion",         14900, false, false, 12, 2, Img("photo-1551504734-5ee1c4a1479b", 400, 300)));
        items.Add(Item(r6Seq.Next(),  Cat6Tacos, Restaurant6Id, "Paneer Tikka Taco",         "Indian-Mexican fusion with spiced paneer and mint chutney",   13900, true,  true,  12, 3, Img("photo-1624300629298-e9209b7a8a52", 400, 300)));
        items.Add(Item(r6Seq.Next(),  Cat6Tacos, Restaurant6Id, "Fish Taco",                 "Beer-battered fish with chipotle mayo and cabbage slaw",      15900, false, false, 15, 4, Img("photo-1512838243191-e81e8f66f1fd", 400, 300)));

        // Burritos & Quesadillas
        var chickenBurritoId = r6Seq.Next();
        items.Add(Item(chickenBurritoId, Cat6Burritos, Restaurant6Id, "Chicken Burrito",         "Grilled chicken, Mexican rice, beans, cheese, and salsa",     19900, false, true,  15, 1, Img("photo-1626700051175-6818013e1d4f", 400, 300)));
        items.Add(Item(r6Seq.Next(),     Cat6Burritos, Restaurant6Id, "Bean & Cheese Quesadilla","Refried beans and melted cheese in a crispy flour tortilla",   14900, true,  false, 10, 2, Img("photo-1618040996337-56904b7850b9", 400, 300)));
        items.Add(Item(r6Seq.Next(),     Cat6Burritos, Restaurant6Id, "Veggie Burrito",          "Rice, black beans, corn salsa, guacamole, and sour cream",    17900, true,  false, 15, 3, Img("photo-1584208632869-05fa2b2a5934", 400, 300)));

        // Sides & Drinks
        items.Add(Item(r6Seq.Next(), Cat6Sides, Restaurant6Id, "Nachos Supreme",             "Tortilla chips loaded with cheese, jalapenos, and sour cream", 13900, true,  true,  10, 1, Img("photo-1513456852971-30c0b8199d4d", 400, 300)));
        items.Add(Item(r6Seq.Next(), Cat6Sides, Restaurant6Id, "Churros (4 pcs)",            "Cinnamon sugar churros with warm chocolate dip",               9900,  true,  false, 8,  2, Img("photo-1624371414361-081b3fa4e9bc", 400, 300)));
        items.Add(Item(r6Seq.Next(), Cat6Sides, Restaurant6Id, "Horchata",                   "Creamy cinnamon rice drink served chilled",                    8900,  true,  false, 5,  3, Img("photo-1544145945-f90425340c7e", 400, 300)));

        // Variants for Chicken Burrito
        var vSeq6 = new GuidSequence("61000000-0000-0000-0000-");
        variants.Add(Variant(vSeq6.Next(), chickenBurritoId, "Regular",        0,    true,  1));
        variants.Add(Variant(vSeq6.Next(), chickenBurritoId, "Large",          6000, false, 2)); // +INR 60
        variants.Add(Variant(vSeq6.Next(), chickenBurritoId, "Bowl (no wrap)", 0,    false, 3));

        // Addons for Classic Chicken Taco
        var aSeq6 = new GuidSequence("62000000-0000-0000-0000-");
        addons.Add(Addon(aSeq6.Next(), chickenTacoId, "Extra Guacamole", 4900, true, 1));
        addons.Add(Addon(aSeq6.Next(), chickenTacoId, "Sour Cream",     2900, true, 2));
        addons.Add(Addon(aSeq6.Next(), chickenTacoId, "Extra Cheese",   3900, true, 3));

        // ---------- Restaurant 7: Sushi Sen ----------
        var r7Seq = new GuidSequence("70000000-0000-0000-0000-");

        // Sushi & Rolls
        var californiaRollId = r7Seq.Next();
        items.Add(Item(californiaRollId, Cat7Sushi, Restaurant7Id, "California Roll (8 pcs)",  "Crab, avocado, cucumber with toasted sesame",                 22900, false, true,  20, 1, Img("photo-1579584425555-c3ce17fd4351", 400, 300)));
        items.Add(Item(r7Seq.Next(),     Cat7Sushi, Restaurant7Id, "Salmon Nigiri (4 pcs)",    "Fresh Atlantic salmon over seasoned sushi rice",              29900, false, true,  15, 2, Img("photo-1553621042-f6e147245754", 400, 300)));
        items.Add(Item(r7Seq.Next(),     Cat7Sushi, Restaurant7Id, "Veg Tempura Roll (8 pcs)", "Crispy tempura vegetables with spicy mayo",                   19900, true,  false, 20, 3, Img("photo-1617196034183-421b4917c92d", 400, 300)));
        items.Add(Item(r7Seq.Next(),     Cat7Sushi, Restaurant7Id, "Spicy Tuna Roll (8 pcs)",  "Tuna, sriracha mayo, cucumber, and tobiko",                   27900, false, false, 20, 4, Img("photo-1611143669185-af224c5e3252", 400, 300)));

        // Hot Dishes
        var katsuCurryId = r7Seq.Next();
        items.Add(Item(katsuCurryId, Cat7HotDishes, Restaurant7Id, "Chicken Katsu Curry",       "Crispy chicken cutlet with Japanese curry and steamed rice",  26900, false, true,  25, 1, Img("photo-1604908176997-125f25cc6f3d", 400, 300)));
        items.Add(Item(r7Seq.Next(), Cat7HotDishes, Restaurant7Id, "Miso Ramen",                "Rich miso broth with chashu pork, noodles, and soft egg",    24900, false, false, 25, 2, Img("photo-1557872943-16a5ac26437e", 400, 300)));
        items.Add(Item(r7Seq.Next(), Cat7HotDishes, Restaurant7Id, "Vegetable Tempura Platter", "Assorted seasonal vegetables in light crispy batter",        18900, true,  false, 15, 3, Img("photo-1615361200141-f45040f367be", 400, 300)));

        // Bento Boxes
        items.Add(Item(r7Seq.Next(), Cat7Bento, Restaurant7Id, "Teriyaki Chicken Bento", "Grilled chicken, rice, salad, miso soup, and pickles",           32900, false, true,  20, 1, Img("photo-1569050467447-ce54b3bbc37d", 400, 300)));
        items.Add(Item(r7Seq.Next(), Cat7Bento, Restaurant7Id, "Salmon Bento",           "Grilled salmon, rice, edamame, gyoza, and seasonal fruit",       37900, false, false, 20, 2, Img("photo-1580822184713-fc5400e7fe10", 400, 300)));
        items.Add(Item(r7Seq.Next(), Cat7Bento, Restaurant7Id, "Tofu Bento",             "Teriyaki tofu, rice, salad, miso soup, and pickles",             27900, true,  false, 20, 3, Img("photo-1546069901-ba9599a7e63c", 400, 300)));

        // Variants for California Roll
        var vSeq7 = new GuidSequence("71000000-0000-0000-0000-");
        variants.Add(Variant(vSeq7.Next(), californiaRollId, "8 pcs",  0,     true,  1));
        variants.Add(Variant(vSeq7.Next(), californiaRollId, "12 pcs", 8000,  false, 2)); // +INR 80
        variants.Add(Variant(vSeq7.Next(), californiaRollId, "16 pcs", 15000, false, 3)); // +INR 150

        // Addons for Chicken Katsu Curry
        var aSeq7 = new GuidSequence("72000000-0000-0000-0000-");
        addons.Add(Addon(aSeq7.Next(), katsuCurryId, "Extra Rice",     3900, true, 1));
        addons.Add(Addon(aSeq7.Next(), katsuCurryId, "Pickled Ginger", 1900, true, 2));
        addons.Add(Addon(aSeq7.Next(), katsuCurryId, "Miso Soup",     4900, true, 3));

        // ---------- Restaurant 8: Dosa Factory ----------
        var r8Seq = new GuidSequence("80000000-0000-0000-0000-");

        // Dosas
        var masalaDosaId = r8Seq.Next();
        items.Add(Item(masalaDosaId, Cat8Dosas, Restaurant8Id, "Masala Dosa",        "Crispy golden dosa with spiced potato filling",             8900,  true, true,  12, 1, Img("photo-1630383249896-424e482df921", 400, 300)));
        items.Add(Item(r8Seq.Next(), Cat8Dosas, Restaurant8Id, "Mysore Masala Dosa", "Dosa with red chutney spread and potato masala",            10900, true, true,  15, 2, Img("photo-1589301760014-d929f3979dbc", 400, 300)));
        items.Add(Item(r8Seq.Next(), Cat8Dosas, Restaurant8Id, "Rava Dosa",          "Crispy semolina dosa with onion and green chili",           9900,  true, false, 12, 3, Img("photo-1668236543090-82eba5ee5976", 400, 300)));
        items.Add(Item(r8Seq.Next(), Cat8Dosas, Restaurant8Id, "Paneer Dosa",        "Dosa stuffed with spiced cottage cheese and capsicum",      12900, true, false, 15, 4, Img("photo-1645177628172-a94c1f96e6db", 400, 300)));
        items.Add(Item(r8Seq.Next(), Cat8Dosas, Restaurant8Id, "Ghee Roast Dosa",    "Thin crispy dosa roasted in pure ghee",                     7900,  true, false, 10, 5, Img("photo-1610192244261-3f33de3f55e4", 400, 300)));

        // Idli & Vada
        items.Add(Item(r8Seq.Next(), Cat8IdliVada, Restaurant8Id, "Idli (3 pcs)",      "Steamed rice cakes served with sambar and coconut chutney", 5900, true, true,  10, 1, Img("photo-1589301760014-d929f3979dbc", 400, 300)));
        items.Add(Item(r8Seq.Next(), Cat8IdliVada, Restaurant8Id, "Medu Vada (2 pcs)", "Crispy lentil donuts with sambar and chutney",              6900, true, false, 10, 2, Img("photo-1606491956689-2ea866880049", 400, 300)));
        items.Add(Item(r8Seq.Next(), Cat8IdliVada, Restaurant8Id, "Idli Vada Combo",   "2 idlis and 1 medu vada with sambar and chutney",           8900, true, true,  12, 3, Img("photo-1630383249896-424e482df921", 400, 300)));

        // South Indian Beverages
        items.Add(Item(r8Seq.Next(), Cat8Beverages, Restaurant8Id, "Filter Coffee", "Traditional South Indian decoction filter coffee",           4900, true, true,  5, 1, Img("photo-1442512595331-e89e73853f31", 400, 300)));
        items.Add(Item(r8Seq.Next(), Cat8Beverages, Restaurant8Id, "Mango Lassi",   "Thick creamy yogurt blended with Alphonso mango",            7900, true, false, 5, 2, Img("photo-1623065422902-30a2d299bbe4", 400, 300)));
        items.Add(Item(r8Seq.Next(), Cat8Beverages, Restaurant8Id, "Buttermilk",    "Spiced and salted traditional South Indian buttermilk",      3900, true, false, 5, 3, Img("photo-1544145945-f90425340c7e", 400, 300)));

        // Variants for Masala Dosa
        var vSeq8 = new GuidSequence("81000000-0000-0000-0000-");
        variants.Add(Variant(vSeq8.Next(), masalaDosaId, "Regular",        0,     true,  1));
        variants.Add(Variant(vSeq8.Next(), masalaDosaId, "Set Dosa (2)",   5000,  false, 2)); // +INR 50
        variants.Add(Variant(vSeq8.Next(), masalaDosaId, "Family Pack (4)", 15000, false, 3)); // +INR 150

        // Addons for Masala Dosa
        var aSeq8 = new GuidSequence("82000000-0000-0000-0000-");
        addons.Add(Addon(aSeq8.Next(), masalaDosaId, "Extra Sambar",         2900, true, 1));
        addons.Add(Addon(aSeq8.Next(), masalaDosaId, "Extra Coconut Chutney", 1900, true, 2));
        addons.Add(Addon(aSeq8.Next(), masalaDosaId, "Extra Potato Filling", 3900, true, 3));

        context.MenuItems.AddRange(items);
        context.MenuItemVariants.AddRange(variants);
        context.MenuItemAddons.AddRange(addons);
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Factory helpers — keep the seed code concise and uniform.
    // ────────────────────────────────────────────────────────────────────────

    private static MenuCategory Cat(Guid id, Guid restaurantId, string name, string description, int sortOrder)
    {
        return new MenuCategory
        {
            Id           = id,
            RestaurantId = restaurantId,
            Name         = name,
            Description  = description,
            SortOrder    = sortOrder,
            IsActive     = true,
            CreatedAt    = SeedTimestamp,
            UpdatedAt    = SeedTimestamp,
        };
    }

    private static MenuItem Item(
        Guid id,
        Guid categoryId,
        Guid restaurantId,
        string name,
        string description,
        int price,
        bool isVeg,
        bool isBestseller,
        int prepTimeMin,
        int sortOrder,
        string? imageUrl = null)
    {
        return new MenuItem
        {
            Id                 = id,
            CategoryId         = categoryId,
            RestaurantId       = restaurantId,
            Name               = name,
            Description        = description,
            Price              = price,
            IsVeg              = isVeg,
            IsBestseller       = isBestseller,
            IsAvailable        = true,
            PreparationTimeMin = prepTimeMin,
            SortOrder          = sortOrder,
            ImageUrl           = imageUrl,
            CreatedAt          = SeedTimestamp,
            UpdatedAt          = SeedTimestamp,
        };
    }

    private static MenuItemVariant Variant(
        Guid id,
        Guid menuItemId,
        string name,
        int priceAdjustment,
        bool isDefault,
        int sortOrder)
    {
        return new MenuItemVariant
        {
            Id              = id,
            MenuItemId      = menuItemId,
            Name            = name,
            PriceAdjustment = priceAdjustment,
            IsDefault       = isDefault,
            IsAvailable     = true,
            SortOrder       = sortOrder,
            CreatedAt       = SeedTimestamp,
        };
    }

    private static MenuItemAddon Addon(
        Guid id,
        Guid menuItemId,
        string name,
        int price,
        bool isVeg,
        int sortOrder)
    {
        return new MenuItemAddon
        {
            Id          = id,
            MenuItemId  = menuItemId,
            Name        = name,
            Price       = price,
            IsVeg       = isVeg,
            IsAvailable = true,
            MaxQuantity = 5,
            SortOrder   = sortOrder,
            CreatedAt   = SeedTimestamp,
        };
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Deterministic GUID generator — produces sequential, fixed GUIDs
    //  by appending a zero-padded hex counter to a supplied prefix.
    // ────────────────────────────────────────────────────────────────────────

    private sealed class GuidSequence
    {
        private readonly string _prefix;
        private int _counter;

        /// <summary>
        /// Creates a new sequence. The prefix must be exactly 24 hex characters
        /// (including hyphens that are part of the GUID format), leaving 12 hex
        /// characters for the counter suffix.
        /// Example prefix: "10000000-0000-0000-0000-"
        /// </summary>
        public GuidSequence(string prefix)
        {
            _prefix = prefix;
        }

        public Guid Next()
        {
            _counter++;
            return Guid.Parse($"{_prefix}{_counter:x12}");
        }
    }
}
