using FileStorageMigration.Context;
using FileStorageMigration.Entities.FileStorage;
using FileStorageMigration.Model.Options;
using Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileStorageMigration.Service.FileStorage
{
    public class FileStorageService
    {
        private readonly FileStorageDbContext _fileStorageDbContext;
        private readonly MigrationOptions _migrationOptions;

        readonly SHA256Managed _sha = new SHA256Managed();

        public FileStorageService(
            IOptions<MigrationOptions> migrationOptions,
            IOptions<ConnectionStrings> connectionStrings
            )
        {
            _migrationOptions = migrationOptions.Value;

            _fileStorageDbContext = new FileStorageDbContext(connectionStrings.Value.FileStorageDataContext);
            
        }

        public async Task<DriveItemEntity> GetDirectoryByFullPathAsync(string fullPath)
        {
            var item = await _fileStorageDbContext.DriveItemEntities
                .Where(x => x.FullPath.Equals(fullPath) && !x.FileInfoId.HasValue)
                .FirstOrDefaultAsync();

            return item;
        }

        public async Task<DriveItemEntity> GetDirectoryByIdAsync(int directoryId)
        {
            var item = await _fileStorageDbContext.DriveItemEntities
                .Where(x => x.Id == directoryId)
                .FirstOrDefaultAsync();

            return item;
        }

        public async Task<DriveItemEntity> CreateDirectoryAsync(string name, DriveItemEntity parentDirectory)
        {
            var dtn = DateTime.Now;

            var fullPath = name + "\\";
            if (parentDirectory != null)
                fullPath = Path.Combine(parentDirectory.FullPath, name) + "\\";

            var directory = new DriveItemEntity()
            {
                Name = name,
                FullPath = fullPath,

                DriveId = _migrationOptions.DestinationDriveId,
                DirectoryId = parentDirectory?.Id ?? null,

                CreateById = _migrationOptions.AdminUserId,
                CreateDate = dtn,
                ModifyById = _migrationOptions.AdminUserId,
                ModifyDate = dtn,
                
            };

            await _fileStorageDbContext.DriveItemEntities.AddAsync(directory);
            await _fileStorageDbContext.SaveChangesAsync();

            return directory;
        }

        public async Task<DriveItemEntity> CreateFileAsync(
            Guid uuid,
            FileInfo fileInfo, 
            DriveItemEntity parentDirectory
            )
        {
            var dtn = DateTime.Now;

            var shaHash = _sha.ComputeHash(Encoding.UTF8.GetBytes($"{Guid.NewGuid()}{Guid.NewGuid()}"));
            var hash = Hex.ToHexString(shaHash);

            var fileInfoEntity = new FileInfoEntity()
            {
                Uuid = uuid,
                Size = fileInfo.Length,
                StorageSize = fileInfo.Length,
                Hash = hash
            };

            await _fileStorageDbContext.FileInfoEntities.AddAsync(fileInfoEntity);
            await _fileStorageDbContext.SaveChangesAsync();

            var name = fileInfo.Name;

            var fullPath = name + "\\";
            if (parentDirectory != null)
                fullPath = Path.Combine(parentDirectory.FullPath, name) + "\\";

            var file = new DriveItemEntity()
            {
                Name = name,
                FullPath = fullPath,
                Extension = fileInfo.Extension.Replace(".", ""),
                FileInfoId = fileInfoEntity.Id,

                DriveId = _migrationOptions.DestinationDriveId,
                DirectoryId = parentDirectory?.Id ?? null,

                CreateById = _migrationOptions.AdminUserId,
                CreateDate = dtn,
                ModifyById = _migrationOptions.AdminUserId,
                ModifyDate = dtn
            };

            await _fileStorageDbContext.DriveItemEntities.AddAsync(file);
            await _fileStorageDbContext.SaveChangesAsync();

            return file;
        }
    }
}
