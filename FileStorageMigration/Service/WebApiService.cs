using FileStorageMigration.Context;
using FileStorageMigration.Entities.FileStorage;
using FileStorageMigration.Entities.WebApi;
using FileStorageMigration.Model.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FileStorageMigration.Service.FileStorage
{
    public class WebApiService
    {
        private readonly string _connectionString;

        readonly SHA256Managed _sha = new SHA256Managed();

        public WebApiService(
            IOptions<ConnectionStrings> connectionStrings
            )
        {
            _connectionString = connectionStrings.Value.WebApiDataContext;
        }

        public async Task<FileExt> GetFileExtBySedkpIdentityAsync(string sedkpIdentity)
        {
            using var webApiDbContext = new WebApiDbContext(_connectionString);

            var item = await webApiDbContext.FilesExt
                .Where(x => x.SedkpIdentity.Equals(sedkpIdentity))
                .FirstOrDefaultAsync();

            return item;
        }

        public async Task<FileExt> CreateFileExtAsync(
            string sedkpIdentity,
            DriveItemEntity driveItemEntity,
            string fileLink
            )
        {
            var fileExt = new FileExt()
            {
                SedkpIdentity = sedkpIdentity,
                DriveId = driveItemEntity.DriveId,
                FileItemId = driveItemEntity.Id,
                FileLink = fileLink,
                Hidden = false
            };

            using var webApiDbContext = new WebApiDbContext(_connectionString);
            await webApiDbContext.FilesExt.AddAsync(fileExt);
            await webApiDbContext.SaveChangesAsync();

            return fileExt;
        }
    }
}
