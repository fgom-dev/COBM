using System.Security.Claims;

namespace Cobm.Domain.Entities;

public sealed class UserClaim(string claimType, string claimValue) : Claim(claimType, claimValue)
{
    // Ef constructor
    public UserClaim() : this(string.Empty, string.Empty)
    {
    }
    
    public UserClaim(Guid userId, string claimType, string claimValue) : this(claimType, claimValue)
    {
        this.UserId = userId;
    }
    
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; } = DateTime.Now;
    public User? User { get; set; }
}