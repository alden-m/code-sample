using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace BizName.Studio.Data.Database.Impl;

/// <summary>
/// Database initializer that ensures database and containers exist on startup
/// </summary>
public class DatabaseInitializer(IDatabaseContext databaseContext, ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    private readonly IDatabaseContext _databaseContext = databaseContext;
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    /// <summary>
    /// Ensures the database and all required containers exist
    /// </summary>
    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing Cosmos DB database and containers...");

        try
        {
            // Create database if it doesn't exist
            var databaseResponse = await _databaseContext.CosmosClient.CreateDatabaseIfNotExistsAsync(_databaseContext.DatabaseName);
            var database = databaseResponse.Database;
            
            _logger.LogInformation("Database '{DatabaseName}' initialized", _databaseContext.DatabaseName);

            // Create containers for each entity type
            await CreateContainerAsync(database, DatabaseConstants.Containers.Tenant, $"/{DatabaseConstants.PartitionKeysNames.Application}"); // Application-level partition
            await CreateContainerAsync(database, DatabaseConstants.Containers.Website, $"/{DatabaseConstants.PartitionKeysNames.Tenant}"); // Tenant-level partition
            await CreateContainerAsync(database, DatabaseConstants.Containers.Experience, $"/{DatabaseConstants.PartitionKeysNames.Tenant}"); // Tenant-level partition

            _logger.LogInformation("All containers initialized successfully");
        }
        catch (Exception ex) when (ex.ToString().Contains("actively refused") || ex.ToString().Contains("No connection could be made"))
        {
            _logger.LogError("Cosmos DB is not accessible at localhost:8081. Please start the Cosmos DB Emulator.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Cosmos DB database and containers");
            throw;
        }
    }

    /// <summary>
    /// Creates a container if it doesn't exist
    /// </summary>
    private async Task CreateContainerAsync(Microsoft.Azure.Cosmos.Database database, string containerName, string partitionKeyPath)
    {
        try
        {
            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerName,
                partitionKeyPath,
                throughput: 400); // Minimum throughput for development

            _logger.LogInformation("Container '{ContainerName}' with partition key '{PartitionKey}' initialized", 
                containerName, partitionKeyPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create container '{ContainerName}'", containerName);
            throw;
        }
    }
}
