namespace FileStorageMigration.Model.Options
{
    public class MigrationOptions
    {
        public string StoreSourceRootPath { get; set; }
        public string SourceDirectoryName { get; set; }
        public string DestinationPath { get; set; }
        public string DirectoryFullPath { get; set; }
        public int DestinationDriveId { get; set; }
        public int DestinationDirectoryId { get; set; }
        public int AdminUserId { get; set; }
    }
}
