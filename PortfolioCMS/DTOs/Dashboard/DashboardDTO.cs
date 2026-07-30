namespace PortfolioCMS.DTOs.Dashboard
{
    public class DashboardDTO
    {
        public SectionStatusDTO Hero { get; set; } = new();
        public SectionStatusDTO About { get; set; } = new();
        public MultiSectionStatusDTO Experience { get; set; } = new();
        public MultiSectionStatusDTO Projects { get; set; } = new();
        public MultiSectionStatusDTO SocialLinks { get; set; } = new();
    }

    public class SectionStatusDTO
    {
        public bool IsConfigured { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MultiSectionStatusDTO
    {
        public int Count { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}