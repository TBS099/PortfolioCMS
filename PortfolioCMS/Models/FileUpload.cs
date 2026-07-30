namespace PortfolioCMS.Models
{
    public class FileUpload
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string OriginalName { get; set; }
        public required string FilePath { get; set; }
        public required string ContentType { get; set; }
        public required string Category { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}