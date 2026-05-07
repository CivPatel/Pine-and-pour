namespace PinePour.Api.Features.Rewards;

public class PointsBalanceDto
{
    public int Points { get; set; }
    public string CurrentTier { get; set; } = string.Empty;
    public string NextTier { get; set; } = string.Empty;
    public int PointsToNextTier { get; set; }
}
