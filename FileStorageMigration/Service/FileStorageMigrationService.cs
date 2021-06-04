using FileStorageMigration.Entities.FileStorage;
using FileStorageMigration.Model.Options;
using FileStorageMigration.Service.FileStorage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileStorageMigration.Service
{

    public class FileStorageMigrationService
    {
        private Dictionary<string, DriveItemEntity> _directoryDictionary = new Dictionary<string, DriveItemEntity>();

        private readonly MigrationOptions _migrationOptions;
        private readonly ConnectionStrings _connectionStrings;
        private readonly FileCreatorService _fileCreatorService;
        private readonly FileStorageService _fileStorageService;

        public FileStorageMigrationService(
            IOptions<MigrationOptions> migrationOptions,
            IOptions<ConnectionStrings> connectionStrings,
            FileCreatorService fileCreatorService,
            FileStorageService fileStorageService
            )
        {
            _migrationOptions = migrationOptions.Value;
            _connectionStrings = connectionStrings.Value;
            _fileCreatorService = fileCreatorService;
            _fileStorageService = fileStorageService;
        }

        public async Task StartAsync()
        {
            var isReplaceRequired = !_migrationOptions.AbsouluteSourceRootPath.Equals(_migrationOptions.AbsouluteDestinationRootPath);
            
            var countFile = 1;

            await SearchFilesAsync(
                _migrationOptions.AbsouluteSourceRootPath,
                _migrationOptions.RelativeDestinationDirectoryName,
                null,
                async (filePath, driveItemEntity) =>
            {
                Console.Clear();
                Console.WriteLine($"{filePath}, [{countFile}]");

                var uuid = await _fileCreatorService.CreateAsync(new FileCreateInfo()
                {
                    FilePath = filePath,
                    Directory = driveItemEntity
                });

                if (isReplaceRequired)
                {
                    File.Copy(filePath, Path.Combine(_migrationOptions.AbsouluteDestinationRootPath, uuid));
                    //File.Delete(filePath);
                }
                else
                {
                    File.Move(filePath, Path.Combine(_migrationOptions.AbsouluteDestinationRootPath, uuid));
                }
            });
        }

        async Task SearchFilesAsync(
            string rootSourceDirPath,
            string directoryName,
            DriveItemEntity parentDirectory,
            Func<string, DriveItemEntity, Task> action)
        {
            var directory = new DriveItemEntity();

            var directoryFullPath = $"{directoryName}\\";
            if (parentDirectory != null)
                directoryFullPath = Path.Combine(parentDirectory.FullPath, directoryName) + "\\";

            directory = await _fileStorageService.GetDirectoryByFullPathAsync(directoryFullPath);
            if (directory == null)
                directory = await _fileStorageService.CreateDirectoryAsync(directoryName, parentDirectory);

            foreach (var file in Directory.GetFiles(rootSourceDirPath))
            {
                await action(file, directory);
            }

            foreach(var dirFullPath in Directory.GetDirectories(rootSourceDirPath))
            {
                var dirName = dirFullPath.Split(Path.DirectorySeparatorChar).Last();

                await SearchFilesAsync(dirFullPath, dirName, directory, action);
            }
        }
    }
}
