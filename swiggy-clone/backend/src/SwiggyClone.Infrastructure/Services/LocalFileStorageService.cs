using Microsoft.AspNetCore.Hosting;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Infrastructure.Services;

public sealed class LocalFileStorageService(IWebHostEnvironment env) : IFileStorageService
{
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default)
    {
        var uploadsDir = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "uploads", folder);
        Directory.CreateDirectory(uploadsDir);

        var uniqueName = $"{Guid.CreateVersion7()}_{fileName}";
        var filePath = Path.Combine(uploadsDir, uniqueName);

        await using var fs = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fs, ct);

        return $"/uploads/{folder}/{uniqueName}";
    }

    public Task DeleteAsync(string filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.CompletedTask;

        var fullPath = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), filePath.TrimStart('/'));
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
