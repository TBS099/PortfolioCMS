namespace PortfolioCMS.DTOs.Dashboard
{
    public class DashboardDTO
    {
        public SectionStatusDTO Hero { get; set; } = new();
        public SectionStatusDTO About { get; set; } = new();
        public MultiSectionStatusDTO Experience { get; set; } = new();
        public MultiSectionStatusDTO Projects { get; set; } = new();
        public MultiSectionStatusDTO SocialLinks { get; set; } = new();
        public FilesSectionDTO Files { get; set; } = new();
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

    public class FilesSectionDTO
    {
        public int TotalCount { get; set; }
        public Dictionary<string, int> CountByCategory { get; set; } = new();
        public DateTime? LastUploadedAt { get; set; }
    }
}