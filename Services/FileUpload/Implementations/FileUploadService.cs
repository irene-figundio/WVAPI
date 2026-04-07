using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Model.FileUpload;
using AI_Integration.Services.FileUpload.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AI_Integration.Services.FileUpload.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<IUploadNamingStrategy> _namingStrategies;
        private readonly IStorageMappingService _storageMappingService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IImageConversionService _imageConversionService;

        public FileUploadService(
            IUnitOfWork unitOfWork,
            IEnumerable<IUploadNamingStrategy> namingStrategies,
            IStorageMappingService storageMappingService,
            IFileStorageService fileStorageService,
            IImageConversionService imageConversionService)
        {
            _unitOfWork = unitOfWork;
            _namingStrategies = namingStrategies;
            _storageMappingService = storageMappingService;
            _fileStorageService = fileStorageService;
            _imageConversionService = imageConversionService;
        }

        public async Task<FileUploadResponse> UploadFileAsync(FileUploadRequest request)
        {
            // 1. Validate Parent
            var parentValidated = await ValidateParentAsync(request);
            if (!parentValidated.Success) return parentValidated;

            // 2. Identify Strategy
            var strategy = _namingStrategies.FirstOrDefault(s => s.CanHandle(request.UploadType));
            if (strategy == null) return new FileUploadResponse { Success = false, Message = "No naming strategy found for this upload type." };

            // 3. Get Mapping (N)
            // HeroImages don't have a parent in the same way, but we use ParentId 0 and Type HeroImage for mapping if needed,
            // although the requirement says they are independent. Let's use a dummy N=0 for Hero if not needed.
            int progressiveN = 0;
            if (request.ParentType != ParentType.HeroImage)
            {
                // We need to know which storage area to use for mapping.
                // Let's peek at the strategy or define it. Most are Events or Articles.
                string mappingArea = request.ParentType == ParentType.Events ? "Events" : "Articles";
                progressiveN = await _storageMappingService.GetOrAddProgressiveNumberAsync(request.ParentType, request.ParentId, mappingArea);
            }

            // 4. Determine naming
            var namingInfo = await strategy.GetNamingInfoAsync(request, progressiveN);

            // 5. Process File (Convert if image)
            Stream finalStream;
            string finalFileName = namingInfo.FileName;

            if (_imageConversionService.IsImage(request.File.ContentType) && request.UploadType != UploadType.EventProgramPdf)
            {
                finalStream = await _imageConversionService.ConvertToPngAsync(request.File.OpenReadStream());
                // Ensure extension is .png
                finalFileName = Path.ChangeExtension(finalFileName, ".png");
            }
            else
            {
                if (request.UploadType == UploadType.EventProgramPdf && !request.File.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return new FileUploadResponse { Success = false, Message = "Only PDF files are allowed for Program PDF." };
                }
                finalStream = request.File.OpenReadStream();
            }

            // 6. Save Physical File
            string physicalPath = await _fileStorageService.SaveFileAsync(namingInfo.FolderPath, finalFileName, finalStream, namingInfo.Overwrite);
            string virtualUrl = _fileStorageService.GetVirtualUrl(namingInfo.FolderPath, finalFileName);

            // 7. Update DB
            await UpdateDatabaseAsync(request, virtualUrl, finalFileName);

            return new FileUploadResponse
            {
                Success = true,
                Message = "File uploaded successfully",
                FileUrl = virtualUrl,
                PhysicalPath = physicalPath
            };
        }

        private async Task<FileUploadResponse> ValidateParentAsync(FileUploadRequest request)
        {
            if (request.ParentType == ParentType.HeroImage) return new FileUploadResponse { Success = true };

            if (request.ParentType == ParentType.Events)
            {
                var ev = await _unitOfWork.Events.GetByIdAsync(request.ParentId);
                if (ev == null) return new FileUploadResponse { Success = false, Message = "Event not found" };

                if (request.UploadType == UploadType.EventStageImage)
                {
                    if (ev.CategoryId != 3 && ev.CategoryId != 4)
                        return new FileUploadResponse { Success = false, Message = "Stage images only allowed for Category 3 or 4" };

                    if (request.TripId == null || request.DayNumber == null)
                        return new FileUploadResponse { Success = false, Message = "TripId and DayNumber are required for Stage images" };
                }
            }
            else if (request.ParentType == ParentType.Content)
            {
                var content = await _unitOfWork.Contents.GetByIdAsync(request.ParentId);
                if (content == null) return new FileUploadResponse { Success = false, Message = "Content not found" };
            }

            return new FileUploadResponse { Success = true };
        }

        private async Task UpdateDatabaseAsync(FileUploadRequest request, string url, string fileName)
        {
            switch (request.UploadType)
            {
                case UploadType.EventGalleryImage:
                    var evG = await _unitOfWork.Events.GetByIdAsync(request.ParentId);
                    if (evG.GalleryId == null)
                    {
                        var gallery = new Gallery { EventId = request.ParentId, CreatedAt = DateTime.UtcNow };
                        await _unitOfWork.Galleries.InsertAsync(gallery);
                        await _unitOfWork.SaveChangesAsync();
                        evG.GalleryId = gallery.Id;
                        _unitOfWork.Events.Update(evG);
                    }
                    var photo = new PhotoGallery
                    {
                        GalleryId = evG.GalleryId.Value,
                        ImageUrl = url,
                        Caption = request.Caption,
                        LangID = request.LangId ?? 1,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.PhotoGalleries.InsertAsync(photo);
                    break;

                case UploadType.EventStageImage:
                    var day = await _unitOfWork.Query<ItineraryDay>()
                        .FirstOrDefaultAsync(d => d.TripId == request.TripId && d.DayNumber == request.DayNumber);
                    if (day != null)
                    {
                        // Requirement: max 3 images. Slot images pieno check?
                        if (string.IsNullOrEmpty(day.Image1)) day.Image1 = url;
                        else if (string.IsNullOrEmpty(day.Image2)) day.Image2 = url;
                        else if (string.IsNullOrEmpty(day.Image3)) day.Image3 = url;
                        else throw new Exception("Slot immagini pieno (max 3)");

                        _unitOfWork.Repository<ItineraryDay>().Update(day);
                    }
                    break;

                case UploadType.EventProgramPdf:
                    var evP = await _unitOfWork.Events.GetByIdAsync(request.ParentId);
                    evP.ProgramPdf = url;
                    _unitOfWork.Events.Update(evP);
                    break;

                case UploadType.EventCoverImage:
                    var evC = await _unitOfWork.Events.GetByIdAsync(request.ParentId);
                    evC.CoverImage = url;
                    _unitOfWork.Events.Update(evC);
                    break;

                case UploadType.ContentGalleryImage:
                    var match = Regex.Match(fileName, @"photo(\d+)\.png$");
                    int pos = match.Success ? int.Parse(match.Groups[1].Value) : 1;
                    var cImg = new ContentImage
                    {
                        ContentId = request.ParentId,
                        ImageUrl = url,
                        Caption = request.Caption,
                        Position = pos,
                        LangID = request.LangId ?? 1,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.ContentImages.InsertAsync(cImg);
                    break;

                case UploadType.ContentCoverImage:
                    var cont = await _unitOfWork.Contents.GetByIdAsync(request.ParentId);
                    cont.CoverImage = url;
                    _unitOfWork.Contents.Update(cont);
                    break;

                case UploadType.HeroImage:
                    var hero = new HeroImage
                    {
                        Name = request.Caption ?? fileName,
                        ImageUrl = url,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Repository<HeroImage>().InsertAsync(hero);
                    break;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
