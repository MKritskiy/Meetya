using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Files.Web.Services;

public static class FileService
{
    const string FOLDER_PREFIX = "./wwwroot";

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

    #region Common Methods
    private static string GenerateHash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(data);
        return Convert.ToHexString(hashBytes);
    }

    private static string GenerateHash(Stream stream)
    {
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(stream);
        stream.Position = 0;
        return Convert.ToHexString(hashBytes);
    }

    private static string GetFileFolderPath(string hash, string category = "files")
    {
        return $"/{category}/{hash[..2]}/{hash[2..4]}";
    }

    private static void EnsureDirectoryExists(string fullPath)
    {
        var dirPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath!);
        }
    }
    #endregion
    #region General File Handling
    public static async Task<string> SaveFileAsync(Stream fileStream, string originalFileName, string category = "files")
    {
        var fileHash = GenerateHash(fileStream);
        var fileExtension = Path.GetExtension(originalFileName).ToLowerInvariant();
        var storagePath = GetFileFolderPath(fileHash, category);
        var fileName = $"{fileHash}{fileExtension}";
        var fullPath = Path.Join(FOLDER_PREFIX, storagePath, fileName);
        EnsureDirectoryExists(fullPath);


        await using var outputStream = File.Create(fullPath);
        await fileStream.CopyToAsync(outputStream);

        return $"{storagePath}/{fileName}";
    }

    public static async Task<Stream> GetFileAsync(string filePath)
    {
        var fullPath = Path.Join(FOLDER_PREFIX, filePath);
        if (!File.Exists(fullPath)) throw new FileNotFoundException("File not found");

        var memoryStream = new MemoryStream();
        await using var fileStream = File.OpenRead(fullPath);
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;

    }

    #endregion
    #region Image-Specific Handling
    public static async Task<string> ProcessImageAsync(
        Stream imageStream,
        string originalFileName,
        int? maxWidth=null,
        int? maxHeight = null,
        int quality = 75)
    {
        var fileExtension = Path.GetExtension(originalFileName).ToLowerInvariant();

        if (!AllowedImageExtensions.Contains(fileExtension)) throw new InvalidOperationException("Unsupported image format");
        

        using var image = await Image.LoadAsync(imageStream);


        var options = new ResizeOptions
        {
            Size = new Size(maxWidth ?? image.Width, maxHeight ?? image.Height),
            Mode = ResizeMode.Max,
            Sampler = KnownResamplers.Lanczos3
        };

        //Применяем настройки к изображнию
        image.Mutate(x=>x.Resize(options));


        using var processedStream = new MemoryStream();

        await image.SaveAsJpegAsync(processedStream, new JpegEncoder { Quality = quality });


        //Вычисляем хеш изображения из временного потока и из настроек изображения
        string contentHash = GenerateHash(processedStream.ToArray());
        string settingsHash = GenerateHash(Encoding.UTF8.GetBytes(
            $"{maxWidth}:{maxHeight}:{quality}"));
        string uniqueId = GenerateHash(Encoding.UTF8.GetBytes(contentHash+settingsHash));


        //Определяем путь хранения файла
        string storagePath = GetFileFolderPath(uniqueId, "images");
        string fileName = $"{uniqueId}.jpeg";
        string fullPath = Path.Join(FOLDER_PREFIX, storagePath, fileName);

        EnsureDirectoryExists(fullPath);

        //Переносим данные из веременного потока в файл
        processedStream.Position = 0;
        await using var fileStream = File.Create(fullPath);
        await processedStream.CopyToAsync(fileStream);


        return $"{storagePath}/{fileName}";
    }
    #endregion
}
