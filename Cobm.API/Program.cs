using Cobm.Application.Helpers;
using Cobm.CrossCutting.AppDependencies;
using Cobm.CrossCutting.Authorizations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = TokenHelper.GetTokenValidationParameters(builder.Configuration);
});

builder.Services.AddAuthorizationBuilder().AddPolicies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();