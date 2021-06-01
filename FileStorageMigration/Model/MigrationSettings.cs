namespace FileStorageMigration.Model
{
    public class MigrationSettings
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public string DirectoryFullPath { get; set; }
        public int DestinationDriveId { get; set; }
        public int DestinationDirectoryId { get; set; }
        public int AdminUserId { get; set; }
    }
}
