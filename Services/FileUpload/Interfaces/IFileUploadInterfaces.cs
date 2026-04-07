using AI_Integration.Model.FileUpload;
using Microsoft.AspNetCore.Http;

namespace AI_Integration.Services.FileUpload.Interfaces
{
    public interface IFileUploadService
    {
        Task<FileUploadResponse> UploadFileAsync(FileUploadRequest request);
    }

    public interface IStorageMappingService
    {
        Task<int> GetOrAddProgressiveNumberAsync(ParentType parentType, int parentId, string storageArea);
    }

    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(string folderPath, string fileName, Stream fileStream, bool overwrite = false);
        string GetPhysicalPath(params string[] pathParts);
        string GetVirtualUrl(params string[] pathParts);
    }

    public interface IImageConversionService
    {
        Task<Stream> ConvertToPngAsync(Stream inputStream);
        bool IsImage(string contentType);
    }

    public interface IProgressiveResolver
    {
        int ResolveNextFolderNumber(string basePath, string folderPrefix);
        int ResolveNextFileNumber(string folderPath, string filePrefix, string fileSuffix);
    }

    public interface IUploadNamingStrategy
    {
        bool CanHandle(UploadType uploadType);
        Task<(string FolderPath, string FileName, string StorageArea, bool Overwrite)> GetNamingInfoAsync(FileUploadRequest request, int progressiveN);
    }
}
