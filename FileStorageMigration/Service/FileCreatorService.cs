using FileStorageMigration.Context;
using FileStorageMigration.Entities.FileStorage;
using FileStorageMigration.Entities.WebApi;
using FileStorageMigration.Model;
using Helpers;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageMigration.Service
{
    public class FileCreateInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public FileStorageDbContext FileStorageDbContext { get; set; }
        public WebApiDbContext WebApiDbContext { get; set; }
        public MigrationSettings MigrationSettings { get; set; }
        public SHA256Managed Sha { get; set; }
        public bool IsReplaceRequired { get; set; }
    }



    public static class FileCreatorService
    {
        public static async Task<string> CreateAsync(FileCreateInfo f)
        {
            var fileStorageContext = f.FileStorageDbContext;
            var filePath = f.FilePath;
            var fileName = f.FileName;
            var fileNameWithoutExtension = fileName.Split('.').First();
            var sha = f.Sha;
            var migrationSettings = f.MigrationSettings;

            Console.WriteLine(filePath);

            var dtn = DateTime.Now;
            var fileInfo = new FileInfo(filePath);
            var uuid = Guid.NewGuid();

            var shaHash = sha.ComputeHash(Encoding.UTF8.GetBytes($"{Guid.NewGuid()}{Guid.NewGuid()}"));
            var hash = Hex.ToHexString(shaHash);

            if (Guid.TryParse(fileNameWithoutExtension, out var guid))
                uuid = guid;

            var fileInfoEntity = new FileInfoEntity()
            {
                Uuid = uuid,
                Size = fileInfo.Length,
                StorageSize = fileInfo.Length,
                Hash = hash
            };
            fileStorageContext.FileInfoEntities.Add(fileInfoEntity);
            fileStorageContext.SaveChanges();

            var driveItemEntity = new DriveItemEntity()
            {
                CreateDate = dtn,
                ModifyDate = dtn,
                FullPath = Path.Combine(migrationSettings.DirectoryFullPath, fileName),
                Name = fileName,
                FileInfoId = fileInfoEntity.Id,
                Extension = fileInfo.Extension.Replace(".", ""),

                DirectoryId = migrationSettings.DestinationDirectoryId,
                DriveId = migrationSettings.DestinationDriveId,
                CreateById = migrationSettings.AdminUserId,
                ModifyById = migrationSettings.AdminUserId
            };
            fileStorageContext.DriveItemEntities.Add(driveItemEntity);
            fileStorageContext.SaveChanges();

            var fileExt = new FileExt()
            {
                SedkpIdentity = fileNameWithoutExtension,
                FileName = fileName,
                FullPath = filePath,
                FileItemId = driveItemEntity.Id
            };

            //webApiContext.FilesExt.Add(fileExt);
            //webApiContext.SaveChanges();

            return fileInfoEntity.Uuid.ToString();
        }
    }
}
