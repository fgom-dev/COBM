using Cobm.Application.Helpers;
using Cobm.Domain.Helpers;
using Cobm.Infra.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cobm.CrossCutting.AppDependencies;

public static class ApiExtensions
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                    configuration.GetSection("ConnectionStrings:DefaultConnection").Value,
                    x =>
                    {
                        x.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorNumbersToAdd: null);
                    })
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        
        // Add Mediatr
        var myhandlers = AppDomain.CurrentDomain.Load("Cobm.Application");
        services.AddMediatR(options => options.RegisterServicesFromAssembly(myhandlers));

        services.AddSingleton<ITokenManager, TokenManager>();
    }
}