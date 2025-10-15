using System.Linq.Expressions;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.App.Repositories;

/// <summary>
/// Repository interface for entities using application-level partitioning (constant partition key)
/// </summary>
public interface IApplicationRepository<T> where T : IEntity
{
    /// <summary>
    /// Retrieves a single entity by predicate
    /// </summary>
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Lists all entities filtered by optional predicate
    /// </summary>
    Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    /// Adds a new entity
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Adds a range of entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity
    /// </summary>
    Task DeleteAsync(T entity);
}
