namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(IFormFile file, CancellationToken cancellation);

        void DeleteImage(string path);
    }
}
