using System.ComponentModel.DataAnnotations;

namespace PinePour.Api.Features.Payments;

public class CheckoutPaymentDto
{
    [Required]
    public int OrderId { get; set; }

    public string PaymentMethod { get; set; } = "Card";

    public decimal? Amount { get; set; }

    public string CardLastFour { get; set; } = string.Empty;

    public string StripeIntentId { get; set; } = string.Empty;
}
