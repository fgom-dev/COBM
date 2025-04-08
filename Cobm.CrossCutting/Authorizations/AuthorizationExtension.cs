using Microsoft.AspNetCore.Authorization;

namespace Cobm.CrossCutting.Authorizations;

public static class AuthorizationExtension
{
    public static void AddPolicies(this AuthorizationBuilder builder)
    {
        // user
        builder.AddPolicy("user-read", policy => policy.RequireClaim("user", "read"));
        builder.AddPolicy("user-create", policy => policy.RequireClaim("user", "create"));
    }
}