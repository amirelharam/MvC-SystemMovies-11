namespace MvC_SystemMovies.Services.Interfaces
{
   
    public interface IFileStorageService
    {
      
        Task<string> SaveImageAsync(IFormFile file);

        
        void DeleteImage(string? fileName);
    }
}
