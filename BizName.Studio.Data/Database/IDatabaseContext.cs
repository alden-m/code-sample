using Microsoft.Azure.Cosmos;

namespace BizName.Studio.Data.Database;

/// <summary>
///     Database context interface providing shared database infrastructure dependencies
/// </summary>
public interface IDatabaseContext
{
    CosmosClient CosmosClient { get; }
    string DatabaseName { get; }
}
