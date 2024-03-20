using System.Diagnostics;
using System.Security.Cryptography;

namespace Files.Api.Services;
public class FileHandlerService(ILogger<FileHandlerService> logger, IConfiguration config) : IFileHandlerService
{
    public string FileStoragePath => config["STORAGEPATH"]
                                     ?? throw new NullReferenceException("No storage path has been set.");

    public async Task<(string, long)> WriteFileAndGetCheckSum(Stream readStream, string fileName)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using var md5 = MD5.Create();
        Directory.CreateDirectory(FileStoragePath);
        using var writeStream = File.Create(Path.Combine(FileStoragePath, fileName));

        var buffer = new byte[1048576];
        int bytesRead;

        while ((bytesRead = await readStream.ReadAsync(buffer)) > 0)
        {
            await writeStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
        }

        md5.TransformFinalBlock(buffer, 0, 0);

        var hash = md5.Hash;
        var hashString = BitConverter.ToString(hash!).Replace("-", "").ToLowerInvariant();
        var size = writeStream.Length;

        stopwatch.Stop();
        logger.LogInformation("WriteToDiskAndGetCheckSum took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        
        return (hashString, size);
    }

    public Stream GetFileStream(string fileName)
    {
        var path = Path.Combine(FileStoragePath, fileName);
        var fs = File.OpenRead(path);

        return fs;
    }

    public void DeleteFiles(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            var path = Path.Combine(FileStoragePath, fileName);
            try
            {
                File.Delete(path);
            }
            catch (FileNotFoundException)
            {
                logger.LogWarning($"File {fileName} could not be found.");
            }
            catch (IOException)
            {
                logger.LogWarning($"File {fileName} could not be deleted.");
            }
        }
    }
}

public interface IFileHandlerService
{
    string FileStoragePath { get; }
    Task<(string, long)> WriteFileAndGetCheckSum(Stream readStream, string fileName);
    Stream GetFileStream(string fileName);
    void DeleteFiles(IEnumerable<string> fileName);
}