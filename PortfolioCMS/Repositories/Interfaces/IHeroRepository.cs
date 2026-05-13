using PortfolioCMS.Models;

namespace PortfolioCMS.Repositories.Interfaces
{
    public interface IHeroRepository
    {
        // Define the methods for CRUD operations
        Task<Hero?> GetHeroAsync();
        Task CreateHeroAsync(Hero hero);
        Task UpdateHeroAsync(Hero hero);
    }
}
