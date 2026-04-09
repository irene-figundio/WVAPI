using AI_Integration.Services.FileUpload.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AI_Integration.Services.FileUpload.Implementations
{
    using Microsoft.AspNetCore.Hosting;

    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePhysicalPath;
        private readonly string _baseVirtualUrl;

        public FileStorageService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var configuredPath = configuration["FileStorage:BasePhysicalPath"];
            if (string.IsNullOrEmpty(configuredPath))
            {
                _basePhysicalPath = Path.Combine(environment.ContentRootPath, "VitinerarioImages");
            }
            else
            {
                _basePhysicalPath = configuredPath;
            }

            if (!Path.IsPathRooted(_basePhysicalPath))
            {
                _basePhysicalPath = Path.GetFullPath(_basePhysicalPath);
            }

            _baseVirtualUrl = configuration["FileStorage:BaseVirtualUrl"] ?? "/Images/";
        }

        public async Task<string> SaveFileAsync(string folderPath, string fileName, Stream fileStream, bool overwrite = false)
        {
            string fullDirectoryPath = Path.Combine(_basePhysicalPath, folderPath);
            if (!Directory.Exists(fullDirectoryPath))
            {
                Directory.CreateDirectory(fullDirectoryPath);
            }

            string fullFilePath = Path.Combine(fullDirectoryPath, fileName);

            if (File.Exists(fullFilePath))
            {
                if (overwrite)
                {
                    HandleOverwrite(fullDirectoryPath, fileName);
                }
                else
                {
                    // If not overwrite, we should have already calculated a new 'i',
                    // but as a safety measure, we handle it if needed.
                    // For this project, 'overwrite' is true for CoverImages and ProgramPdf.
                }
            }

            using (var fs = new FileStream(fullFilePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fs);
            }

            return fullFilePath;
        }

        private void HandleOverwrite(string directory, string fileName)
        {
            string oldPath = Path.Combine(directory, fileName);
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string oldFileName = $"OLD_{timestamp}_{fileName}";
            string newPath = Path.Combine(directory, oldFileName);

            // In case of collision (unlikely with timestamp), add extra suffix
            int counter = 1;
            while (File.Exists(newPath))
            {
                oldFileName = $"OLD_{timestamp}_{counter}_{fileName}";
                newPath = Path.Combine(directory, oldFileName);
                counter++;
            }

            File.Move(oldPath, newPath);
        }

        public string GetPhysicalPath(params string[] pathParts)
        {
            return Path.Combine(_basePhysicalPath, Path.Combine(pathParts));
        }

        public string GetVirtualUrl(params string[] pathParts)
        {
            string combined = string.Join("/", pathParts).Replace("\\", "/");
            return $"{_baseVirtualUrl.TrimEnd('/')}/{combined}";
        }
    }
}
