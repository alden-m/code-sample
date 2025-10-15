using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using BizName.Studio.Contracts.Common;
using BizName.Studio.App.Repositories;
using BizName.Studio.Data.Database;
using BizName.Studio.Data.Serialization;

namespace BizName.Studio.Data.Repositories.Impl;

/// <summary>
/// Cosmos DB repository implementation for entities using application-level partitioning (constant partition key)
/// </summary>
internal class ApplicationRepositoryImpl<T>(IDatabaseContext databaseContext) : IApplicationRepository<T> where T : IEntity
{
    private readonly Container _container = databaseContext.CosmosClient.GetContainer(databaseContext.DatabaseName, GetContainerName());
    private readonly string _partitionKeyValue = "APPLICATION";

    /// <summary>
    /// Retrieves a single entity by predicate
    /// </summary>
    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        var query = BuildQuery(predicate);
        
        using var iterator = query.ToFeedIterator();
        var response = await iterator.ReadNextAsync();
        
        return response.FirstOrDefault();
    }

    /// <summary>
    /// Lists all entities filtered by optional predicate
    /// </summary>
    public async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = BuildQuery(predicate);
        
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
    /// Adds a new entity
    /// </summary>
    public async Task AddAsync(T entity)
    {
        var cosmosDoc = PrepareCosmosDocument(entity);
        await _container.CreateItemAsync(cosmosDoc, new PartitionKey(_partitionKeyValue));
    }

    /// <summary>
    /// Adds a range of entities
    /// </summary>
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        var tasks = entities.Select(async entity =>
        {
            var cosmosDoc = PrepareCosmosDocument(entity);
            await _container.CreateItemAsync(cosmosDoc, new PartitionKey(_partitionKeyValue));
        });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    public async Task UpdateAsync(T entity)
    {
        var cosmosDoc = PrepareCosmosDocument(entity);
        var id = GetEntityId(entity);
        await _container.ReplaceItemAsync(cosmosDoc, id, new PartitionKey(_partitionKeyValue));
    }

    /// <summary>
    /// Deletes an entity
    /// </summary>
    public async Task DeleteAsync(T entity)
    {
        var id = GetEntityId(entity);
        await _container.DeleteItemAsync<dynamic>(id, new PartitionKey(_partitionKeyValue));
    }

    /// <summary>
    /// Builds a query with optional predicate
    /// </summary>
    private IQueryable<T> BuildQuery(Expression<Func<T, bool>>? predicate)
    {
        IQueryable<T> query = _container.GetItemLinqQueryable<T>(
            allowSynchronousQueryExecution: true,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(_partitionKeyValue)
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
    /// Prepares a Cosmos document from an App entity (adds partition key)
    /// </summary>
    protected internal JsonObject PrepareCosmosDocument(T entity)
    {
        var json = JsonSerializer.Serialize(entity, BizNameCosmosSerializationOptions.Default);
        var cosmosDoc = JsonNode.Parse(json)!.AsObject();
        cosmosDoc[DatabaseConstants.PartitionKeysNames.Application] = _partitionKeyValue;

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
