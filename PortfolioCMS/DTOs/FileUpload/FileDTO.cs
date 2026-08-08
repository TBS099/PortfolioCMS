namespace PortfolioCMS.DTOs.FileUpload
{
    public class FileDTO
    {
        public Guid Id { get; set; }
        public required string DisplayName { get; set; }
        public required string FileUrl { get; set; }
        public required string Category { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}