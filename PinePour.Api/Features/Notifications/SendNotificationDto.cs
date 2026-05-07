using System.ComponentModel.DataAnnotations;

namespace PinePour.Api.Features.Notifications;

public class SendNotificationDto
{
    public int? UserId { get; set; }

    [Required]
    public string Channel { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;
}
