﻿﻿
using System.IO;

namespace NYaul.IO.FileProvider;

/// <summary>
/// Provides default implementation of file system operations using the System.IO classes.
/// </summary>
/// <remarks>
/// This is the standard file provider that directly interacts with the physical file system.
/// It implements the <see cref="IFileProvider"/> interface using the basic File class operations.
/// </remarks>
public class DefaultFileProvider : IFileProvider
{
    /// <summary>
    /// Gets the singleton instance of the DefaultFileProvider.
    /// </summary>
    public static readonly DefaultFileProvider Instance = new DefaultFileProvider();

    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="path">The path to the file to check.</param>
    /// <returns>true if the file exists; otherwise, false.</returns>
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// Opens a file for reading.
    /// </summary>
    /// <param name="path">The path to the file to open.</param>
    /// <returns>A read-only stream for the specified file.</returns>
    /// <remarks>
    /// The returned stream must be disposed by the caller to free file system resources.
    /// </remarks>
    public Stream OpenRead(string path)
    {
        return File.OpenRead(path);
    }

    /// <summary>
    /// Opens a file for writing operations.
    /// </summary>
    /// <param name="path">The path to the file to open.</param>
    /// <returns>A write stream for the specified file. The file is truncated on disposal.</returns>
    /// <remarks>
    /// The file is opened using <see cref="File.OpenWrite"/> and wrapped in a TruncateOnDisposal stream,
    /// which truncates the file to the current stream position upon disposal.
    /// </remarks>
    public Stream OpenWrite(string path)
    {
        return File.OpenWrite(path).TruncateOnDisposal();
    }
}
