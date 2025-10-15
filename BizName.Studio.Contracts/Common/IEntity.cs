namespace BizName.Studio.Contracts.Common;

/// <summary>
/// Base interface for all entities with an Id property
/// </summary>
public interface IEntity
{
    Guid Id { get; set; }
}
