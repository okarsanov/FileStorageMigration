using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorageMigration.Entities.FileStorage
{
    [Table("files")]
    public class FileInfoEntity
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public long Size { get; set; }
        public long StorageSize { get; set; }
        public int? StorageId { get; set; }
        public int? ProtectId { get; set; }
        public string Hash { get; set; }
    }
}
