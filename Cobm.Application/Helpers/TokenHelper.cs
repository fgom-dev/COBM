using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cobm.Application.Helpers;

public static class TokenHelper
{
    public static TokenValidationParameters GetTokenValidationParameters(IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var tokenKey = Encoding.UTF8.GetBytes(jwtSettings.GetValue<string>("SecretKey") ?? string.Empty);

        return new TokenValidationParameters()
        {
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateAudience = true,
            ValidAudience = jwtSettings.GetValue<string>("Audience"),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(tokenKey)
        };
    }
    
}