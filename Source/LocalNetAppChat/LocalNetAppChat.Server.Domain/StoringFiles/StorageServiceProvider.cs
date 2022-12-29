using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Server.Domain.StoringFiles;

public class StorageServiceProvider
{
    private readonly string _key;
    private readonly string _dataPath;

    public StorageServiceProvider(
        string key,
        string dataPath)
    {
        _key = key;
        _dataPath = dataPath;

        if (!Directory.Exists(Path.Combine(dataPath)))
            Directory.CreateDirectory(dataPath);
    }

    public async Task<Result<string>> Upload(
        string key,
        string filename,
        Stream dataStream)
    {
        if (key != _key)
            return Result<string>.Failure("Access Denied");

        var sanatizedFilename = Util.SanatizeFilename(filename);
        var filePath = Path.Combine(_dataPath, sanatizedFilename);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await dataStream.CopyToAsync(fileStream);
        }

        return Result<string>.Success("Ok");
    }

    public Result<string[]> GetFiles(
        string key)
    {
        if (key != _key)
            return Result<string[]>.Failure("Access Denied");

        string[] files = Directory.GetFiles(_dataPath);

        files = files.Select(x => x.Remove(0, _dataPath.Length + 1)).ToArray();

        return Result<string[]>.Success(files);
    }

    public async Task<Result<byte[]>> Download(
        string key,
        string filename)
    {
        if (key != _key)
            return Result<byte[]>.Failure("Access Denied");

        var sanatizedFilename = Util.SanatizeFilename(filename);
        var filePath = Path.Combine(_dataPath, sanatizedFilename);

        if (!File.Exists(filePath))
            return Result<byte[]>.Failure("File does not exist!");

        var fileContent = await File.ReadAllBytesAsync(filePath);
        return Result<byte[]>.Success(fileContent);
    }

    public Result<string> Delete(
        string key,
        string filename)
    {
        if (key != _key)
            return Result<string>.Failure("Access Denied");
        
        var sanatizedFilename = Util.SanatizeFilename(filename);
        var filePath = Path.Combine(_dataPath, sanatizedFilename);
        
        if (!File.Exists(filePath))
            return Result<string>.Failure("File does not exist!");
        
        File.Delete(filePath);
        
        return Result<string>.Success("Ok");
    }
}