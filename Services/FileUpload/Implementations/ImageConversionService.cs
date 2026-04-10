using AI_Integration.Services.FileUpload.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace AI_Integration.Services.FileUpload.Implementations
{
    public class ImageConversionService : IImageConversionService
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public async Task<Stream> ConvertToPngAsync(Stream inputStream)
        {
            var outputStream = new MemoryStream();
            using (var image = await Image.LoadAsync(inputStream))
            {
                await image.SaveAsPngAsync(outputStream);
            }
            outputStream.Position = 0;
            return outputStream;
        }

        public bool IsImage(string contentType)
        {
            return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }
    }
}
