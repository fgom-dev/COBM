using System.Reflection;
using System.Text.Json;
using Bogus;
using Cobm.Application.Commands.Auth;
using Cobm.Application.DTOs.Auth;
using Cobm.Application.Errors;
using Cobm.Application.Extensions;
using Cobm.Application.Handlers.Auth;
using Cobm.Application.Helpers;
using Cobm.Domain.Entities;
using Cobm.Infra.Db;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Cobm.Test.Handlers;

public sealed class LoginCommandHandlerTest
{
    private readonly LoginCommandHandler _handler;
    private readonly Faker _faker = new ("pt_BR");
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Guid _roleId;
    private JsonSerializerOptions _jsonOptions;

    public LoginCommandHandlerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        // App Db Context
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>();
        dbContextOptions.UseInMemoryDatabase("BancoTeste");
        
        var dbContext = new AppDbContext(dbContextOptions.Options);

        var role = dbContext.Roles.Add(new Role("RoleTeste"));
        _roleId = role.Entity.Id;
        dbContext.Users.Add(new User(_roleId, "Test Name", "test@mail.com", "Teste@123"));
        dbContext.SaveChanges();
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var configuration = new ConfigurationBuilder()
            .SetBasePath(@"C:\Users\fndal\source\repos\projects\Cobm\Cobm.API\")
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var tokenManager = new TokenManager(configuration);
        
        _handler = new LoginCommandHandler(dbContext, tokenManager);
    }
    
    [Fact]
    public async Task Handle_GivenNonExistentUser_ThenShoudReturnInvalidEmailOrPasswordError()
    {
        // Given Non Existent User
        var email = _faker.Person.Email;
        var password = _faker.Random.AlphaNumeric(8);
        var nonExistentUserCommand = new LoginCommand(email, password);
        
        // When handle
        var result = await _handler.Handle(nonExistentUserCommand, CancellationToken.None);
        
        // Then Shoud Return NotFoundError
        result.IsSuccess().Should().Be(false);
        result.GetError().Should().BeOfType<InvalidEmailOrPasswordError>();
    }
    
    [Fact]
    public async Task Handle_GivenWrongPassword_ThenShoudReturnInvalidEmailOrPasswordError()
    {
        // Given Wrong Passwrod
        const string email = "test@mail.com";
        var password = _faker.Random.AlphaNumeric(8);
        var wrongPasswordCommand = new LoginCommand(email, password);
        
        // When handle
        var result = await _handler.Handle(wrongPasswordCommand, CancellationToken.None);
        
        // Then Shoud Return NotFoundError
        result.IsSuccess().Should().Be(false);
        result.GetError().Should().BeOfType<InvalidEmailOrPasswordError>();
    }
    
    [Fact]
    public async Task Handle_GivenValidParameters_ThenShoudReturnLoginResponseDto()
    {
        // Given Valid Parameters
        const string email = "test@mail.com";
        const string password = "Teste@123";
        var validParametersCommand = new LoginCommand(email, password);
        
        // When handle
        var result = await _handler.Handle(validParametersCommand, CancellationToken.None);
        
        // Then Shoud Return NotFoundError
        result.IsSuccess().Should().Be(true);
        result.GetSuccessResult().Should().BeOfType<LoginResponseDto>();
        result.GetSuccessResult().UserToken.Should().NotBeEmpty();
        result.GetSuccessResult().RefreshToken.Should().NotBeEmpty();
        
        var json = JsonSerializer.Serialize(result.GetSuccessResult(), _jsonOptions);
        _testOutputHelper.WriteLine(json);
    }
    
}