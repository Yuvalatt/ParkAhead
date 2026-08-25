using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkAhead.Application.EventSources;
using ParkAhead.Application.RiskForecast;
using ParkAhead.Infrastructure.EventSources;
using ParkAhead.Infrastructure.Persistence;

namespace ParkAhead.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ParkAheadDb")
            ?? throw new InvalidOperationException("Missing connection string 'ParkAheadDb'.");

        services.AddDbContext<ParkAheadDbContext>(options => options.UseNpgsql(connectionString));

        // Swapping this one registration for a real provider (Ticketmaster, etc.) is the entire
        // migration path — RiskForecastService and ParkingRiskEngine never change.
        services.AddScoped<IEventSource, MockEventSource>();
        services.AddScoped<RiskForecastService>();

        // The background refresh service is added here once event ingestion needs to run periodically.

        return services;
    }
}
