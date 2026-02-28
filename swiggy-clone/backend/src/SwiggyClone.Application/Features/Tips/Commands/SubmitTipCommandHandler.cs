using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Tips.Commands;

internal sealed class SubmitTipCommandHandler(IAppDbContext db, ISender sender, IEventBus eventBus)
    : IRequestHandler<SubmitTipCommand, Result>
{
    public async Task<Result> Handle(SubmitTipCommand command, CancellationToken ct)
    {
        var order = await db.Orders
            .FirstOrDefaultAsync(o => o.Id == command.OrderId && o.UserId == command.UserId, ct);

        if (order is null)
            return Result.Failure("ORDER_NOT_FOUND", "Order not found.");

        if (order.Status != OrderStatus.Delivered)
            return Result.Failure("ORDER_NOT_DELIVERED", "Tips can only be given for delivered orders.");

        if (order.OrderType != OrderType.Delivery)
            return Result.Failure("NOT_DELIVERY_ORDER", "Tips are only available for delivery orders.");

        if (order.TipAmount > 0)
            return Result.Failure("ALREADY_TIPPED", "A tip has already been submitted for this order.");

        var assignment = await db.DeliveryAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.OrderId == order.Id && a.Status == DeliveryStatus.Delivered, ct);

        if (assignment is null)
            return Result.Failure("ASSIGNMENT_NOT_FOUND", "Delivery assignment not found.");

        order.TipAmount = command.AmountPaise;
        order.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        await sender.Send(new CreditWalletCommand(
            assignment.PartnerId,
            command.AmountPaise,
            (short)WalletTransactionSource.Tip,
            order.Id,
            $"Tip for order #{order.OrderNumber}"), ct);

        await eventBus.PublishAsync(
            KafkaTopics.NotificationSend,
            assignment.PartnerId.ToString(),
            new
            {
                UserId = assignment.PartnerId,
                Title = "Tip Received!",
                Body = $"You received a \u20B9{command.AmountPaise / 100} tip for order #{order.OrderNumber}"
            },
            ct);

        return Result.Success();
    }
}
