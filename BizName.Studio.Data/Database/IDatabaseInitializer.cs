using Microsoft.Azure.Cosmos;

namespace BizName.Studio.Data.Database;

/// <summary>
/// Interface for database initialization operations
/// </summary>
public interface IDatabaseInitializer
{
    /// <summary>
    /// Ensures the database and all required containers exist
    /// </summary>
    Task InitializeAsync();
}
