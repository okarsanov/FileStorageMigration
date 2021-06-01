using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageMigration.Entities.WebApi
{
    [Table("FileExt")]
    public class FileExt
    {
        public string SedkpIdentity { get; set; }
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string FileLink { get; set; }
        public int FileItemId { get; set; }
    }
}
