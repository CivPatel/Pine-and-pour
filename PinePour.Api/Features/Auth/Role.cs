using Microsoft.AspNetCore.Identity;

namespace PinePour.Api.Features.Auth;

public class Role : IdentityRole<int>
{
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
