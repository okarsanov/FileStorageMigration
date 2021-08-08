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
        public string FilePath { get; set; }
        public DriveItemEntity Directory { get; set; }
    }

    public class FileCreatorService
    {
        private readonly FileStorageService _fileStorageService;
        private readonly WebApiService _webApiService;
        private readonly PropertiesEncoderService _encoder;
        public FileCreatorService(
            FileStorageService fileStorageService,
            WebApiService webApiService,
            PropertiesEncoderService encoder
            )
        {
            _fileStorageService = fileStorageService;
            _webApiService = webApiService;
            _encoder = encoder;
        }

        public async Task<string> CreateAsync(FileCreateInfo f)
        {
            var fileInfo = new FileInfo(f.FilePath);
            var fileNameWithoutExtension = fileInfo.Name.Split('.').First();

            var uuid = Guid.TryParse(fileNameWithoutExtension, out var guid) ? guid : Guid.NewGuid();

            var file = await _fileStorageService.CreateFileAsync(uuid, fileInfo, f.Directory);

            //var link = CreateLink(file);
            var link = string.Empty;
            await _webApiService.CreateFileExtAsync(fileNameWithoutExtension, file, link);

            return uuid.ToString();
        }

        string CreateLink(DriveItemEntity driveItemEntity)
        {
            var properties = new Properties();

            var link = new LinkProperties(properties)
            {
                CheckPermission = false,
                Type = LinkType.ByItemIdAndDriveId,
                DriveId = driveItemEntity.DriveId,
                ItemId = driveItemEntity.Id,
                ShareLinkCounter = 1
            };

            return _encoder.Encode(properties);
        }
    }
}
