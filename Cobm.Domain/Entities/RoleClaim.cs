using System.Security.Claims;

namespace Cobm.Domain.Entities;

public sealed class RoleClaim(string claimType, string claimValue) : Claim(claimType, claimValue)
{
    // Ef constructor
    public RoleClaim() : this(string.Empty, string.Empty)
    {
    }
    
    public RoleClaim(Guid roleId, string claimType, string claimValue) : this(claimType, claimValue)
    {
        this.RoleId = roleId;
    }
    
    public Guid RoleId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public DateTime UpdatedAt { get; private set; } = DateTime.Now;
    public Role? Role { get; set; }
}