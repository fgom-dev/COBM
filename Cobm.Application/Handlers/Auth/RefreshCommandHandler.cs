using Cobm.Application.Commands.Auth;
using Cobm.Application.DTOs.Auth;
using Cobm.Application.Errors;
using Cobm.Domain.Helpers;
using Cobm.Infra.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Cobm.Application.Handlers.Auth;

public sealed class RefreshCommandHandler(AppDbContext appDbContext, ITokenManager tokenManager) : IRequestHandler<RefreshCommand, OneOf<LoginResponseDto, AppError>>
{
    public async Task<OneOf<LoginResponseDto, AppError>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var token = await tokenManager.ValidateTokenAsync(request.RefreshToken);
        
        if (!token.isValid) return new InvalidTokenError();
        
        var user = await appDbContext
            .Users
            .AsNoTracking()
            .Include(x => x.UserClaims)
            .Include(x => x.Role)
            .ThenInclude(x => x!.RoleClaims)
            .FirstOrDefaultAsync(x => x.Email == token.userEmail, cancellationToken);
        
        if (user == null) return new InvalidTokenError();
        
        // gerar tokens
        var userToken = tokenManager.GenerateUserToken(user);
        var refreshToken = tokenManager.GenerateRefreshToken(user);
        
        // retornar objeto
        return new LoginResponseDto(userToken, refreshToken);
    }
}