using Cobm.Application.Commands.Auth;
using Cobm.Application.DTOs.Users;
using Cobm.Application.Errors;
using Cobm.Domain.Entities;
using Cobm.Infra.Db;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Cobm.Application.Handlers.Auth;

public sealed class RegisterCommandHandler(AppDbContext appDbContext) : IRequestHandler<RegisterCommand, OneOf<UserDto, AppError>>
{
    public async Task<OneOf<UserDto, AppError>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var role = await appDbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.RoleId);

        if (role == null)
            return new NotFoundError<Role>();
        
        var userInDb = await appDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (userInDb != null)
            return new AlreadyExistsError<User>();
        
        var newUser = new User(request.RoleId, request.Name, request.Email, request.Password);
        
        await appDbContext.Users.AddAsync(newUser, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return (UserDto)newUser;
    }
}