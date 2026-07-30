namespace PortfolioCMS.DTOs.FileUpload
{
    public class FileUploadDTO
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string OriginalName { get; set; }
        public required string FileUrl { get; set; }
        public required string Category { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}