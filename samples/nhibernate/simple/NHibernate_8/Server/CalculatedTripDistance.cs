public class CalculatedTripDistance
{
    public CalculationMethod CalculationMethod { get; set; }
    public decimal TripDistance { get; set; }
    public bool InvalidDistance { get; set; }
    public decimal SourceKey { get; set; }
    public decimal? StartLatitude { get; internal set; }
    public decimal? StartLongitude { get; internal set; }
    public decimal? EndLatitude { get; internal set; }
    public decimal? EndLongitude { get; internal set; }
    public long TripKey { get; set; }
}