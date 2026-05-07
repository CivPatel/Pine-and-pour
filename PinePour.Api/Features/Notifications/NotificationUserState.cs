namespace PinePour.Api.Features.Notifications;

public class NotificationUserState
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public bool IsRead { get; set; }

    public bool IsCleared { get; set; }
}
