using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using BizName.Studio.Contracts.Common;
using BizName.Studio.App.Repositories;
using BizName.Studio.Data.Database;
using BizName.Studio.Data.Serialization;

namespace BizName.Studio.Data.Repositories.Impl;

/// <summary>
/// Cosmos DB repository implementation for entities using tenant-level partitioning (dynamic partition key by tenantId)
/// </summary>
internal class PartitionedRepositoryImpl<T>(IDatabaseContext databaseContext) : IPartitionedRepository<T> where T : IEntity
{
    private readonly Container _container = databaseContext.CosmosClient.GetContainer(databaseContext.DatabaseName, GetContainerName());

    /// <summary>
    /// Retrieves a single entity by tenantId and predicate.
    /// </summary>
    public async Task<T?> GetAsync(Guid tenantId, Expression<Func<T, bool>> predicate)
    {
        var query = BuildQueryWithPartitionKey(tenantId, predicate);

        using var iterator = query.ToFeedIterator();
        var response = await iterator.ReadNextAsync();

        return response.FirstOrDefault();
    }

    /// <summary>
    /// Lists all entities filtered by tenantId and optional predicate.
    /// </summary>
    public async Task<IEnumerable<T>> ListAsync(Guid tenantId, Expression<Func<T, bool>>? predicate = null)
    {
        var query = BuildQueryWithPartitionKey(tenantId, predicate);
        
        var results = new List<T>();
        using var iterator = query.ToFeedIterator();
        
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }
        
        return results;
    }

    /// <summary>
    /// Adds a new entity and ensures the tenantId is valid.
    /// </summary>
    public async Task AddAsync(Guid tenantId, T entity)
    {
        var cosmosDoc = PrepareCosmosDocument(entity, tenantId);
        await _container.CreateItemAsync(cosmosDoc, new PartitionKey(tenantId.ToString()));
    }

    /// <summary>
    /// Adds a range of entities with the specified tenantId.
    /// </summary>
    public async Task AddRangeAsync(Guid tenantId, IEnumerable<T> entities)
    {
        var tasks = entities.Select(async entity =>
        {
            var cosmosDoc = PrepareCosmosDocument(entity, tenantId);
            await _container.CreateItemAsync(cosmosDoc, new PartitionKey(tenantId.ToString()));
        });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Updates an existing entity and ensures the tenantId is valid.
    /// </summary>
    public async Task UpdateAsync(Guid tenantId, T entity)
    {
        var cosmosDoc = PrepareCosmosDocument(entity, tenantId);
        var id = GetEntityId(entity);
        await _container.ReplaceItemAsync(cosmosDoc, id, new PartitionKey(tenantId.ToString()));
    }

    /// <summary>
    /// Deletes an entity and ensures the tenantId is valid.
    /// </summary>
    public async Task DeleteAsync(Guid tenantId, T entity)
    {
        var id = GetEntityId(entity);
        await _container.DeleteItemAsync<dynamic>(id, new PartitionKey(tenantId.ToString()));
    }

    /// <summary>
    /// Builds a query with tenantId filtering and optional predicates.
    /// </summary>
    private IQueryable<T> BuildQueryWithPartitionKey(Guid tenantId, Expression<Func<T, bool>>? predicate)
    {
        IQueryable<T> query = _container.GetItemLinqQueryable<T>(
            allowSynchronousQueryExecution: true,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(tenantId.ToString())
            },
            linqSerializerOptions: BizNameCosmosSerializationOptions.LinqDefault);

        // Apply additional predicates
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query;
    }

    /// <summary>
    /// Prepares a Cosmos document from an App entity by adding tenantId
    /// </summary>
    protected internal JsonObject PrepareCosmosDocument(T entity, Guid tenantId)
    {
        var json = JsonSerializer.Serialize(entity, BizNameCosmosSerializationOptions.Default);
        var cosmosDoc = JsonNode.Parse(json)!.AsObject();
        cosmosDoc[DatabaseConstants.PartitionKeysNames.Tenant] = tenantId.ToString();

        return cosmosDoc;
    }

    /// <summary>
    /// Gets the container name for the entity type
    /// </summary>
    private static string GetContainerName()
    {
        return typeof(T).Name;
    }

    /// <summary>
    /// Gets the entity ID for Cosmos DB operations
    /// </summary>
    private static string GetEntityId(T entity)
    {
        return entity.Id.ToString();
    }

}
