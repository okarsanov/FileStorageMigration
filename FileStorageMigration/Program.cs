using FileStorageMigration.Model;
using FileStorageMigration.Service;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace FileStorageMigration
{
    class Program
    {
        static IConfiguration configuration;

        static async Task Main(string[] args)
        {
            configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false)
                    .Build();

            var migrationSettings = configuration.GetSection(nameof(MigrationSettings)).Get<MigrationSettings>();
            var connectionStrings = configuration.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>();

            var fileStorageMigration = new FileStorageMigrationService(migrationSettings, connectionStrings);

            await fileStorageMigration.StartAsync();
        }
    }
}
