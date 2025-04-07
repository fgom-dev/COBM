using System.Text.Json;
using Bogus;
using Cobm.Application.Commands.Auth;
using Cobm.Application.DTOs.Auth;
using Cobm.Application.Errors;
using Cobm.Application.Extensions;
using Cobm.Application.Handlers.Auth;
using Cobm.Domain.Entities;
using Cobm.Domain.Helpers;
using Cobm.Infra.Db;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;

namespace Cobm.Test.Handlers;

public sealed class RefreshCommandHandlerTest
{
    private readonly RefreshCommandHandler _handler;
    private readonly Faker _faker = new ("pt_BR");
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Role _role;
    private readonly User _user;
    private JsonSerializerOptions _jsonOptions;
    private ITokenManager _mockTokenManager;

    public RefreshCommandHandlerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        // App Db Context
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>();
        dbContextOptions.UseInMemoryDatabase("BancoTeste");
        
        var dbContext = new AppDbContext(dbContextOptions.Options);

        _role = dbContext.Roles.Add(new Role("RoleTeste")).Entity;
        _user = dbContext.Users.Add(new User(_role.Id, "Test Name", "test@mail.com", "Teste@123")).Entity;
        dbContext.SaveChanges();
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        _mockTokenManager = Substitute.For<ITokenManager>();
        
        _handler = new RefreshCommandHandler(dbContext, _mockTokenManager);
    }

    [Fact]
    public async Task Handle_GivenInvalidToken_ThenShoudReturnInvalidTokenError()
    {
        var invalidToken = _faker.Random.AlphaNumeric(125);
        var invalidTokenCommand = new RefreshCommand(invalidToken);
        var anyEmail = _faker.Person.Email;
        
        _mockTokenManager.ValidateTokenAsync(Arg.Any<string>())!.Returns(Task.FromResult((false, anyEmail))); 
        
        var result = await _handler.Handle(invalidTokenCommand, CancellationToken.None);

        result.IsSuccess().Should().BeFalse();
        result.GetError().Should().BeOfType<InvalidTokenError>();
    }
    
    [Fact]
    public async Task Handle_GivenInvalidUserEmail_ThenShoudReturnInvalidTokenError()
    {
        var invalidToken = _faker.Random.AlphaNumeric(125);
        var invalidTokenCommand = new RefreshCommand(invalidToken);
        
        _mockTokenManager.ValidateTokenAsync(Arg.Any<string>())!.Returns(Task.FromResult((true, _faker.Person.Email))); 
        
        var result = await _handler.Handle(invalidTokenCommand, CancellationToken.None);

        result.IsSuccess().Should().BeFalse();
        result.GetError().Should().BeOfType<InvalidTokenError>();
    }

    [Fact]
    public async Task Handle_GivenValidRefreshToken_ThenShoudReturnNewTokens()
    {
        var validRefreshToken = _faker.Random.AlphaNumeric(125);
        var validTokenCommand = new RefreshCommand(validRefreshToken);
        var validEmail = _user.Email;
        
        var newUserToken = _faker.Random.AlphaNumeric(125);
        var newUserRefreshToken = _faker.Random.AlphaNumeric(125);
        
        _mockTokenManager.ValidateTokenAsync(Arg.Any<string>())!.Returns(Task.FromResult((true, validEmail))); 
        _mockTokenManager.GenerateUserToken(Arg.Any<User>())!.Returns(newUserToken); 
        _mockTokenManager.GenerateRefreshToken(Arg.Any<User>())!.Returns(newUserRefreshToken); 
        
        var result = await _handler.Handle(validTokenCommand, CancellationToken.None);

        result.IsSuccess().Should().BeTrue();
        result.GetSuccessResult().Should().BeOfType<LoginResponseDto>();
        result.GetSuccessResult().UserToken.Should().Be(newUserToken);   
        result.GetSuccessResult().RefreshToken.Should().Be(newUserRefreshToken);   
    }
    
}