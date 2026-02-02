namespace HotelPlatform.Domain.Common.ValueObjects;

public record AggregatedRating
{
    public decimal AverageScore { get; }
    public int TotalCounts { get; }

    private AggregatedRating()
    { }
    private AggregatedRating(decimal averageScore, int totalCounts)
    {
        AverageScore = averageScore;
        TotalCounts = totalCounts;
    }

    public static AggregatedRating Empty => new(0, 0);

    public static AggregatedRating Create(decimal averageScore, int totalCounts)
    {
        if (totalCounts <= 0)
        {
            return Empty;
        }
        
        var clampedAverage = Math.Clamp(averageScore, 0, 5);
        return new AggregatedRating(Math.Round(clampedAverage, 2), totalCounts);
        
        
    }
    
    public bool HasRatings => TotalCounts > 0;
    public override string ToString() => 
        HasRatings ? $"{AverageScore:F1} ({TotalCounts} reviews)" : "No ratings yet";
}
