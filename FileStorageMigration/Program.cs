using FileStorageMigration.Model.Options;
using FileStorageMigration.Service;
using FileStorageMigration.Service.FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

namespace FileStorageMigration
{
    class Program
    {
        private static DependencyInjectionContainer dic;

        private static void DICCreate()
        {
            dic = new DependencyInjectionContainer();
            var services = dic.Services;

            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false)
                    .Build();

            services.AddSingleton(configuration);

            services.Configure<MigrationOptions>(configuration.GetSection("MigrationOptions"));
            services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));

            services.AddSingleton<FileStorageMigrationService>();
            services.AddSingleton<FileStorageService>();
            services.AddSingleton<FileCreatorService>();
            services.AddSingleton<WebApiService>();

            dic.Create();
        }

        static async Task Main(string[] args)
        {
            DICCreate();

            var fileStorageMigration = dic.Get<FileStorageMigrationService>();

            await fileStorageMigration.StartAsync();
        }
    }
}
