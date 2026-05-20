namespace PortfolioCMS.Models
{
    public class About
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Body { get; set; }
        public string? ImageUrl { get; set; }
        public string? Tagline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
