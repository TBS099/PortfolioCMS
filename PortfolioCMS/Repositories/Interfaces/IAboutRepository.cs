using PortfolioCMS.Models;

namespace PortfolioCMS.Repositories.Interfaces
{
    public interface IAboutRepository
    {
        // Define the methods for CRUD operations
        public Task<About?> GetAboutAsync();
        public Task CreateAboutAsync(About about);
        public Task UpdateAboutAsync(About about);
    }
}
