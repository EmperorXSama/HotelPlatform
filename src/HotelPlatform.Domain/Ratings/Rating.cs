using HotelPlatform.Domain.Common.Errors;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Ratings.Events;

namespace HotelPlatform.Domain.Ratings;

public class Rating : AggregateRoot<RatingId>
{
    public const int MinScore = 1;
    public const int MaxScore = 5;

    public HotelId HotelId { get; private set; }
    public UserId UserId { get; private set; }
    public int Score { get; private set; }
    public string? Comment { get; private set; }

    private Rating() : base() { }

    private Rating(
        RatingId id,
        HotelId hotelId,
        UserId userId,
        int score,
        string? comment) : base(id)
    {
        HotelId = hotelId;
        UserId = userId;
        Score = score;
        Comment = comment;
    }

    public static ErrorOr<Rating> Create(
        HotelId hotelId,
        UserId userId,
        int score,
        string? comment = null)
    {
        if (score is < MinScore or > MaxScore)
            return DomainErrors.Rating.InvalidScore;

        var rating = new Rating(
            RatingId.New(),
            hotelId,
            userId,
            score,
            comment?.Trim());

        rating.AddDomainEvent(new RatingCreatedEvent(
            rating.Id,
            rating.HotelId,
            rating.UserId,
            rating.Score));

        return rating;
    }

    public ErrorOr<Updated> Update(int score,DateTime utcNow ,string? comment = null)
    {
        if (score is < MinScore or > MaxScore)
            return DomainErrors.Rating.InvalidScore;

        var oldScore = Score;
        Score = score;
        Comment = comment?.Trim();
        SetUpdated(utcNow);

        AddDomainEvent(new RatingUpdatedEvent(
            Id,
            HotelId,
            UserId,
            oldScore,
            Score));

        return Result.Updated;
    }
}