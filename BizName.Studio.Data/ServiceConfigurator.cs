using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BizName.Studio.App.Configurator;
using BizName.Studio.App.Repositories;
using BizName.Studio.Contracts.Tenants;
using BizName.Studio.Data.Database;
using BizName.Studio.Data.Database.Impl;
using BizName.Studio.Data.Repositories.Impl;
using BizName.Studio.Data.Serialization;

namespace BizName.Studio.Data;

/// <summary>
/// Service configurator for Cosmos DB implementation
/// </summary>
public class ServiceConfigurator : IServiceConfigurator
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        // Register Cosmos DB client
        var cosmosConnectionString = configuration.GetConnectionString("CosmosDb") 
            ?? throw new InvalidOperationException("CosmosDb connection string is required");
        var cosmosDatabaseName = configuration["CosmosDb:DatabaseName"] ?? throw new InvalidOperationException("CosmosDb:DatabaseName configuration is required");

        services.AddSingleton<CosmosClient>(serviceProvider =>
        {
            return new CosmosClient(cosmosConnectionString, new CosmosClientOptions
            {
                Serializer = new SystemTextJsonCosmosSerializer()
            });
        });

        // Register the database context
        services.AddSingleton<IDatabaseContext>(serviceProvider =>
        {
            var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
            return new DatabaseContext(cosmosClient, cosmosDatabaseName);
        });

        // Register the database initializer
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        // Register repositories
        services.AddScoped(typeof(IPartitionedRepository<>), typeof(PartitionedRepositoryImpl<>));
        
        // Register Tenant with application-level partitioned repository
        services.AddScoped<IApplicationRepository<Tenant>>(serviceProvider =>
        {
            var databaseContext = serviceProvider.GetRequiredService<IDatabaseContext>();
            return new ApplicationRepositoryImpl<Tenant>(databaseContext);
        });

        // Business services are now registered in App layer ServiceConfigurator
        // Data layer only handles repository implementations
    }
}
