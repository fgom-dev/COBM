namespace Cobm.Domain.Entities;

public sealed class Role(string description) : BaseEntity
{
    // Ef constructor
    public Role() : this(string.Empty)
    {
    }
    
    public string Description { get; private set; } = description;
    public ICollection<User>? Users { get; private set; }
    public ICollection<RoleClaim> RoleClaims { get; private set; }
}