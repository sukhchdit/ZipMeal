using FluentAssertions;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.UnitTests.Domain;

public sealed class ReviewTests
{
    [Fact]
    public void NewReview_IsVisible_DefaultsToTrue()
    {
        var review = new Review();

        review.IsVisible.Should().BeTrue();
    }

    [Fact]
    public void NewReview_IsAnonymous_DefaultsToFalse()
    {
        var review = new Review();

        review.IsAnonymous.Should().BeFalse();
    }

    [Fact]
    public void NewReview_HelpfulCount_DefaultsToZero()
    {
        var review = new Review();

        review.HelpfulCount.Should().Be(0);
    }

    [Fact]
    public void NewReview_ReportCount_DefaultsToZero()
    {
        var review = new Review();

        review.ReportCount.Should().Be(0);
    }

    [Fact]
    public void NewReview_Collections_AreInitializedEmpty()
    {
        var review = new Review();

        review.Photos.Should().NotBeNull().And.BeEmpty();
        review.Votes.Should().NotBeNull().And.BeEmpty();
        review.Reports.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void NewReview_NullableProperties_AreNull()
    {
        var review = new Review();

        review.ReviewText.Should().BeNull();
        review.DeliveryRating.Should().BeNull();
        review.RestaurantReply.Should().BeNull();
        review.RepliedAt.Should().BeNull();
    }

    [Fact]
    public void Review_SetProperties_RetainsValues()
    {
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var review = new Review
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            UserId = userId,
            RestaurantId = restaurantId,
            Rating = 4,
            ReviewText = "Great food, fast delivery!",
            DeliveryRating = 5,
            IsAnonymous = true,
            IsVisible = true,
            HelpfulCount = 3,
            ReportCount = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        review.OrderId.Should().Be(orderId);
        review.UserId.Should().Be(userId);
        review.RestaurantId.Should().Be(restaurantId);
        review.Rating.Should().Be(4);
        review.ReviewText.Should().Be("Great food, fast delivery!");
        review.DeliveryRating.Should().Be(5);
        review.IsAnonymous.Should().BeTrue();
        review.HelpfulCount.Should().Be(3);
    }

    [Fact]
    public void Review_RestaurantReply_CanBeSet()
    {
        var now = DateTimeOffset.UtcNow;

        var review = new Review
        {
            RestaurantReply = "Thank you for your feedback!",
            RepliedAt = now
        };

        review.RestaurantReply.Should().Be("Thank you for your feedback!");
        review.RepliedAt.Should().Be(now);
    }

    [Fact]
    public void Review_IsVisible_CanBeSetToFalse()
    {
        var review = new Review { IsVisible = false };

        review.IsVisible.Should().BeFalse();
    }

    [Fact]
    public void Review_PhotosCollection_CanAddPhotos()
    {
        var review = new Review();
        var photo = new ReviewPhoto { Id = Guid.NewGuid() };

        review.Photos.Add(photo);

        review.Photos.Should().HaveCount(1);
        review.Photos.Should().Contain(photo);
    }

    [Fact]
    public void Review_VotesCollection_CanAddVotes()
    {
        var review = new Review();
        var vote = new ReviewVote
        {
            ReviewId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsHelpful = true
        };

        review.Votes.Add(vote);

        review.Votes.Should().HaveCount(1);
        review.Votes.Should().Contain(vote);
    }

    [Fact]
    public void Review_ReportsCollection_CanAddReports()
    {
        var review = new Review();
        var report = new ReviewReport { Id = Guid.NewGuid() };

        review.Reports.Add(report);

        review.Reports.Should().HaveCount(1);
        review.Reports.Should().Contain(report);
    }

    [Fact]
    public void Review_Rating_AcceptsValidRange()
    {
        var review = new Review { Rating = 1 };
        review.Rating.Should().Be(1);

        review.Rating = 5;
        review.Rating.Should().Be(5);
    }

    [Fact]
    public void Review_DeliveryRating_AcceptsValidRange()
    {
        var review = new Review { DeliveryRating = 1 };
        review.DeliveryRating.Should().Be(1);

        review.DeliveryRating = 5;
        review.DeliveryRating.Should().Be(5);
    }
}
