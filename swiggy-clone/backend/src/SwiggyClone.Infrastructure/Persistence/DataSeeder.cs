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
            return;
        }

        SeedCuisineTypes(context);
        SeedUsers(context);
        SeedRestaurants(context);
        SeedRestaurantCuisines(context);
        SeedOperatingHours(context);
        SeedTables(context);
        SeedMenuCategoriesAndItems(context);

        await context.SaveChangesAsync();
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Cuisine Types
    // ────────────────────────────────────────────────────────────────────────

    private static void SeedCuisineTypes(AppDbContext context)
    {
        CuisineType[] cuisines =
        [
            new() { Id = CuisineNorthIndianId, Name = "North Indian",  SortOrder = 1,  IsActive = true },
            new() { Id = CuisineSouthIndianId, Name = "South Indian",  SortOrder = 2,  IsActive = true },
            new() { Id = CuisineChineseId,     Name = "Chinese",       SortOrder = 3,  IsActive = true },
            new() { Id = CuisineItalianId,     Name = "Italian",       SortOrder = 4,  IsActive = true },
            new() { Id = CuisineMexicanId,     Name = "Mexican",       SortOrder = 5,  IsActive = true },
            new() { Id = CuisineJapaneseId,    Name = "Japanese",      SortOrder = 6,  IsActive = true },
            new() { Id = CuisineThaiId,        Name = "Thai",          SortOrder = 7,  IsActive = true },
            new() { Id = CuisineContinentalId, Name = "Continental",   SortOrder = 8,  IsActive = true },
            new() { Id = CuisineBiryaniId,     Name = "Biryani",       SortOrder = 9,  IsActive = true },
            new() { Id = CuisinePizzaId,       Name = "Pizza",         SortOrder = 10, IsActive = true },
            new() { Id = CuisineBurgerId,      Name = "Burger",        SortOrder = 11, IsActive = true },
            new() { Id = CuisineDessertsId,    Name = "Desserts",      SortOrder = 12, IsActive = true },
            new() { Id = CuisineBeveragesId,   Name = "Beverages",     SortOrder = 13, IsActive = true },
            new() { Id = CuisineStreetFoodId,  Name = "Street Food",   SortOrder = 14, IsActive = true },
            new() { Id = CuisineHealthyId,     Name = "Healthy",       SortOrder = 15, IsActive = true },
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
                Id          = AdminUserId,
                PhoneNumber = "+919999999999",
                Email       = "admin@swiggyclone.com",
                FullName    = "System Admin",
                Role        = UserRole.Admin,
                IsVerified  = true,
                IsActive    = true,
                CreatedAt   = SeedTimestamp,
                UpdatedAt   = SeedTimestamp,
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
            Id          = id,
            PhoneNumber = phone,
            Email       = email,
            FullName    = name,
            Role        = UserRole.RestaurantOwner,
            IsVerified  = true,
            IsActive    = true,
            CreatedAt   = SeedTimestamp,
            UpdatedAt   = SeedTimestamp,
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
        items.Add(Item(r1Seq.Next(), Cat1Starters, Restaurant1Id, "Paneer Tikka",          "Marinated cottage cheese grilled in a tandoor",       17900, true,  true,  20, 1));
        items.Add(Item(r1Seq.Next(), Cat1Starters, Restaurant1Id, "Chicken Seekh Kebab",   "Minced chicken spiced and skewered on charcoal",      19900, false, false, 25, 2));
        items.Add(Item(r1Seq.Next(), Cat1Starters, Restaurant1Id, "Samosa (2 pcs)",        "Crispy pastry stuffed with spiced potatoes and peas", 6900,  true,  true,  10, 3));
        items.Add(Item(r1Seq.Next(), Cat1Starters, Restaurant1Id, "Dahi Puri",             "Crispy puris filled with yogurt and tangy chutney",   8900,  true,  false, 10, 4));

        // Main Course
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Butter Chicken",      "Tender chicken in rich tomato-butter gravy",          29900, false, true,  30, 1));
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Dal Makhani",         "Slow-cooked black lentils with cream and butter",     19900, true,  true,  25, 2));
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Palak Paneer",        "Cottage cheese cubes in smooth spinach gravy",        22900, true,  false, 25, 3));
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Rogan Josh",          "Kashmiri-style slow-braised lamb curry",              34900, false, false, 35, 4));
        items.Add(Item(r1Seq.Next(), Cat1MainCourse, Restaurant1Id, "Garlic Naan",         "Soft naan bread topped with garlic and coriander",    5900,  true,  true,  8,  5));

        // Biryani
        var biryaniChickenId = r1Seq.Next();
        items.Add(Item(biryaniChickenId, Cat1Biryani, Restaurant1Id, "Chicken Dum Biryani", "Aromatic basmati rice layered with spiced chicken",  25900, false, true,  40, 1));
        items.Add(Item(r1Seq.Next(),     Cat1Biryani, Restaurant1Id, "Veg Biryani",         "Fragrant rice with seasonal vegetables and saffron", 19900, true,  false, 35, 2));
        items.Add(Item(r1Seq.Next(),     Cat1Biryani, Restaurant1Id, "Mutton Biryani",      "Tender mutton pieces in slow-cooked hyderabadi rice",32900, false, false, 45, 3));

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
        items.Add(Item(margheritaId,  Cat2Pizza, Restaurant2Id, "Margherita",         "Classic tomato, mozzarella, and fresh basil",         19900, true,  true,  20, 1));
        items.Add(Item(r2Seq.Next(),  Cat2Pizza, Restaurant2Id, "Pepperoni",          "Spicy pepperoni with mozzarella on tomato base",      29900, false, true,  20, 2));
        items.Add(Item(r2Seq.Next(),  Cat2Pizza, Restaurant2Id, "BBQ Chicken",        "Grilled chicken, BBQ sauce, red onions, mozzarella",  32900, false, false, 25, 3));
        items.Add(Item(r2Seq.Next(),  Cat2Pizza, Restaurant2Id, "Farm Fresh Veggie",  "Bell peppers, olives, mushrooms, sweet corn",         24900, true,  false, 20, 4));
        items.Add(Item(r2Seq.Next(),  Cat2Pizza, Restaurant2Id, "Truffle Mushroom",   "Wild mushrooms with truffle oil and parmesan",        37900, true,  false, 25, 5));

        // Pasta
        items.Add(Item(r2Seq.Next(), Cat2Pasta, Restaurant2Id, "Spaghetti Carbonara", "Creamy egg and parmesan sauce with crispy pancetta",  27900, false, true,  18, 1));
        items.Add(Item(r2Seq.Next(), Cat2Pasta, Restaurant2Id, "Penne Arrabbiata",    "Spicy tomato sauce with garlic and red chili flakes", 21900, true,  false, 15, 2));
        items.Add(Item(r2Seq.Next(), Cat2Pasta, Restaurant2Id, "Alfredo Pasta",       "Rich cream and parmesan sauce with fettuccine",       25900, true,  true,  18, 3));

        // Desserts
        items.Add(Item(r2Seq.Next(), Cat2Desserts, Restaurant2Id, "Tiramisu",         "Classic Italian coffee-flavored layered dessert",     19900, true,  true,  5,  1));
        items.Add(Item(r2Seq.Next(), Cat2Desserts, Restaurant2Id, "Panna Cotta",      "Silky vanilla cream topped with berry coulis",        17900, true,  false, 5,  2));
        items.Add(Item(r2Seq.Next(), Cat2Desserts, Restaurant2Id, "Chocolate Lava Cake","Warm dark chocolate cake with molten center",       21900, true,  false, 12, 3));

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
        items.Add(Item(r3Seq.Next(), Cat3Starters, Restaurant3Id, "Veg Spring Rolls (4 pcs)",   "Crispy rolls stuffed with julienned vegetables",    14900, true,  true,  12, 1));
        items.Add(Item(r3Seq.Next(), Cat3Starters, Restaurant3Id, "Chicken Dimsum (6 pcs)",     "Steamed dumplings with minced chicken filling",      17900, false, true,  15, 2));
        items.Add(Item(r3Seq.Next(), Cat3Starters, Restaurant3Id, "Tom Yum Soup",               "Spicy and sour Thai soup with mushrooms and herbs",  13900, true,  false, 10, 3));
        items.Add(Item(r3Seq.Next(), Cat3Starters, Restaurant3Id, "Edamame",                    "Steamed Japanese soybeans with sea salt",             9900, true,  false, 8,  4));

        // Main Course
        var kungPaoId = r3Seq.Next();
        items.Add(Item(kungPaoId,    Cat3MainCourse, Restaurant3Id, "Kung Pao Chicken",     "Wok-tossed chicken with peanuts and dried chili",       26900, false, true,  25, 1));
        items.Add(Item(r3Seq.Next(), Cat3MainCourse, Restaurant3Id, "Thai Green Curry",     "Creamy coconut curry with thai basil and vegetables",    24900, true,  false, 25, 2));
        items.Add(Item(r3Seq.Next(), Cat3MainCourse, Restaurant3Id, "Teriyaki Salmon",      "Grilled salmon fillet glazed with teriyaki sauce",      39900, false, false, 30, 3));
        items.Add(Item(r3Seq.Next(), Cat3MainCourse, Restaurant3Id, "Mapo Tofu",            "Silken tofu in spicy Sichuan bean sauce",               19900, true,  false, 20, 4));

        // Noodles
        items.Add(Item(r3Seq.Next(), Cat3Noodles, Restaurant3Id, "Hakka Noodles",           "Stir-fried noodles with mixed vegetables",              17900, true,  true,  15, 1));
        items.Add(Item(r3Seq.Next(), Cat3Noodles, Restaurant3Id, "Pad Thai",                "Thai rice noodles with tamarind sauce and peanuts",      21900, false, true,  20, 2));
        items.Add(Item(r3Seq.Next(), Cat3Noodles, Restaurant3Id, "Ramen",                   "Rich pork broth with wheat noodles and soft-boiled egg", 24900, false, false, 25, 3));

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
        items.Add(Item(classicBurgerId, Cat4Burgers, Restaurant4Id, "Classic Smash Burger",  "Double-smashed beef patties with American cheese",  24900, false, true,  15, 1));
        items.Add(Item(r4Seq.Next(),    Cat4Burgers, Restaurant4Id, "BBQ Bacon Burger",      "Beef patty with crispy bacon and smoky BBQ sauce",  29900, false, true,  15, 2));
        items.Add(Item(r4Seq.Next(),    Cat4Burgers, Restaurant4Id, "Mushroom Swiss Burger", "Sauteed mushrooms and Swiss cheese on beef patty",  27900, false, false, 15, 3));
        items.Add(Item(r4Seq.Next(),    Cat4Burgers, Restaurant4Id, "Veggie Crunch Burger",  "Crispy vegetable patty with lettuce and sriracha",  19900, true,  false, 12, 4));
        items.Add(Item(r4Seq.Next(),    Cat4Burgers, Restaurant4Id, "Chicken Zinger",        "Crispy fried chicken fillet with spicy mayo",       22900, false, true,  15, 5));

        // Sides
        items.Add(Item(r4Seq.Next(), Cat4Sides, Restaurant4Id, "French Fries",          "Golden crispy fries with ketchup",                   9900, true,  true,  10, 1));
        items.Add(Item(r4Seq.Next(), Cat4Sides, Restaurant4Id, "Onion Rings",           "Beer-battered onion rings with ranch dip",           12900, true,  false, 12, 2));
        items.Add(Item(r4Seq.Next(), Cat4Sides, Restaurant4Id, "Chicken Wings (6 pcs)", "Tossed in your choice of buffalo or garlic parmesan",18900, false, true,  15, 3));
        items.Add(Item(r4Seq.Next(), Cat4Sides, Restaurant4Id, "Coleslaw",              "Creamy cabbage and carrot slaw",                      6900, true,  false, 5,  4));

        // Beverages
        items.Add(Item(r4Seq.Next(), Cat4Beverages, Restaurant4Id, "Chocolate Milkshake", "Thick shake with Belgian chocolate and cream",      14900, true,  true,  8,  1));
        items.Add(Item(r4Seq.Next(), Cat4Beverages, Restaurant4Id, "Oreo Shake",          "Blended Oreo cookies with vanilla ice cream",       16900, true,  false, 8,  2));
        items.Add(Item(r4Seq.Next(), Cat4Beverages, Restaurant4Id, "Fresh Lime Soda",     "Freshly squeezed lime with soda water",              7900, true,  false, 5,  3));

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
        items.Add(Item(r5Seq.Next(), Cat5Salads, Restaurant5Id, "Caesar Salad",           "Romaine, croutons, parmesan with creamy Caesar dressing", 17900, true,  true,  10, 1));
        items.Add(Item(r5Seq.Next(), Cat5Salads, Restaurant5Id, "Greek Salad",            "Cucumber, tomato, olives, feta with herb vinaigrette",    16900, true,  false, 10, 2));
        items.Add(Item(r5Seq.Next(), Cat5Salads, Restaurant5Id, "Quinoa Tabbouleh",       "Quinoa with parsley, mint, tomato, and lemon dressing",   18900, true,  false, 10, 3));
        items.Add(Item(r5Seq.Next(), Cat5Salads, Restaurant5Id, "Asian Sesame Salad",     "Mixed greens with edamame, tofu, and sesame ginger dressing", 19900, true, false, 10, 4));

        // Bowls
        var buddhaId = r5Seq.Next();
        items.Add(Item(buddhaId,     Cat5Bowls, Restaurant5Id, "Buddha Bowl",             "Brown rice, roasted vegetables, hummus, and tahini drizzle", 22900, true,  true,  15, 1));
        items.Add(Item(r5Seq.Next(), Cat5Bowls, Restaurant5Id, "Mexican Burrito Bowl",    "Brown rice, black beans, corn salsa, guacamole, and lime crema", 24900, true, true, 15, 2));
        items.Add(Item(r5Seq.Next(), Cat5Bowls, Restaurant5Id, "Teriyaki Tofu Bowl",      "Soba noodles, teriyaki-glazed tofu, steamed broccoli",    21900, true,  false, 18, 3));
        items.Add(Item(r5Seq.Next(), Cat5Bowls, Restaurant5Id, "Mediterranean Bowl",      "Falafel, hummus, tabbouleh, pickled turnip, pita chips",  23900, true,  false, 15, 4));

        // Smoothies
        items.Add(Item(r5Seq.Next(), Cat5Smoothies, Restaurant5Id, "Green Detox",          "Spinach, apple, ginger, lemon, and celery",           14900, true,  true,  5,  1));
        items.Add(Item(r5Seq.Next(), Cat5Smoothies, Restaurant5Id, "Berry Blast",          "Mixed berries, banana, and almond milk",              15900, true,  true,  5,  2));
        items.Add(Item(r5Seq.Next(), Cat5Smoothies, Restaurant5Id, "Tropical Mango",      "Alphonso mango, pineapple, and coconut water",        13900, true,  false, 5,  3));
        items.Add(Item(r5Seq.Next(), Cat5Smoothies, Restaurant5Id, "Peanut Butter Banana", "Banana, peanut butter, oat milk, and cocoa nibs",    16900, true,  false, 5,  4));

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
        int sortOrder)
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
