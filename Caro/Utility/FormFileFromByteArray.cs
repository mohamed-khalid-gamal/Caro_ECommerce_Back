using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

public class FormFileFromByteArray : IFormFile
{
    private readonly byte[] _fileBytes;
    private readonly string _fileName;
    private readonly string _contentType;

    public FormFileFromByteArray(byte[] fileBytes, string fileName, string contentType)
    {
        _fileBytes = fileBytes;
        _fileName = fileName;
        _contentType = contentType;
    }

    public string ContentType => _contentType;

    public string ContentDisposition => $"inline; filename={_fileName}";

    public IHeaderDictionary Headers => new HeaderDictionary();

    public long Length => _fileBytes.Length;

    public string Name => _fileName;

    public string FileName => _fileName;

    public void CopyTo(Stream target)
    {
        using (var stream = new MemoryStream(_fileBytes))
        {
            stream.CopyTo(target);
        }
    }

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        using (var stream = new MemoryStream(_fileBytes))
        {
            return stream.CopyToAsync(target, cancellationToken);
        }
    }

    public Stream OpenReadStream()
    {
        return new MemoryStream(_fileBytes);
    }
}
