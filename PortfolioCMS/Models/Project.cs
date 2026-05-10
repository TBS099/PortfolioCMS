namespace PortfolioCMS.Models
{
    public class Project
    {
        // Primary key
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LiveUrl { get; set; }
        public string? GithubUrl { get; set; }
        public string Stack { get; set; }
        public bool IsFeatured { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
