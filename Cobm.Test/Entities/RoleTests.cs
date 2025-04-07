using System.Text.Json;
using Bogus;
using Cobm.Domain.Entities;
using FluentAssertions;
using Xunit.Abstractions;

namespace Cobm.Test.Entities;

public class RoleTests(ITestOutputHelper testOutputHelper)
{
    private readonly Faker _faker = new ("pt_BR");
    
    [Fact]
    public void Constructor_GivenAllParameters_ThenShouldSetThePropertiesCorrectly()
    {
        var expectedDescription = _faker.Lorem.Word();
        // Act
        var role = new Role(expectedDescription); 
         
        // Assert

        role.Id.Should().NotBeEmpty("Should contain an ID");
        role.Description.Should().Be(expectedDescription, "Should be the same description");
         
        var json = JsonSerializer.Serialize(role);
        testOutputHelper.WriteLine(json);
    }
}