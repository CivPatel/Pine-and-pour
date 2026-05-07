namespace PinePour.Api.Features.Admin;

public class DashboardOrderSummaryDto
{
    public int Id { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public string OrderType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
