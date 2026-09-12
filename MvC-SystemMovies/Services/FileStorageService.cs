using MvC_SystemMovies.Services.Interfaces;

namespace MvC_SystemMovies.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; 

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("File is empty.");

            if (file.Length > MaxFileSizeBytes)
                throw new InvalidOperationException("File exceeds the maximum allowed size (5 MB).");

            var extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Unsupported file type. Allowed: jpg, jpeg, png, gif, webp.");

            var imagesFolder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(imagesFolder); 

          
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(imagesFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public void DeleteImage(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var imagesFolder = Path.Combine(_env.WebRootPath, "images");
            var filePath = Path.Combine(imagesFolder, fileName);

            var fullPath = Path.GetFullPath(filePath);
            if (!fullPath.StartsWith(Path.GetFullPath(imagesFolder), StringComparison.OrdinalIgnoreCase))
                return;

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
