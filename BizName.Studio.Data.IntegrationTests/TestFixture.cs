using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BizName.Studio.Data.Database;

namespace BizName.Studio.Data.IntegrationTests;

/// <summary>
/// Test fixture that sets up the integration test environment
/// </summary>
public class TestFixture : IAsyncLifetime
{
    public ServiceProvider ServiceProvider { get; private set; } = null!;
    public IConfiguration Configuration { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Build configuration
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Setup services
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder => builder.AddConsole());
        
        // Register Data layer services using the ServiceConfigurator
        var configurator = new ServiceConfigurator();
        configurator.Register(services, Configuration);

        ServiceProvider = services.BuildServiceProvider();

        // Initialize database and containers
        var databaseInitializer = ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await databaseInitializer.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        if (ServiceProvider != null)
        {
            // Optional: Clean up test database
            // For now, we'll leave it for inspection
            await ServiceProvider.DisposeAsync();
        }
    }
}
