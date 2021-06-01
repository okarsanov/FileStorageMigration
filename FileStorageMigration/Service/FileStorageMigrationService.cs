using FileStorageMigration.Context;
using FileStorageMigration.Model;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FileStorageMigration.Service
{
    public class FileStorageMigrationService
    {
        MigrationSettings _migrationSettings;
        ConnectionStrings _connectionStrings;
        public FileStorageMigrationService(
            MigrationSettings migrationSettings,
            ConnectionStrings connectionStrings
            )
        {
            Console.WriteLine(migrationSettings.SourcePath);
            Console.WriteLine(migrationSettings.DestinationPath);
            Console.WriteLine(connectionStrings.FileStorageDataContext);

            _migrationSettings = migrationSettings;
            _connectionStrings = connectionStrings;
        }

        public async Task StartAsync()
        {
            using var fileStorageContext = new FileStorageDbContext(_connectionStrings.FileStorageDataContext);
            using var webApiContext = new WebApiDbContext(_connectionStrings.WebApiDataContext);

            Console.WriteLine("");
            Console.WriteLine("Files:");

            var sha = new SHA256Managed();

            var isReplaceRequired = !_migrationSettings.SourcePath.Equals(_migrationSettings.DestinationPath);

            await SearchFilesAsync(_migrationSettings.SourcePath, async (filePath, fileName) =>
            {
                var uuid = await FileCreatorService.CreateAsync(new FileCreateInfo()
                {
                    FileName = fileName,
                    FilePath = filePath,
                    Sha = sha,
                    FileStorageDbContext = fileStorageContext,
                    MigrationSettings = _migrationSettings,
                    WebApiDbContext = webApiContext
                });

                if (isReplaceRequired)
                {
                    File.Copy(filePath, Path.Combine(_migrationSettings.DestinationPath, uuid));
                    File.Delete(filePath);
                }
                else
                {
                    File.Move(filePath, Path.Combine(_migrationSettings.DestinationPath, uuid));
                }

            });
        }

        async Task SearchFilesAsync(string rootPath, Action<string, string> action)
        {
            foreach(var file in Directory.GetFiles(rootPath))
            {
                action(file, file.Split("\\").Last());
            }

            foreach(var dir in Directory.GetDirectories(rootPath))
            {
                await SearchFilesAsync(Path.Combine(rootPath, dir), action);
            }
        }
    }
}
