namespace SwiggyClone.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken ct = default);
    Task DeleteAsync(string filePath, CancellationToken ct = default);
}
