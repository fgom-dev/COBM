using Cobm.Application.DTOs.Users;
using Cobm.Application.Errors;
using MediatR;
using OneOf;

namespace Cobm.Application.Commands.Auth;

public record RegisterCommand(Guid RoleId, string Name, string Email, string Password) : IRequest<OneOf<UserDto, AppError>>;