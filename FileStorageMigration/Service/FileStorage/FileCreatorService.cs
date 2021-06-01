using FileStorageMigration.Entities.FileStorage;
using FileStorageMigration.Model.FileStorage;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileStorageMigration.Service.FileStorage
{
    public class FileCreateInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DriveItemEntity Directory { get; set; }
    }

    public class FileCreatorService
    {
        private readonly FileStorageService _fileStorageService;
        private readonly WebApiService _webApiService;

        public FileCreatorService(
            FileStorageService fileStorageService,
            WebApiService webApiService
            )
        {
            _fileStorageService = fileStorageService;
            _webApiService = webApiService;
        }

        public async Task<string> CreateAsync(FileCreateInfo f)
        {
            var fileNameWithoutExtension = f.FileName.Split('.').First();

            var fileInfo = new FileInfo(f.FilePath);

            var uuid = Guid.TryParse(fileNameWithoutExtension, out var guid) ? guid : Guid.NewGuid();

            var file = await _fileStorageService.CreateFileAsync(uuid, fileInfo, f.Directory);

            var link = CreateLink(file.DriveId, file.Id);
            await _webApiService.CreateFileExtAsync(fileNameWithoutExtension, file, link);

            return uuid.ToString();
        }

        string CreateLink(int driveId, int itemId)
        {
            var properties = new Properties();

            var link = new LinkProperties(properties)
            {
                CheckPermission = true,
                Type = LinkType.ByItemIdAndDriveId,
                DriveId = driveId,
                ItemId = itemId,
                ShareLinkCounter = 1
            };

            var encoder = new PropertiesEncoderService("test_key");
            return encoder.Encode(properties);
        }
    }
}
