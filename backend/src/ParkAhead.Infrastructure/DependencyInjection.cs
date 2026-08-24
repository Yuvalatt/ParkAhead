using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkAhead.Infrastructure.Persistence;

namespace ParkAhead.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ParkAheadDb")
            ?? throw new InvalidOperationException("Missing connection string 'ParkAheadDb'.");

        services.AddDbContext<ParkAheadDbContext>(options => options.UseNpgsql(connectionString));

        // Event source registration (Ticketmaster / Mock) and the background refresh service
        // are added here once the risk-calculation and ingestion logic are implemented.

        return services;
    }
}
