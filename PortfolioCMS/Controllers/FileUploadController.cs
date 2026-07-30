using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public FileUploadController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/FileUpload
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            var files = await _context.FileUploads
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();

            var fileDTOs = files.Select(f => new FileUploadDTO
            {
                Id = f.Id,
                FileName = f.FileName,
                OriginalName = f.OriginalName,
                FileUrl = $"{Request.Scheme}://{Request.Host}/uploads/files/{f.FileName}",
                Category = f.Category,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt
            });

            return Ok(fileDTOs);
        }

        // GET: api/FileUpload/{category}
        [HttpGet("{category}")]
        public async Task<IActionResult> GetFilesByCategory(string category)
        {
            var files = await _context.FileUploads
                .Where(f => f.Category == category)
                .OrderByDescending(f => f.UploadedAt)
                .ToListAsync();

            var fileDTOs = files.Select(f => new FileUploadDTO
            {
                Id = f.Id,
                FileName = f.FileName,
                OriginalName = f.OriginalName,
                FileUrl = $"{Request.Scheme}://{Request.Host}/uploads/files/{f.FileName}",
                Category = f.Category,
                FileSize = f.FileSize,
                UploadedAt = f.UploadedAt
            });

            return Ok(fileDTOs);
        }

        // POST: api/FileUpload
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string category)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            // Allowed file types
            var allowedTypes = new[]
            {
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "image/jpeg",
                "image/png",
                "image/webp",
                "image/gif",
                "image/svg+xml",
            };

            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("File type not allowed. Allowed types: PDF, JPEG, PNG, WebP, GIF, SVG.");

            // Max 10MB
            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("File size cannot exceed 10MB.");

            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("Category is required.");

            // Create upload directory
            var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "files");
            Directory.CreateDirectory(uploadDir);

            // Preserve original file extension
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUpload = new FileUpload
            {
                FileName = fileName,
                OriginalName = file.FileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                Category = category,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _context.FileUploads.Add(fileUpload);
            await _context.SaveChangesAsync();

            return Ok(new FileUploadDTO
            {
                Id = fileUpload.Id,
                FileName = fileUpload.FileName,
                OriginalName = fileUpload.OriginalName,
                FileUrl = $"{Request.Scheme}://{Request.Host}/uploads/files/{fileName}",
                Category = fileUpload.Category,
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

            // Delete file from disk
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "files", file.FileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            // Delete record from database
            _context.FileUploads.Remove(file);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}