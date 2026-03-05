using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SwiggyClone.Api.Hubs;

[Authorize]
public sealed class GroupOrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        await base.OnConnectedAsync();
    }

    public async Task JoinGroupOrder(Guid groupOrderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"group-order-{groupOrderId}");
    }

    public async Task LeaveGroupOrder(Guid groupOrderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group-order-{groupOrderId}");
    }
}
