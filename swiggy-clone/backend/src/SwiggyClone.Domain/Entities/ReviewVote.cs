namespace SwiggyClone.Domain.Entities;

public sealed class ReviewVote
{
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public bool IsHelpful { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation
    public Review Review { get; set; } = null!;
    public User User { get; set; } = null!;
}
