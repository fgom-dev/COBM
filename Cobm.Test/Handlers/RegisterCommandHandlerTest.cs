using System.Text.Json;
using Bogus;
using Cobm.Application.Commands.Auth;
using Cobm.Application.DTOs.Users;
using Cobm.Application.Errors;
using Cobm.Application.Extensions;
using Cobm.Application.Handlers.Auth;
using Cobm.Domain.Entities;
using Cobm.Infra.Db;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Cobm.Test.Handlers;

public sealed class RegisterCommandHandlerTest
{
    private readonly RegisterCommandHandler _handler;
    private readonly Faker _faker = new ("pt_BR");
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Guid _roleId;
    private JsonSerializerOptions _jsonOptions;

    public RegisterCommandHandlerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        // App Db Context
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>();
        dbContextOptions.UseInMemoryDatabase("BancoTeste");
        
        var dbContext = new AppDbContext(dbContextOptions.Options);

        var role = dbContext.Roles.Add(new Role("RoleTeste"));
        _roleId = role.Entity.Id;
        dbContext.Users.Add(new User(_roleId, "Test Name", "test@mail.com", _faker.Random.AlphaNumeric(8)));
        dbContext.SaveChanges();
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        
        _handler = new RegisterCommandHandler(dbContext);
    }

    [Fact]
    public async Task Handle_GivenNonExistentRoleId_ThenShoudReturnNotFoundError()
    {
        // Given Non Existent RoleId
        var roleId = _faker.Random.Guid();
        var name = _faker.Person.FullName;
        var email = _faker.Person.Email;
        var password = _faker.Random.AlphaNumeric(8);
        var nonExistentRoleIdCommand = new RegisterCommand(roleId, name, email, password);
        
        // When handle
        var result = await _handler.Handle(nonExistentRoleIdCommand, CancellationToken.None);
        
        // Then Shoud Return NotFoundError
        result.IsSuccess().Should().Be(false);
        result.GetError().Should().BeOfType<NotFoundError<Role>>();
    }
    
    [Fact]
    public async Task Handle_GivenExistentUserEmail_ThenShoudReturnAlreadyExistsError()
    {
        // Given Existent User Email
        var name = _faker.Person.FullName;
        var password = _faker.Random.AlphaNumeric(8);
        var existentUserEmail = "test@mail.com";
        var existentUserEmailCommand = new RegisterCommand(_roleId, name, existentUserEmail, password);
        
        // When handle
        var result = await _handler.Handle(existentUserEmailCommand, CancellationToken.None);
        
        // Then Shoud Return AlreadyExistsError
        result.IsSuccess().Should().Be(false);
        result.GetError().Should().BeOfType<AlreadyExistsError<User>>();
    }
    
    [Fact]
    public async Task Handle_GivenUserParameters_ThenShoudReturnNewUser()
    {
        // Given Existent User Email
        var name = _faker.Person.FullName;
        var password = _faker.Random.AlphaNumeric(8);
        var email = _faker.Person.Email;
        var registerCommand = new RegisterCommand(_roleId, name, email, password);
        
        // When handle
        var result = await _handler.Handle(registerCommand, CancellationToken.None);
        
        // Then Shoud Return AlreadyExistsError
        result.IsSuccess().Should().Be(true);
        result.GetSuccessResult().Should().BeOfType<UserDto>();
        result.GetSuccessResult().Id.Should().NotBeEmpty();
        result.GetSuccessResult().RoleId.Should().Be(_roleId);
        result.GetSuccessResult().Name.Should().Be(name);
        result.GetSuccessResult().Email.Should().Be(email);
        result.GetSuccessResult().Blocked.Should().Be(false);
        result.GetSuccessResult().EmailVerified.Should().Be(false);
        
        var json = JsonSerializer.Serialize(result.GetSuccessResult(), _jsonOptions);
        _testOutputHelper.WriteLine(json);
    }
}