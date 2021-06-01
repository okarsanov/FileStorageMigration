using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageMigration.Entities.WebApi
{
    [Table("FileExt")]
    public class FileExt
    {
        public int ID { get; set; }
        public bool Hidden { get; set; }
        public string SedkpIdentity { get; set; }
        public string FileLink { get; set; }
        public int FileItemId { get; set; }
        public int DriveId { get; set; }
    }
}
