using Microsoft.Azure.Cosmos;

namespace BizName.Studio.Data.Database.Impl;

/// <summary>
///     Database context implementation providing shared database infrastructure dependencies
/// </summary>
internal class DatabaseContext(CosmosClient cosmosClient, string databaseName) : IDatabaseContext
{
    public CosmosClient CosmosClient { get; } = cosmosClient;
    public string DatabaseName { get; } = databaseName;
}
