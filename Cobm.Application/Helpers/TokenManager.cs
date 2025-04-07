using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cobm.Domain.Entities;
using Cobm.Domain.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Cobm.Application.Helpers;

public sealed class TokenManager(IConfiguration configuration) : ITokenManager
{
    public string GenerateUserToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? string.Empty));

        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        claims.AddRange(user.UserClaims.Select(userClaim => new Claim(userClaim.Type, userClaim.Value)));
        claims.AddRange(user.Role!.RoleClaims.Select(roleClaim => new Claim(roleClaim.Type, roleClaim.Value)));

        var expirationTimeInMinutes = jwtSettings.GetValue<int>("ExpirationTimeInMinutes");

        var token = new JwtSecurityToken(
            issuer: jwtSettings.GetValue<string>("Issuer"),
            audience: jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationTimeInMinutes),
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? string.Empty));

        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var expirationTimeInMinutes = jwtSettings.GetValue<int>("RefreshExpirationTimeInMinutes");

        var token = new JwtSecurityToken(
            issuer: jwtSettings.GetValue<string>("Issuer"),
            audience: jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationTimeInMinutes),
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<(bool isValid, string? userEmail)> ValidateTokenAsync(string token)
    {
        if(string.IsNullOrWhiteSpace(token))
            return (false, null);
        
        var tokenParameters = TokenHelper.GetTokenValidationParameters(configuration);
        var validTokenResult = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, tokenParameters);
        
        if (!validTokenResult.IsValid)
            return (false, null);
        
        var userEmail = validTokenResult
            .Claims.FirstOrDefault(c => c.Key == ClaimTypes.Email).Value as string;

        return (true, userEmail);
    }
}