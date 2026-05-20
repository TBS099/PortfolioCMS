using PortfolioCMS.Models;

namespace PortfolioCMS.Services.Interfaces
{
    public interface IAboutService
    {
        // Define the methods for CRUD operations
        public Task<About?> GetAboutAsync();
        public Task CreateAboutAsync(About about);
        public Task UpdateAboutAsync(About about);

    }
}
