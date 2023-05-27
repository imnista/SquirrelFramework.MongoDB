namespace SquirrelFramework.Repository.Internal
{
    internal class RepositoryContext
    {
        private static PartitionLevel partitionLevel;

        [ThreadStatic] private static string? currentPartitionName;

        // For Database Level
        private static string currentPartitionDatabaseNamePrefix;

        private static string currentPartitionDatabaseNameFormat;

        // For Collection Level
        private static string currentPartitionCollectionNameFormat;

        static RepositoryContext()
        {
            partitionLevel = PartitionLevel.Disable;
            currentPartitionName = null;
            currentPartitionDatabaseNamePrefix = "Partition";
            currentPartitionDatabaseNameFormat = "{0:currentPartitionDatabaseNamePrefix}_{1:currentPartitionName}";
            currentPartitionCollectionNameFormat = "{0:collectionBaseName}_{1:currentPartitionName}";
        }

        public static void InitializePartitionSupportLevel(PartitionLevel partitionSupportLevel)
        {
            partitionLevel = partitionSupportLevel;
        }

        public static PartitionLevel GetPartitionSupportLevel()
        {
            return partitionLevel;
        }

        public static void InitializeDataBaseLevelParams(string partitionDatabaseNamePrefix = "", string partitionDatabaseNameFormat = "")
        {
            if (partitionLevel != PartitionLevel.DatabaseLevel)
            {
                throw new Exception("Please initialize partition support level at first.");
            }
            if (!string.IsNullOrWhiteSpace(partitionDatabaseNamePrefix))
            {
                currentPartitionDatabaseNamePrefix = partitionDatabaseNamePrefix;
            }
            if (!string.IsNullOrWhiteSpace(partitionDatabaseNameFormat))
            {
                currentPartitionDatabaseNameFormat = partitionDatabaseNameFormat;
            }
        }

        public static void InitializeCollectionLevelParams(string partitionCollectionNameFormat = "")
        {
            if (partitionLevel != PartitionLevel.CollectionLevel)
            {
                throw new Exception("Please initialize partition support level at first.");
            }
            if (!string.IsNullOrWhiteSpace(partitionCollectionNameFormat))
            {
                currentPartitionCollectionNameFormat = partitionCollectionNameFormat;
            }
        }

        public static void BindCurrentPartition(string partitionName)
        {
            if (partitionLevel == PartitionLevel.Disable)
            {
                throw new Exception("Please initialize partition support level at first.");
            }
            if (string.IsNullOrWhiteSpace(partitionName))
            {
                throw new ArgumentException("Must specify the current partition name", nameof(partitionName));
            }
            currentPartitionName = partitionName;
        }

        public static string? GetCurrentPartitionName()
        {
            return currentPartitionName;
        }

        public static string GetDatabaseName()
        {
            if (partitionLevel != PartitionLevel.DatabaseLevel)
            {
                throw new Exception("Please initialize partition support level at first.");
            }
            if (string.IsNullOrWhiteSpace(currentPartitionName))
            {
                throw new Exception("Please bind current partition at first.");
            }
            return string.Format(currentPartitionDatabaseNameFormat, currentPartitionDatabaseNamePrefix, currentPartitionName);
        }

        public static string GetCollectionName(string collectionBaseName)
        {
            if (partitionLevel != PartitionLevel.CollectionLevel)
            {
                throw new Exception("Please initialize partition support level at first.");
            }
            if (string.IsNullOrWhiteSpace(currentPartitionName))
            {
                throw new Exception("Please bind current partition at first.");
            }
            return string.Format(currentPartitionCollectionNameFormat, collectionBaseName, currentPartitionName);
        }
    }
}