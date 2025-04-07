using Microsoft.AspNetCore.Identity;

namespace Cobm.Domain.Entities;

public sealed class User : BaseEntity
{
    private readonly PasswordHasher<User> _hasher = new ();
    
    // Ef constructor
    public User() : this(Guid.Empty, string.Empty, string.Empty, string.Empty)
    {
    }

    public User(Guid roleId, string name, string email, string password)
    {
        RoleId = roleId;
        Name = name;
        Email = email;
        PasswordHashed = _hasher.HashPassword(this, password);
    }

    public Guid RoleId { get; private set; }
    public string Name { get; set; }
    public string Email { get; init; }
    public string PasswordHashed { get; private set; }
    public bool EmailVerified { get; private set; } = false;
    public bool Blocked { get; private set; } = false;
    public Role? Role { get; private set; }
    public ICollection<UserClaim> UserClaims { get; private set; } = [];
    
    public void VerifyEmail() => EmailVerified = true;
    public void Block() => Blocked = true;

    public bool VerifyHashedPassword(string password)
    {
        return _hasher.VerifyHashedPassword(this, PasswordHashed, password) == PasswordVerificationResult.Success;
    }
}