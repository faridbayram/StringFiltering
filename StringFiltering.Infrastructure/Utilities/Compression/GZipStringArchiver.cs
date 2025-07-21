using System.IO.Compression;
using System.Text;
using StringFiltering.Application.Utilities;

namespace StringFiltering.Infrastructure.Utilities.Compression;

public class GZipStringArchiver : IStringArchiver
{
    public byte[] Archive(string valueToArchive)
    {
        var bytes = Encoding.UTF8.GetBytes(valueToArchive);
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
            gzip.Write(bytes, 0, bytes.Length);
        return output.ToArray();
    }

    public string Unarchive(byte[] valueToUnarchive)
    {
        using var input = new MemoryStream(valueToUnarchive);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return Encoding.UTF8.GetString(output.ToArray());
    }
}