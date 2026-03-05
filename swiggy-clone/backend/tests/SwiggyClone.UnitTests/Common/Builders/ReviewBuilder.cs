using SwiggyClone.Domain.Entities;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class ReviewBuilder
{
    private Guid _id = TestConstants.ReviewId;
    private Guid _orderId = TestConstants.OrderId;
    private Guid _userId = TestConstants.UserId;
    private Guid _restaurantId = TestConstants.RestaurantId;
    private short _rating = 4;
    private string? _reviewText = "Great food!";
    private short? _deliveryRating = 5;
    private bool _isAnonymous;
    private bool _isVisible = true;
    private string? _restaurantReply;
    private DateTimeOffset? _repliedAt;
    private int _helpfulCount;
    private int _reportCount;
    private Restaurant? _restaurant;
    private User? _user;

    public ReviewBuilder WithId(Guid id) { _id = id; return this; }
    public ReviewBuilder WithOrderId(Guid orderId) { _orderId = orderId; return this; }
    public ReviewBuilder WithUserId(Guid userId) { _userId = userId; return this; }
    public ReviewBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public ReviewBuilder WithRating(short rating) { _rating = rating; return this; }
    public ReviewBuilder WithReviewText(string? text) { _reviewText = text; return this; }
    public ReviewBuilder WithDeliveryRating(short? rating) { _deliveryRating = rating; return this; }
    public ReviewBuilder WithIsAnonymous(bool anonymous) { _isAnonymous = anonymous; return this; }
    public ReviewBuilder WithIsVisible(bool visible) { _isVisible = visible; return this; }
    public ReviewBuilder WithRestaurantReply(string? reply) { _restaurantReply = reply; return this; }
    public ReviewBuilder WithRepliedAt(DateTimeOffset? repliedAt) { _repliedAt = repliedAt; return this; }
    public ReviewBuilder WithHelpfulCount(int count) { _helpfulCount = count; return this; }
    public ReviewBuilder WithReportCount(int count) { _reportCount = count; return this; }
    public ReviewBuilder WithRestaurant(Restaurant restaurant) { _restaurant = restaurant; return this; }
    public ReviewBuilder WithUser(User user) { _user = user; return this; }

    public Review Build()
    {
        var review = new Review
        {
            Id = _id,
            OrderId = _orderId,
            UserId = _userId,
            RestaurantId = _restaurantId,
            Rating = _rating,
            ReviewText = _reviewText,
            DeliveryRating = _deliveryRating,
            IsAnonymous = _isAnonymous,
            IsVisible = _isVisible,
            RestaurantReply = _restaurantReply,
            RepliedAt = _repliedAt,
            HelpfulCount = _helpfulCount,
            ReportCount = _reportCount,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        if (_restaurant is not null) review.Restaurant = _restaurant;
        if (_user is not null) review.User = _user;

        return review;
    }
}
