using AI_Integration.Services.FileUpload.Interfaces;
using System.Text.RegularExpressions;

namespace AI_Integration.Services.FileUpload.Implementations
{
    public class ProgressiveResolver : IProgressiveResolver
    {
        private static readonly object _lock = new object();

        public int ResolveNextFolderNumber(string basePath, string folderPrefix)
        {
            lock (_lock)
            {
                if (!Directory.Exists(basePath))
                {
                    return 1;
                }

                var directories = Directory.GetDirectories(basePath, $"{folderPrefix}*");
                int maxN = 0;

                foreach (var dir in directories)
                {
                    var name = Path.GetFileName(dir);
                    var match = Regex.Match(name, $@"{Regex.Escape(folderPrefix)}(\d+)");
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int n))
                    {
                        if (n > maxN) maxN = n;
                    }
                }

                return maxN + 1;
            }
        }

        public int ResolveNextFileNumber(string folderPath, string filePrefix, string fileSuffix)
        {
            lock (_lock)
            {
                if (!Directory.Exists(folderPath))
                {
                    return 1;
                }

                // Match files like: 1.png, image1.png, art5_photo3.png
                // Excluding OLD_*
                var files = Directory.GetFiles(folderPath)
                                     .Select(Path.GetFileName)
                                     .Where(f => !f!.StartsWith("OLD_", StringComparison.OrdinalIgnoreCase));

                int maxI = 0;
                // Pattern matches the last number before the extension
                string pattern = $@"{Regex.Escape(filePrefix)}(\d+){Regex.Escape(fileSuffix)}\.[^.]+$";

                foreach (var file in files)
                {
                    var match = Regex.Match(file!, pattern);
                    if (match.Success && int.TryParse(match.Groups[1].Value, out int i))
                    {
                        if (i > maxI) maxI = i;
                    }
                    else
                    {
                        // Fallback for simple numeric names like 1.png
                        if (string.IsNullOrEmpty(filePrefix) && string.IsNullOrEmpty(fileSuffix))
                        {
                             var simpleMatch = Regex.Match(file!, @"^(\d+)\.[^.]+$");
                             if (simpleMatch.Success && int.TryParse(simpleMatch.Groups[1].Value, out int si))
                             {
                                 if (si > maxI) maxI = si;
                             }
                        }
                    }
                }

                return maxI + 1;
            }
        }
    }
}
