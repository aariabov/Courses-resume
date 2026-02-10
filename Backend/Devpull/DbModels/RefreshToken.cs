using Devpull.Controllers;

namespace Devpull.DbModels;

public class RefreshToken
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    public required string DeviceFingerprint { get; set; }
    public required string Token { get; set; }
    public required string? OldToken { get; set; }
    public required DateTime ExpiryDate { get; set; }
    public required DateTime? RefreshDate { get; set; }
    public required bool RememberMe { get; set; }

    public virtual AppUser User { get; set; } = null!;
}
