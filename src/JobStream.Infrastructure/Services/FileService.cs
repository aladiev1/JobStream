using JobStream.Application.Interfaces;

namespace JobStream.Infrastructure.Services;

public class FileService : IFileService
{
    public bool FileExists(string path)
    {
        return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
    }

    public Stream OpenRead(string path)
    {
        return File.OpenRead(path);
    }

    public string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }
}