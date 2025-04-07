using System.Text.Json;
using Bogus;
using Cobm.Domain.Entities;
using FluentAssertions;
using Xunit.Abstractions;

namespace Cobm.Test.Entities;

public sealed class UserTests(ITestOutputHelper testOutputHelper)
{
    private readonly Faker _faker = new ("pt_BR");
    
    [Fact]
    public void Constructor_GivenAllParameters_ThenShouldSetThePropertiesCorrectly()
    {
        var roleId = Guid.NewGuid();
        var expectedName = _faker.Person.FirstName;
        var expectedEmail = _faker.Person.Email;
        var expectedPassword = _faker.Random.AlphaNumeric(8);
        // Act
        var user = new User(roleId, expectedName, expectedEmail, expectedPassword); 
         
        // Assert
        user.Id.Should().NotBeEmpty("Should contain an ID");
        user.Name.Should().Be(expectedName, "Should be the same name");
        user.Email.Should().Be(expectedEmail, "Should be the same email address");
         
        var json = JsonSerializer.Serialize(user);
        testOutputHelper.WriteLine(json);
    }
}