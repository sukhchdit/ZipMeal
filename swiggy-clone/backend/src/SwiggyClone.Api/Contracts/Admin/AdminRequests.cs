namespace SwiggyClone.Api.Contracts.Admin;

public sealed record ToggleUserActiveRequest(bool IsActive);

public sealed record ChangeUserRoleRequest(short NewRole);

public sealed record RejectRestaurantRequest(string Reason);

public sealed record SuspendRestaurantRequest(string Reason);
