namespace FileStorageMigration.Model.Options
{
    public class MigrationOptions
    {
        public string AbsouluteSourceRootPath { get; set; }
        public string AbsouluteDestinationRootPath { get; set; }
        public string RelativeSourceDirectoryName { get; set; }
        public string RelativeDestinationDirectoryName { get; set; }
        public int DestinationDriveId { get; set; }
        public int DestinationDirectoryId { get; set; }
        public int AdminUserId { get; set; }
        public bool IsRemoveSourceFiles { get; set; } = false;
    }
}
