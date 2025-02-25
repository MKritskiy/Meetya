using Files.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Files.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly string _baseUrl;
    public FilesController(
        Serilog.ILogger logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _baseUrl = configuration["Storage:BaseUrl"] ?? "http://localhost:50000/web-files/api/files/static";
    }

    /// <summary>
    /// Загрузка обычного файла
    /// </summary>
    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)] // Лимит 100 МБ
    public async Task<IActionResult> UploadFile(
        [Required] IFormFile file,
        [FromQuery] string category = "files")
    {
        try
        {
            await using var stream = file.OpenReadStream();
            var filePath = await FileService.SaveFileAsync(
                stream,
                file.FileName,
                category);
            return Ok(new
            {
                Url = $"{_baseUrl}{filePath}",
                Path = filePath,
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "File upload failed");
            return StatusCode(500, new { Error = "File upload failed" });
        }
    }

    /// <summary>
    /// Загрузка и обработка изображения
    /// </summary>
    [HttpPost("upload-image")]
    [RequestSizeLimit(20 * 1024 * 1024)] // Лимит 20 МБ
    public async Task<IActionResult> UploadImage(
        [Required] IFormFile image,
        [FromQuery] int? maxWidth = null,
        [FromQuery] int? maxHeight = null,
        [FromQuery][Range(1, 100)] int quality = 75)
    {
        try
        {
            await using var stream = image.OpenReadStream();
            var imagePath = await FileService.ProcessImageAsync(
                stream,
                image.FileName,
                maxWidth,
                maxHeight,
                quality);

            return Ok(new
            {
                Url = $"{_baseUrl}{imagePath}",
                Path = imagePath
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Image upload failed");
            return StatusCode(500, new { Error = "Image processing failed" });
        }
    }

    /// <summary>
    /// Получение файла по пути
    /// </summary>
    [HttpGet("{*path}")]
    public async Task<IActionResult> GetFile([FromRoute] string path)
    {
        try
        {
            string filePath = Uri.UnescapeDataString(path);
            var stream = await FileService.GetFileAsync(filePath);
            var mimeType = GetMimeType(Path.GetExtension(filePath));

            return File(stream, mimeType, Path.GetFileName(filePath));
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { Error = "File not found" });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "File download failed");
            return StatusCode(500, new { Error = "File download failed" });
        }
    }

    private static string GetMimeType(string extension)
    {
        return extension.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

}
