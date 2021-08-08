using FileStorageMigration.Entities.FileStorage;
using FileStorageMigration.Helpers;
using FileStorageMigration.Model.Options;
using FileStorageMigration.Service.FileStorage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private int countFile = 1;

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
        void PrintOptions()
        {
            Console.WriteLine($"Options:{Environment.NewLine}{JsonConvert.SerializeObject(_migrationOptions)}");
            Console.WriteLine($"{Environment.NewLine}{JsonConvert.SerializeObject(_connectionStrings)}");
            Console.WriteLine("");
        }
        public async Task StartAsync()
        {
            PrintOptions();

            var isReplaceRequired = !_migrationOptions.AbsouluteSourceRootPath.Equals(_migrationOptions.AbsouluteDestinationRootPath);
            
            await SearchFilesAsync(
                _migrationOptions.AbsouluteSourceRootPath,
                _migrationOptions.RelativeDestinationDirectoryName,
                null,
                async (filePath, driveItemEntity) =>
            {
                Console.Clear();
                Console.WriteLine($"{filePath}, [{countFile}]");

                try
                {
                    var uuid = await _fileCreatorService.CreateAsync(new FileCreateInfo()
                    {
                        FilePath = filePath,
                        Directory = driveItemEntity
                    });

                    if (isReplaceRequired)
                        File.Copy(filePath, Path.Combine(_migrationOptions.AbsouluteDestinationRootPath, uuid));
                    else
                        File.Move(filePath, Path.Combine(_migrationOptions.AbsouluteDestinationRootPath, uuid));

                    if (_migrationOptions.IsRemoveSourceFiles && File.Exists(filePath))
                        File.Delete(filePath);

                    countFile++;
                }
                catch(Exception e)
                {
                    LoggerHelper.LogError($"Processing file '{filePath}'", e);
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
