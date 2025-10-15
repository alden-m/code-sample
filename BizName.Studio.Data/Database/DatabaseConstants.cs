namespace BizName.Studio.Data.Database;

/// <summary>
/// Constants used for Cosmos DB database operations
/// </summary>
internal static class DatabaseConstants
{

    /// <summary>
    /// Container names used in Cosmos DB
    /// </summary>
    public static class Containers
    {
        /// <summary>
        /// Container for Tenant entities
        /// </summary>
        public const string Tenant = "Tenant";

        /// <summary>
        /// Container for Website entities
        /// </summary>
        public const string Website = "Website";

        /// <summary>
        /// Container for Experience entities
        /// </summary>
        public const string Experience = "Experience";
    }

    /// <summary>
    /// Partition key names used in Cosmos DB containers
    /// </summary>
    public static class PartitionKeysNames
    {
        /// <summary>
        /// Partition key for application-level entities (Tenant)
        /// </summary>
        public const string Application = "partitionKey";

        /// <summary>
        /// Partition key for tenant-level entities (Website, Experience)
        /// </summary>
        public const string Tenant = "tenantId";
    }
}
