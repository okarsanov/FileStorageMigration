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
            var isReplaceRequired = ! Path.Combine(_migrationOptions.StoreSourceRootPath, 
                _migrationOptions.SourceDirectoryName).Equals(_migrationOptions.DestinationPath);

            var countFile = 1;

            await SearchFilesAsync(
                _migrationOptions.StoreSourceRootPath,
                _migrationOptions.SourceDirectoryName,
                null,
                async (filePath, fileName, driveItemEntity) =>
            {
                Console.Clear();
                Console.WriteLine($"{filePath}, {fileName}, [{countFile}]");

                var uuid = await _fileCreatorService.CreateAsync(new FileCreateInfo()
                {
                    FileName = fileName,
                    FilePath = filePath,
                    Directory = driveItemEntity
                });

                if (isReplaceRequired)
                {
                    File.Copy(filePath, Path.Combine(_migrationOptions.DestinationPath, uuid));
                    File.Delete(filePath);
                }
                else
                {
                    File.Move(filePath, Path.Combine(_migrationOptions.DestinationPath, uuid));
                }
            });
        }

        async Task SearchFilesAsync(
            string rootDirPath,
            string directoryName,
            DriveItemEntity parentDirectory,
            Func<string, string, DriveItemEntity, Task> action)
        {
            var directory = new DriveItemEntity();
            var rootDirectoryFullPath = Path.Combine(rootDirPath, directoryName);
            var directoryFullPath = Path.Combine(parentDirectory?.FullPath ?? string.Empty, directoryName, "/");

            if (!_directoryDictionary.TryGetValue(directoryFullPath, out directory))
            {
                directory = await _fileStorageService.GetDirectoryByFullPathAsync(directoryFullPath);
                if (directory == null)
                {
                    directory = await _fileStorageService.CreateDirectoryAsync(directoryName, parentDirectory);
                    _directoryDictionary.Add(directory.FullPath, directory);
                }
            }

            foreach(var file in Directory.GetFiles(rootDirectoryFullPath))
            {
                await action(file, file.Split("\\").Last(), directory);
            }

            foreach(var dir in Directory.GetDirectories(rootDirectoryFullPath))
            {
                await SearchFilesAsync(rootDirectoryFullPath, dir, directory, action);
            }
        }
    }
}
