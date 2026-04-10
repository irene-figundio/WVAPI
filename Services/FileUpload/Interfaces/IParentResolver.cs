using AI_Integration.Model.FileUpload;

namespace AI_Integration.Services.FileUpload.Interfaces
{
    public interface IParentResolver
    {
        Task<(int Id, Guid Guid, bool Success, string? Error)> ResolveParentAsync(FileUploadRequest request);
    }
}
