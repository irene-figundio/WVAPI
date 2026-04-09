using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Model.FileUpload;
using AI_Integration.Services.FileUpload.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AI_Integration.Services.FileUpload.Implementations
{
    using Microsoft.AspNetCore.Hosting;

    public class StorageMappingService : IStorageMappingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProgressiveResolver _progressiveResolver;
        private readonly string _basePhysicalPath;

        public StorageMappingService(IUnitOfWork unitOfWork, IProgressiveResolver progressiveResolver, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _progressiveResolver = progressiveResolver;

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
        }

        public async Task<int> GetOrAddProgressiveNumberAsync(ParentType parentType, int parentId, string storageArea)
        {
            var parentTypeStr = parentType.ToString();

            var mapping = await _unitOfWork.Query<StorageMapping>()
                .FirstOrDefaultAsync(m => m.ParentType == parentTypeStr && m.ParentId == parentId && m.StorageArea == storageArea);

            if (mapping != null)
            {
                return mapping.ProgressiveNumber;
            }

            // Calculate new N
            string folderPrefix = storageArea == "Articles" ? "Art" : "Evento";
            string areaPath = Path.Combine(_basePhysicalPath, storageArea);

            int nextN = _progressiveResolver.ResolveNextFolderNumber(areaPath, folderPrefix);
            string folderName = $"{folderPrefix}{nextN}";

            mapping = new StorageMapping
            {
                ParentType = parentTypeStr,
                ParentId = parentId,
                StorageArea = storageArea,
                ProgressiveNumber = nextN,
                FolderName = folderName,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.InsertAsync(mapping);
            await _unitOfWork.SaveChangesAsync();

            return nextN;
        }
    }
}
