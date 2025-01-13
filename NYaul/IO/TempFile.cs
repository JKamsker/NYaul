using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NYaul.Internals;

namespace NYaul.IO;

public class TempFile : IDisposable
{
    public string Location { get; }

    public bool Exists => File.Exists(Location);

    public TempFile(string path)
    {
        Location = path;
    }

    /// <summary>
    /// Creates a TempFile with a random name in the temp directory.
    /// </summary>
    /// <returns></returns>
    public static TempFile CreateRandom()
    {
        return CreateWithExtension(".tmp");
    }

    /// <summary>
    /// Finds a random filename that does not exist in the temp directory and creates a TempFile with that name.
    /// Does not actually create the file on disk.
    /// </summary>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static TempFile CreateWithExtension(FileExtension extension)
        => CreateWithExtension(extension.Extension);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static TempFile CreateWithExtension(string extension)
    {
        var tempDirectory = Path.GetTempPath();
        var startsWithDot = extension.StartsWith('.');

        while (true)
        {
            var randomName = RandomPolyfill.Shared.NextString(10);
            var sb = new StringBuilder();
            sb.Append(tempDirectory);
            sb.Append(randomName);

            if (startsWithDot)
            {
                sb.Append(extension);
            }
            else
            {
                sb.Append('.');
                sb.Append(extension);
            }

            var newPath = sb.ToString();
            if (!File.Exists(newPath))
            {
                return new TempFile(newPath);
            }
        }
    }

    public async Task WriteAllTextAsync(string content)
    {
        using var stream = File.OpenWrite(Location);
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(content);
    }

    // ReadAllTextAsync
    public async Task<string> ReadAllTextAsync()
    {
        using var stream = File.OpenRead(Location);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public async Task WriteAllBytesAsync(byte[] bytes)
    {
        await FilePolyfill.WriteAllBytesAsync(Location, bytes);
    }

    public void OverwriteFrom(FileInfo sourcePath)
    {
        File.Copy(sourcePath.FullName, Location, true);
    }

    public void OverwriteFrom(string sourcePath)
    {
        File.Copy(sourcePath, Location, true);
    }

    public void OverwriteFrom(Stream stream)
    {
        using var fs = new FileStream(Location, FileMode.Create, FileAccess.Write);
        fs.Position = 0;
        stream.CopyTo(fs);
        if (fs.Position < fs.Length)
        {
            fs.SetLength(fs.Position);
        }
    }

    public Stream OpenWrite()
    {
        FileEx.DeleteIfExists(Location);
        return File.OpenWrite(Location);
    }

    public void Dispose()
    {
        FileEx.DeleteIfExists(Location);
    }
}