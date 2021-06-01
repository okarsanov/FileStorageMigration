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
        private readonly WebApiDbContext _webApiDbContext;

        readonly SHA256Managed _sha = new SHA256Managed();

        public WebApiService(
            IOptions<ConnectionStrings> connectionStrings
            )
        {
            _webApiDbContext = new WebApiDbContext(connectionStrings.Value.WebApiDataContext);
        }

        public async Task<FileExt> GetFileExtBySedkpIdentityAsync(string sedkpIdentity)
        {
            var item = await _webApiDbContext.FilesExt
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

            await _webApiDbContext.FilesExt.AddAsync(fileExt);
            await _webApiDbContext.SaveChangesAsync();

            return fileExt;
        }
    }
}
