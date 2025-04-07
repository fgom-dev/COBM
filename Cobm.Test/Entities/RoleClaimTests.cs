using System.Text.Json;
using Bogus;
using Cobm.Domain.Entities;
using FluentAssertions;
using Xunit.Abstractions;

namespace Cobm.Test.Entities;

public class RoleClaimTests(ITestOutputHelper testOutputHelper)
{
    private readonly Faker _faker = new ("pt_BR");
    
    [Fact]
    public void Constructor_GivenAllParameters_ThenShouldSetThePropertiesCorrectly()
    {
        var expectedRoleId = Guid.NewGuid();
        var expectedClaimType = _faker.Lorem.Word();
        var expectedClaimValue = _faker.Lorem.Word();

        var roleClaim = new RoleClaim(expectedRoleId, expectedClaimType, expectedClaimValue);
        
        roleClaim.RoleId.Should().NotBeEmpty("Should contain an RoleId");
        roleClaim.Type.Should().Be(expectedClaimType, "Should be the same ClaimType");
        roleClaim.Value.Should().Be(expectedClaimValue, "Should be the same ClaimValue");
        
        var json = JsonSerializer.Serialize(roleClaim);
        testOutputHelper.WriteLine(json);
    }
}