using Cobm.Application.DTOs.Auth;
using Cobm.Application.Errors;
using MediatR;
using OneOf;

namespace Cobm.Application.Commands.Auth;
public record LoginCommand(string Email, string Password) : IRequest<OneOf<LoginResponseDto, AppError>>;