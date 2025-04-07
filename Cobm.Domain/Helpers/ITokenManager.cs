using Cobm.Domain.Entities;

namespace Cobm.Domain.Helpers;

public interface ITokenManager
{
    string GenerateUserToken(User user);
    string GenerateRefreshToken(User user);
    Task<(bool isValid, string? userEmail)> ValidateTokenAsync(string token);
}