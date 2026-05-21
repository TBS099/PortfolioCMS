namespace PortfolioCMS.Models
{
    public class SocialLink
    {
        public Guid Id { get; set; }
        public required string Platform { get; set; }
        public required string Url { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
