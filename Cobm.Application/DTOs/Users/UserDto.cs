using Cobm.Domain.Entities;

namespace Cobm.Application.DTOs.Users;

public sealed record UserDto(Guid Id, Guid RoleId, string Name, string Email, bool EmailVerified, bool Blocked)
{
    public static implicit operator UserDto(User user) =>
        new UserDto(user.Id, user.RoleId, user.Name, user.Email, user.EmailVerified, user.Blocked);
}