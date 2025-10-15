using System.Linq.Expressions;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.App.Repositories;

/// <summary>
/// Repository interface for entities using tenant-level partitioning (dynamic partition key by tenantId)
/// </summary>
public interface IPartitionedRepository<T> where T : IEntity
{
    /// <summary>
    ///     Retrieves a single entity by tenantId and predicate
    /// </summary>
    Task<T?> GetAsync(Guid tenantId, Expression<Func<T, bool>> predicate);

    /// <summary>
    ///     Lists all entities filtered by tenantId and optional predicate
    /// </summary>
    Task<IEnumerable<T>> ListAsync(Guid tenantId, Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    ///     Adds a new entity and ensures the tenantId is valid.
    /// </summary>
    Task AddAsync(Guid tenantId, T entity);

    /// <summary>
    /// Adds a range of entities with the specified tenantId.
    /// </summary>
    Task AddRangeAsync(Guid tenantId, IEnumerable<T> entities);

    /// <summary>
    ///     Updates an existing entity and ensures the tenantId is valid.
    /// </summary>
    Task UpdateAsync(Guid tenantId, T entity);

    /// <summary>
    ///     Deletes an entity and ensures the tenantId is valid.
    /// </summary>
    Task DeleteAsync(Guid tenantId, T entity);
}
