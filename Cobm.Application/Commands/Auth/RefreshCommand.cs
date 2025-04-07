using Cobm.Application.DTOs.Auth;
using Cobm.Application.Errors;
using MediatR;
using OneOf;

namespace Cobm.Application.Commands.Auth;

public record RefreshCommand(string RefreshToken) : IRequest<OneOf<LoginResponseDto, AppError>>;