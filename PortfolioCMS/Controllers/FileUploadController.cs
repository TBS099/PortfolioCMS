using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PortfolioCMS.Data;
using PortfolioCMS.DTOs.FileUpload;
using PortfolioCMS.Models;

namespace PortfolioCMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly string _uploadDirectory;

        public FileUploadController(AppDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
            _uploadDirectory = Path.Combine(_environment.ContentRootPath, "uploads", "files");
        }

        // Generate unique filename with suffix if duplicate exists
        private (string fileName, int? suffix) SanitizeFileName(string displayName, string extension)
        {
            // Lowercase and trim input
            var sanitized = displayName.Trim().ToLower();

            // Replace spaces with hyphens
            sanitized = sanitized.Replace(" ", "-");

            // Strip non-alphanumeric characters except hyphens and dots
            sanitized = new string(sanitized
                .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '.')
                .ToArray());

            // Remove leading/trailing hyphens
            sanitized = sanitized.Trim('-');

            // Fallback if empty
            if (string.IsNullOrWhiteSpace(sanitized))
                sanitized = "file";

            // Check for existing filename and append suffix if needed
            var uploadDir = Path.Combine(_uploadDirectory);
            var fileName = $"{sanitized}{extension}";
            int? suffix = null;
            var counter = 1;

            while (System.IO.File.Exists(Path.Combine(uploadDir, fileName)))
            {
                suffix = counter;
                fileName = $"{sanitized}-{counter}{extension}";
                counter++;
            }

            return (fileName, suffix);
        }

        // File signature validation by extension
        private static readonly Dictionary<string, Func<byte[], bool>> FileSignatureValidators = new()
        {
            [".pdf"] = b => b.Length >= 4 && b[0] == 0x25 && b[1] == 0x50 && b[2] == 0x44 && b[3] == 0x46, // %PDF
            [".jpg"] = b => b.Length >= 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF,
            [".jpeg"] = b => b.Length >= 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF,
            [".png"] = b => b.Length >= 8 &&
                b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47 &&
                b[4] == 0x0D && b[5] == 0x0A && b[6] == 0x1A && b[7] == 0x0A,
            [".gif"] = b => b.Length >= 4 && b[0] == 0x47 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x38, // GIF8
            [".webp"] = b => b.Length >= 12 &&
                b[0] == 0x52 && b[1] == 0x49 && b[2] == 0x46 && b[3] == 0x46 && // "RIFF"
                b[8] == 0x57 && b[9] == 0x45 && b[10] == 0x42 && b[11] == 0x50, // "WEBP"
            [".doc"] = b => b.Length >= 8 &&
                b[0] == 0xD0 && b[1] == 0xCF && b[2] == 0x11 && b[3] == 0xE0 &&
                b[4] == 0xA1 && b[5] == 0xB1 && b[6] == 0x1A && b[7] == 0xE1, // legacy OLE format
            [".docx"] = b => b.Length >= 4 && b[0] == 0x50 && b[1] == 0x4B && (b[2] == 0x03 || b[2] == 0x05 || b[2] == 0x07), // "PK.." (zip)
        };

        // Validate file signature against its extension
        private static async Task<bool> HasValidFileSignatureAsync(IFormFile file, string extension)
        {
            if (!FileSignatureValidators.TryGetValue(extension, out var validator))
                return false;

            var buffer = new byte[12];
            await using var stream = file.OpenReadStream();
            var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
            var header = bytesRead == buffer.Length ? buffer : buffer[..bytesRead];
            return validator(header);
        }

        // Sanitize display name for content-disposition header
        private static string SanitizeDisplayName(string name)
        {
            var cleaned = new string(name.Where(c => !char.IsControl(c)).ToArray()).Trim();

            if (string.IsNullOrWhiteSpace(cleaned))
                cleaned = "file";

            return cleaned.Length > 150 ? cleaned[..150] : cleaned;
        }

        // GET: api/FileUpload
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            // Fetch all files ordered by upload date
            var files = await _context.FileUploads
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();

            var fileDTOs = files.Select(f => new FileUploadDTO
            {
                Id = f.Id,
                FileName = f.FileName,
                OriginalName = f.OriginalName,
                DisplayName = f.DisplayName,
                FileUrl = $"{Request.Scheme}://{Request.Host}/api/FileUpload/download/{f.Id}",
                Category = f.Category,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt
            });

            return Ok(fileDTOs);
        }

        // GET: api/FileUpload/public
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicFiles()
        {
            // Fetch only public files
            var files = await _context.FileUploads
                .Where(f => f.IsPublic)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();

            var fileDTOs = files.Select(f => new FileDTO
            {
                Id = f.Id,
                DisplayName = f.DisplayName,
                FileUrl = $"{Request.Scheme}://{Request.Host}/api/FileUpload/download/{f.Id}",
                Category = f.Category,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt
            });

            return Ok(fileDTOs);
        }

        // POST: api/FileUpload
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string category, [FromForm] string? displayName, [FromForm] bool isPublic = true)
        {
            // Validate file presence
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            // Define allowed extensions and MIME types
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var allowedTypes = new Dictionary<string, string>
            {
                { ".pdf", "application/pdf" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".webp", "image/webp" },
                { ".gif", "image/gif" },
            };

            var extension = Path.GetExtension(file.FileName).ToLower();

            // Validate file extension
            if (!allowedExtensions.Contains(extension))
                return BadRequest("File type not allowed.");

            // Validate file signature
            if (!await HasValidFileSignatureAsync(file, extension))
                return BadRequest("File content does not match its extension.");

            var contentType = allowedTypes[extension];

            // Validate file size (10MB max)
            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("File size cannot exceed 10MB.");

            // Validate category
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("Category is required.");

            // Create upload directory if not exists
            var uploadDir = Path.Combine(_uploadDirectory);
            Directory.CreateDirectory(uploadDir);

            // Sanitize filename
            var nameToSanitize = string.IsNullOrWhiteSpace(displayName)
            ? Path.GetFileNameWithoutExtension(file.FileName)
            : displayName;

            var (fileName, suffix) = SanitizeFileName(nameToSanitize, extension);

            // Build final display name with suffix if needed
            var finalDisplayName = SanitizeDisplayName(nameToSanitize);

            if (suffix.HasValue)
                finalDisplayName = $"{finalDisplayName} ({suffix})";

            // Save file to disk
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create database record
            var fileUpload = new FileUpload
            {
                FileName = fileName,
                OriginalName = file.FileName,
                DisplayName = finalDisplayName,
                FilePath = filePath,
                ContentType = contentType,
                Category = category,
                IsPublic = isPublic,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _context.FileUploads.Add(fileUpload);
            await _context.SaveChangesAsync();

            // Return uploaded file data
            return Ok(new FileUploadDTO
            {
                Id = fileUpload.Id,
                FileName = fileUpload.FileName,
                OriginalName = fileUpload.OriginalName,
                DisplayName = fileUpload.DisplayName,
                FileUrl = $"{Request.Scheme}://{Request.Host}/api/FileUpload/download/{fileUpload.Id}",
                Category = fileUpload.Category,
                IsPublic = fileUpload.IsPublic,
                FileSize = fileUpload.FileSize,
                UploadedAt = fileUpload.UploadedAt
            });
        }

        // DELETE: api/FileUpload/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var file = await _context.FileUploads.FindAsync(id);

            if (file == null)
                return NotFound($"File with id {id} was not found.");

            // Remove file from disk
            var filePath = Path.Combine(_uploadDirectory, file.FileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            // Remove file record from database
            _context.FileUploads.Remove(file);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/FileUpload/download/{id}
        [HttpGet("download/{id}")]
        [EnableRateLimiting("downloads")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var file = await _context.FileUploads.FindAsync(id);
            if (file == null)
                return NotFound("File not found.");

            // Restrict private files to authenticated single-admin deployments
            if (!file.IsPublic && (User.Identity?.IsAuthenticated != true || await _userManager.Users.CountAsync() != 1))
                return NotFound("File not found.");

            var filePath = Path.Combine(_uploadDirectory, file.FileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on disk.");

            // Return file as download
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, file.ContentType, file.DisplayName + Path.GetExtension(file.FileName));
        }

        // PATCH: api/FileUpload/{id}/visibility
        [Authorize]
        [HttpPatch("{id}/visibility")]
        public async Task<IActionResult> UpdateVisibility(Guid id, [FromBody] bool isPublic)
        {
            var file = await _context.FileUploads.FindAsync(id);
            if (file == null)
                return NotFound($"File with id {id} was not found.");

            // Toggle file visibility
            file.IsPublic = isPublic;
            await _context.SaveChangesAsync();

            return Ok(new FileUploadDTO
            {
                Id = file.Id,
                FileName = file.FileName,
                OriginalName = file.OriginalName,
                DisplayName = file.DisplayName,
                FileUrl = $"{Request.Scheme}://{Request.Host}/api/FileUpload/download/{file.Id}",
                Category = file.Category,
                IsPublic = file.IsPublic,
                FileSize = file.FileSize,
                UploadedAt = file.UploadedAt
            });
        }
    }
}