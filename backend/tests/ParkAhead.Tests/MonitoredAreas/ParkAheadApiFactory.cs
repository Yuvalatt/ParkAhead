using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ParkAhead.Infrastructure.Persistence;

namespace ParkAhead.Tests.MonitoredAreas;

/// <summary>
/// Boots the real API pipeline (routing, model binding, validation) against an EF Core
/// InMemory database instead of Postgres, so HTTP-level behavior can be tested without Docker.
/// </summary>
public class ParkAheadApiFactory : WebApplicationFactory<Program>
{
    // AddDbContext re-invokes the options-builder delegate on every DbContext instantiation
    // (i.e. every request, since it's scoped) — generating the database name inside that
    // delegate would hand each request its own empty database. Capturing it once here means
    // every request in a test shares the same in-memory store.
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:ParkAheadDb"] = "unused-placeholder"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ParkAheadDbContext>>();
            services.AddDbContext<ParkAheadDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
