using PortfolioCMS.Models;

namespace PortfolioCMS.Services.Interfaces
{
    public interface IHeroService
    {
        // Define the methods for CRUD operations
        Task<Hero?> GetHeroAsync();
        Task CreateHeroAsync(Hero hero);
        Task UpdateHeroAsync(Hero hero);
    }
}
