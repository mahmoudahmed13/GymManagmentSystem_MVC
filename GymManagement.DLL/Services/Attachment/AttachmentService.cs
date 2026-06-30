using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GymManagement.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public bool Delete(string fileName, string folderName)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            try
            {
                if (!File.Exists(fullPath)) return false;

                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"File Deletion Failed: {fileName}.");
                return false;
            }
        }

        public (Stream stream, string contentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName)) return null;
            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            if (!File.Exists(fullPath)) return null;

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(fullPath).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream" //binary stream for unknown types
            };

            return (stream, contentType);
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream == null || fileStream.CanRead) return null;
            if (fileStream.Length == 0) return null;

            //2.Check the size — reject anything over 5 MB.
            if (fileStream.Length > _maxFileSize)
            {
                _logger.LogError($"File Rejected: File Too Large{fileStream.Length} Bytes.");
                return null;
            }

            //1.Check the extension — only.jpg.jpeg.png allowed.
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension))
            {
                _logger.LogError($"File Rejected: Invalid File Extension {extension}.");
                return null;
            }

            //3.Locate the folder & create it if missing
            var uploadsFolder = Path.Combine(_env.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadsFolder);

            //4.Generate a unique file name
            var storedFileName = $"{Guid.NewGuid()}{extension}";

            //5.Build the full file path.
            var filePath = Path.Combine(uploadsFolder, storedFileName);

            try
            {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(fs, ct);
                return storedFileName;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"File Upload Failed: {fileName}.");
                return null;
            }
        }
    }
}
