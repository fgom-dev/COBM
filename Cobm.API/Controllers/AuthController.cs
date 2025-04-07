using Cobm.Application.Commands.Auth;
using Cobm.Application.Errors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cobm.API.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(ISender mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IResult> Register(RegisterCommand command)
    {
        var result = await mediator.Send(command);
        
        return result.Match(
            user => Results.Created("/login", user),
            error =>
            {
                return error.ErrorType switch
                {
                    ErrorType.Validation => Results.BadRequest(error),
                    ErrorType.BusinessRule => Results.Conflict(error),
                    _ => Results.BadRequest(error)
                };
            });
    }
    
    [HttpPost("login")]
    public async Task<IResult> Login(LoginCommand command)
    {
        var result = await mediator.Send(command);

        return result.Match(
            loginResponse => Results.Ok(loginResponse),
            error =>
            {
                return error.ErrorType switch
                {
                    ErrorType.Validation => Results.BadRequest(error),
                    ErrorType.BusinessRule => Results.Conflict(error),
                    _ => Results.BadRequest(error)
                };
            }
        );
    }
    
    [HttpPost("refresh")]
    public async Task<IResult> Refresh(RefreshCommand command)
    {
        var result = await mediator.Send(command);

        return result.Match(
            loginResponse => Results.Ok(loginResponse),
            error =>
            {
                return error.ErrorType switch
                {
                    ErrorType.Validation => Results.BadRequest(error),
                    ErrorType.BusinessRule => Results.Conflict(error),
                    _ => Results.BadRequest(error)
                };
            }
        );
    }

    [Authorize(Policy = "user-create")]
    [HttpGet("hello-world")]
    public OkObjectResult HelloWorld()
    {
        return Ok("Hello World");
    }
}