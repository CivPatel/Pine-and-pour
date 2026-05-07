namespace PinePour.Api.Features.Auth;

public class UpdateProfileDto
{
    public string? DisplayName { get; set; }
    public DateTime? Birthday { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
