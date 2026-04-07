using AI_Integration.Model.FileUpload;
using AI_Integration.Services.FileUpload.Interfaces;

namespace AI_Integration.Services.FileUpload.Implementations
{
    public class EventNamingStrategy : IUploadNamingStrategy
    {
        private readonly IProgressiveResolver _progressiveResolver;
        private readonly IFileStorageService _fileStorageService;

        public EventNamingStrategy(IProgressiveResolver progressiveResolver, IFileStorageService fileStorageService)
        {
            _progressiveResolver = progressiveResolver;
            _fileStorageService = fileStorageService;
        }

        public bool CanHandle(UploadType uploadType)
        {
            return uploadType == UploadType.EventGalleryImage ||
                   uploadType == UploadType.EventStageImage ||
                   uploadType == UploadType.EventProgramPdf ||
                   uploadType == UploadType.EventCoverImage;
        }

        public Task<(string FolderPath, string FileName, string StorageArea, bool Overwrite)> GetNamingInfoAsync(FileUploadRequest request, int progressiveN)
        {
            string folderName = $"Evento{progressiveN}";
            string folderPath;
            string fileName;
            string storageArea;
            bool overwrite = false;

            switch (request.UploadType)
            {
                case UploadType.EventGalleryImage:
                    storageArea = "Events";
                    folderPath = Path.Combine(storageArea, folderName);
                    int i = _progressiveResolver.ResolveNextFileNumber(_fileStorageService.GetPhysicalPath(folderPath), "", "");
                    fileName = $"{i}.png";
                    break;

                case UploadType.EventStageImage:
                    storageArea = "Events";
                    folderPath = Path.Combine(storageArea, folderName);
                    int si = _progressiveResolver.ResolveNextFileNumber(_fileStorageService.GetPhysicalPath(folderPath), "image", "");
                    fileName = $"image{si}.png";
                    break;

                case UploadType.EventProgramPdf:
                    storageArea = "ProgramPdf";
                    folderPath = storageArea;
                    fileName = $"event{progressiveN}.pdf";
                    overwrite = true;
                    break;

                case UploadType.EventCoverImage:
                    storageArea = "Events";
                    folderPath = Path.Combine(storageArea, folderName);
                    fileName = "locandina.png";
                    overwrite = true;
                    break;

                default:
                    throw new ArgumentException("Invalid upload type for Event strategy");
            }

            return Task.FromResult((folderPath, fileName, storageArea, overwrite));
        }
    }

    public class ContentNamingStrategy : IUploadNamingStrategy
    {
        private readonly IProgressiveResolver _progressiveResolver;
        private readonly IFileStorageService _fileStorageService;

        public ContentNamingStrategy(IProgressiveResolver progressiveResolver, IFileStorageService fileStorageService)
        {
            _progressiveResolver = progressiveResolver;
            _fileStorageService = fileStorageService;
        }

        public bool CanHandle(UploadType uploadType)
        {
            return uploadType == UploadType.ContentGalleryImage ||
                   uploadType == UploadType.ContentCoverImage;
        }

        public Task<(string FolderPath, string FileName, string StorageArea, bool Overwrite)> GetNamingInfoAsync(FileUploadRequest request, int progressiveN)
        {
            string folderName = $"Art{progressiveN}";
            string storageArea = "Articles";
            string folderPath = Path.Combine(storageArea, folderName);
            string fileName;
            bool overwrite = false;

            switch (request.UploadType)
            {
                case UploadType.ContentGalleryImage:
                    int i = _progressiveResolver.ResolveNextFileNumber(_fileStorageService.GetPhysicalPath(folderPath), $"art{progressiveN}_photo", "");
                    fileName = $"art{progressiveN}_photo{i}.png";
                    break;

                case UploadType.ContentCoverImage:
                    fileName = $"art{progressiveN}_min.png";
                    overwrite = true;
                    break;

                default:
                    throw new ArgumentException("Invalid upload type for Content strategy");
            }

            return Task.FromResult((folderPath, fileName, storageArea, overwrite));
        }
    }

    public class HeroNamingStrategy : IUploadNamingStrategy
    {
        private readonly IProgressiveResolver _progressiveResolver;
        private readonly IFileStorageService _fileStorageService;

        public HeroNamingStrategy(IProgressiveResolver progressiveResolver, IFileStorageService fileStorageService)
        {
            _progressiveResolver = progressiveResolver;
            _fileStorageService = fileStorageService;
        }

        public bool CanHandle(UploadType uploadType)
        {
            return uploadType == UploadType.HeroImage;
        }

        public Task<(string FolderPath, string FileName, string StorageArea, bool Overwrite)> GetNamingInfoAsync(FileUploadRequest request, int progressiveN)
        {
            string storageArea = "Hero";
            string folderPath = storageArea;
            int i = _progressiveResolver.ResolveNextFileNumber(_fileStorageService.GetPhysicalPath(folderPath), "hero", "");
            string fileName = $"hero{i}.png";

            return Task.FromResult((folderPath, fileName, storageArea, false));
        }
    }
}
