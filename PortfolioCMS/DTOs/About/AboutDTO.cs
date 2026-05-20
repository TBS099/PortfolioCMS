namespace PortfolioCMS.DTOs.About
{
    public class AboutDTO
    {
        public required string Title { get; set; }
        public required string Body { get; set; }
        public string? ImageUrl { get; set; }
        public string? Tagline { get; set; }
    }
}
