namespace PortfolioCMS.DTOs.SocialLink
{
    public class SocialLinkDTO
    {
        public Guid Id { get; set; }
        public required string Platform { get; set; }
        public required string Url { get; set; }
    }
}
