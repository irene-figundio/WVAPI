using AI_Integration.Model;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using AI_Integration.DataAccess.Database.Models;
namespace AI_Integration.Helpers
{
    public class VideoHelper
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VideoHelper(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            return baseUrl;
        }
       
        public Video GetRandomVideo(WebAPILog log)
        {
            var videosDirectory = Path.Combine(_environment.WebRootPath, "videos");

            if (!Directory.Exists(videosDirectory))
            {
                throw new DirectoryNotFoundException("Video directory not found.");
            }

            var videoFiles = Directory.GetFiles(videosDirectory);

            if (videoFiles.Length == 0)
            {
                throw new InvalidOperationException("No videos found in the directory.");
            }

            var random = new Random();
            var randomIndex = random.Next(0, videoFiles.Length);

            var randomVideoFile = videoFiles[randomIndex];

            // Ottieni l'indirizzo IP o il nome del dominio del server
            var serverBaseUrl = _httpContextAccessor.HttpContext.Request.Host.Value;

            // Costruisci l'URL completo per il video
            var videoRelativePath = $"/videos/{Path.GetFileName(randomVideoFile)}";
            var videoUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{serverBaseUrl}{videoRelativePath}";

            var randomVideo = new Video
            {
                Id = Path.GetFileNameWithoutExtension(randomVideoFile), // Assumi che il nome del file sia l'ID del video
                Title = Path.GetFileNameWithoutExtension(randomVideoFile), // Usa il nome del file come titolo del video
                Url = videoUrl // URL completo per il video
            };
            log.ResponseBody = randomVideo.ToString();
            return randomVideo;
        }

        public Video GetRandomLandscapeVideo()
        {
            var videosDirectory = Path.Combine(_environment.WebRootPath, "videosLand");

            if (!Directory.Exists(videosDirectory))
            {
                throw new DirectoryNotFoundException("Video directory not found.");
            }

            var videoFiles = Directory.GetFiles(videosDirectory);

            if (videoFiles.Length == 0)
            {
                throw new InvalidOperationException("No videos found in the directory.");
            }

            var random = new Random();
            var randomIndex = random.Next(0, videoFiles.Length);

            var randomVideoFile = videoFiles[randomIndex];

            // Ottieni l'indirizzo IP o il nome del dominio del server
            var serverBaseUrl = _httpContextAccessor.HttpContext.Request.Host.Value;

            // Costruisci l'URL completo per il video
            var videoRelativePath = $"/videosLand/{Path.GetFileName(randomVideoFile)}";
            var videoUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{serverBaseUrl}{videoRelativePath}";

            var randomVideo = new Video
            {
                Id = Path.GetFileNameWithoutExtension(randomVideoFile), // Assumi che il nome del file sia l'ID del video
                Title = Path.GetFileNameWithoutExtension(randomVideoFile), // Usa il nome del file come titolo del video
                Url = videoUrl // URL completo per il video
            };

            return randomVideo;
        }

        public string BuildPublicUrl(string fileName, bool isLandscape)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;
            var req = _httpContextAccessor.HttpContext.Request;
            var segment = isLandscape ? "landscape" : "portrait";
            var rel = $"/Media/{segment}/{Uri.EscapeDataString(fileName)}";
            return $"{req.Scheme}://{req.Host}{req.PathBase}{rel}";
        }




    }
}
