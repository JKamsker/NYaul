using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using System.Linq;

#if NET5_0_OR_GREATER
using System.Text.Json;
#endif

namespace NYaul.Extensions;

public static class FileInfoExtensions
{
    // OpenReadAsync
    public static FileStream OpenReadAsync(this FileInfo fileInfo)
    {
        return new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
    }

    public static FileInfo GetFile(this DirectoryInfo directoryInfo, string path)
        => new FileInfo(Path.Combine(directoryInfo.FullName, path));

    public static FileInfo GetFile(this DirectoryInfo directoryInfo, params string[] path)
    {
        return new FileInfo(Path.Combine(directoryInfo.FullName, Path.Combine(path)));
    }

    public static FileInfo EnsureDeleted(this FileInfo fileInfo)
    {
        if (fileInfo.Exists)
        {
            fileInfo.Delete();
            fileInfo.Refresh();
        }
        return fileInfo;
    }

    public static DirectoryInfo GetDirectory(this DirectoryInfo directoryInfo, params string[] path)
    {
        return new DirectoryInfo(Path.Combine(directoryInfo.FullName, Path.Combine(path)));
    }

    public static bool PathExists(this DirectoryInfo directoryInfo, params string[] path)
    {
        var fullPath = Path.Combine(directoryInfo.FullName, Path.Combine(path));
        return File.Exists(fullPath) || Directory.Exists(fullPath);
    }

    public static IEnumerable<DirectoryInfo> TraverseParents(this DirectoryInfo directory, bool includeSelf = true)
    {
        if (includeSelf)
        {
            yield return directory;
        }

        var current = directory.Parent;
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    public static bool HasDirectory(this DirectoryInfo directoryInfo, string directoryName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        return directoryInfo
            .GetDirectories(directoryName, SearchOption.TopDirectoryOnly)
            .Any(x => string.Equals(x.Name, directoryName, comparison));
    }

#if NET5_0_OR_GREATER

    //WriteAllTextAsync
    public static async Task<FileInfo> WriteAllTextAsync(this FileInfo fileInfo, string contents, CancellationToken cancellationToken = default)
    {
        {
            await using var stream = fileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.None);
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(contents);
        }
        fileInfo.Refresh();
        return fileInfo;
    }

    // WriteJsonAsync
    public static async Task<FileInfo> WriteJsonAsync<T>(this FileInfo fileInfo, T value, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
        return await fileInfo.WriteAllTextAsync(json, cancellationToken);
    }

#endif

    public static DirectoryInfo? FindFirstChildOfCommonAncestor(this DirectoryInfo directory, Func<DirectoryInfo, bool> other)
    {
        foreach (var item in directory.TraverseParents(true))
        {
            foreach (var sub in item.EnumerateDirectories())
            {
                if (other(sub))
                {
                    return sub;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if the file has the requested extension.
    /// It accounts for dotted and non-dotted extensions.
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="requestedExtension"></param>
    /// <returns></returns>
    public static bool HasExtension(this FileInfo fileInfo, string requestedExtension)
    {
        var fileExtension = fileInfo.Extension;
        var fexIsNull = string.IsNullOrEmpty(fileExtension);
        var rexIsNull = string.IsNullOrEmpty(requestedExtension);

        if (fexIsNull && rexIsNull)
        {
            return true;
        }

        if (fexIsNull || rexIsNull)
        {
            // edge case:
            // if fileinfo.name ends with a dot, the extension becomes empty
            var result = fileInfo.Name?.EndsWith('.') == true
                && requestedExtension.Length == 1
                && requestedExtension[0].Equals('.');

            return result;
        }

        if (requestedExtension.StartsWith('.'))
        {
            return fileInfo.Extension.Equals(requestedExtension, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
#if NET5_0_OR_GREATER
            var fexSpan = fileExtension.AsSpan()[1..];
            var rexSpan = requestedExtension.AsSpan();
            return fexSpan.Equals(rexSpan, StringComparison.OrdinalIgnoreCase);
#else
            return fileExtension.Substring(1).Equals(requestedExtension, StringComparison.OrdinalIgnoreCase);
#endif
        }
    }

    public static bool HasExtension(this FileInfo fileInfo, string extension1, string extension2)
    {
        return fileInfo.HasExtension(extension1) || fileInfo.HasExtension(extension2);
    }

    public static bool HasExtension(this FileInfo fileInfo, string extension1, string extension2, string extension3)
    {
        return fileInfo.HasExtension(extension1) || fileInfo.HasExtension(extension2) || fileInfo.HasExtension(extension3);
    }

    public static bool HasExtension(this FileInfo fileInfo, params string[] extensions)
    {
        foreach (var extension in extensions)
        {
            if (fileInfo.HasExtension(extension))
            {
                return true;
            }
        }
        return false;
    }
}