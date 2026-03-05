using FluentValidation;

namespace SwiggyClone.Application.Features.Loyalty.Commands.RedeemReward;

public sealed class RedeemRewardCommandValidator : AbstractValidator<RedeemRewardCommand>
{
    public RedeemRewardCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RewardId).NotEmpty();
    }
}
