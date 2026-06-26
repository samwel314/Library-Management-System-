using LibraryManagement.Api.Services.Interfaces;

namespace LibraryManagement.Api.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public static readonly List<string> Allows = new List<string>()
        {
            ".jpg" ,
            ".png" ,
        };
        public  void DeleteImageAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            var webPath = _webHostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webPath, path.TrimStart('/'));
            if (!File.Exists(imagePath))
                return;
            File.Delete(imagePath);
        }

        public async Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellation)
        {
            var webPath = _webHostEnvironment.WebRootPath;
            var imagesPath = Path.Combine(webPath, "Images");
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(imagesPath, fileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream , cancellation);

            return $"/Images/{fileName}"; 
        }

    }
}
