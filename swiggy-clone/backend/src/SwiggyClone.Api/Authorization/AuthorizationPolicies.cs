using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Authorization;

public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string RestaurantOwner = "RestaurantOwner";
    public const string DeliveryPartner = "DeliveryPartner";
    public const string CustomerOnly = "CustomerOnly";

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AdminOnly, policy =>
                policy.RequireRole(UserRole.Admin.ToString()))
            .AddPolicy(RestaurantOwner, policy =>
                policy.RequireRole(UserRole.RestaurantOwner.ToString()))
            .AddPolicy(DeliveryPartner, policy =>
                policy.RequireRole(UserRole.DeliveryPartner.ToString()))
            .AddPolicy(CustomerOnly, policy =>
                policy.RequireRole(UserRole.Customer.ToString()));

        return services;
    }
}
