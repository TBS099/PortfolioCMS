namespace PortfolioCMS.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Slug { get; set; }
        public required string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LiveUrl { get; set; }
        public string? GithubUrl { get; set; }
        public required string Stack { get; set; }
        public bool IsFeatured { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
