namespace JobStream.Application.Interfaces;

public interface IFileService
{
    bool FileExists(string path);
    Stream OpenRead(string path);
    string GetFileName(string path);
}