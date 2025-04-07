using System.Text.Json;
using Bogus;
using Cobm.Domain.Entities;
using FluentAssertions;
using Xunit.Abstractions;

namespace Cobm.Test.Entities;

public class UserClaimTests(ITestOutputHelper testOutputHelper)
{
    private readonly Faker _faker = new ("pt_BR");
    
    [Fact]
    public void Constructor_GivenAllParameters_ThenShouldSetThePropertiesCorrectly()
    {
        var expectedUserId = Guid.NewGuid();
        var expectedClaimType = _faker.Lorem.Word();
        var expectedClaimValue = _faker.Lorem.Word();

        var userClaim = new UserClaim(expectedUserId, expectedClaimType, expectedClaimValue);
        
        userClaim.UserId.Should().NotBeEmpty("Should contain an UserId");
        userClaim.Type.Should().Be(expectedClaimType, "Should be the same ClaimType");
        userClaim.Value.Should().Be(expectedClaimValue, "Should be the same ClaimValue");
        
        var json = JsonSerializer.Serialize(userClaim);
        testOutputHelper.WriteLine(json);
    }
}