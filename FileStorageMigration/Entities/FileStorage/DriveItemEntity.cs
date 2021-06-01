using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FileStorageMigration.Entities.FileStorage
{
    [Table("items")]
    public class DriveItemEntity
    {
        public int Id { get; set; }
        public int DriveId { get; set; }
        public int? DirectoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public int CreateById { get; set; }
        public int ModifyById { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public int? FileInfoId { get; set; }
        public string[] Tags { get; set; }
        public int ShareLinkCounter { get; set; }
        public string Extension { get; set; }
    }
}
