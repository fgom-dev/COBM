using Cobm.Application.Commands.Auth;
using Cobm.Application.DTOs.Auth;
using Cobm.Application.Errors;
using Cobm.Domain.Helpers;
using Cobm.Infra.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Cobm.Application.Handlers.Auth;

public sealed class LoginCommandHandler(AppDbContext appDbContext, ITokenManager tokenManager) : IRequestHandler<LoginCommand, OneOf<LoginResponseDto, AppError>>
{
    public async Task<OneOf<LoginResponseDto, AppError>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await appDbContext
            .Users
            .AsNoTracking()
            .Include(x => x.UserClaims)
            .Include(x => x.Role)
            .ThenInclude(x => x!.RoleClaims)
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        
        if (user == null)
            return new InvalidEmailOrPasswordError();
        
        if (!user.VerifyHashedPassword(request.Password))
            return new InvalidEmailOrPasswordError();
        
        // gerar tokens
        var userToken = tokenManager.GenerateUserToken(user);
        var refreshToken = tokenManager.GenerateRefreshToken(user);
        
        // retornar objeto
        return new LoginResponseDto(userToken, refreshToken);
    }
}