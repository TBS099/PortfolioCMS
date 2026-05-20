namespace PortfolioCMS.Models
{
    public class Experience
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Organization { get; set; }
        public ExperienceType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
