using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Model.FileUpload;
using AI_Integration.Services.FileUpload.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AI_Integration.Services.FileUpload.Implementations
{
    public class ParentResolver : IParentResolver
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParentResolver(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(int Id, Guid Guid, bool Success, string? Error)> ResolveParentAsync(FileUploadRequest request)
        {
            if (request.ParentType == ParentType.HeroImage)
            {
                return (0, Guid.Empty, true, null);
            }

            if (request.ParentId == null && request.ParentGuid == null)
            {
                return (0, Guid.Empty, false, "At least one of ParentId or ParentGuid must be provided.");
            }

            int resolvedId = 0;
            Guid resolvedGuid = Guid.Empty;

            if (request.ParentType == ParentType.Events)
            {
                Event? ev = null;
                if (request.ParentGuid != null)
                {
                    ev = await _unitOfWork.Query<Event>().FirstOrDefaultAsync(e => e.Guid == request.ParentGuid);
                }
                else if (request.ParentId != null)
                {
                    ev = await _unitOfWork.Events.GetByIdAsync(request.ParentId.Value);
                }

                if (ev == null) return (0, Guid.Empty, false, "Event not found.");

                resolvedId = ev.Id;
                resolvedGuid = ev.Guid;
            }
            else if (request.ParentType == ParentType.Content)
            {
                Content? content = null;
                if (request.ParentGuid != null)
                {
                    content = await _unitOfWork.Query<Content>().FirstOrDefaultAsync(c => c.Guid == request.ParentGuid);
                }
                else if (request.ParentId != null)
                {
                    content = await _unitOfWork.Contents.GetByIdAsync(request.ParentId.Value);
                }

                if (content == null) return (0, Guid.Empty, false, "Content not found.");

                resolvedId = content.Id;
                resolvedGuid = content.Guid;
            }
            else if (request.ParentType == ParentType.Podcast)
            {
                var podcast = await _unitOfWork.Repository<Podcast>().GetByIdAsync(request.ParentId.Value);
                if (podcast == null) return (0, Guid.Empty, false, "Podcast not found.");
                resolvedId = podcast.Id;
                // Podcast doesn't have GUID yet, so we return empty or skip validation
            }

            // Validation: if both provided, they must match
            if (request.ParentId != null && request.ParentId != resolvedId)
            {
                return (0, Guid.Empty, false, $"Inconsistency: Provided ParentId {request.ParentId} does not match record found by Guid.");
            }
            if (request.ParentGuid != null && request.ParentGuid != resolvedGuid)
            {
                return (0, Guid.Empty, false, $"Inconsistency: Provided ParentGuid {request.ParentGuid} does not match record found by Id.");
            }

            return (resolvedId, resolvedGuid, true, null);
        }
    }
}
