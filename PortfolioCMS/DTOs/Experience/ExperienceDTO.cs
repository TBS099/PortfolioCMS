using PortfolioCMS.Models;

namespace PortfolioCMS.DTOs.Experience
{
    public class ExperienceDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Organization { get; set; }
        public required ExperienceType Type { get; set; }
        public required DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
    }
}
