namespace LibraryManagement.Api.Services.Interfaces
{
    public interface IUserActivityLogService
    {
        Task LogAsync(string action, CancellationToken cancellation = default , bool saveChanges = false); 
    }
}
