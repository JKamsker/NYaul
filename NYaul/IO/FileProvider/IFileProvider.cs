﻿using System.IO;

namespace NYaul.IO.FileProvider;

/// <summary>
/// Defines a contract for file system operations.
/// </summary>
/// <remarks>
/// Implementations of this interface provide methods to check for file existence and
/// to open files for reading or writing.
/// </remarks>
public interface IFileProvider
{
    /// <summary>
    /// Determines whether a file exists at the specified path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>True if the file exists, otherwise false.</returns>
    bool FileExists(string path);

    /// <summary>
    /// Opens a file for reading.
    /// </summary>
    /// <param name="path">The path of the file to open.</param>
    /// <returns>A <see cref="Stream"/> for reading from the specified file.</returns>
    Stream OpenRead(string path);

    /// <summary>
    /// Opens a file for writing.
    /// </summary>
    /// <param name="path">The path of the file to open.</param>
    /// <returns>A <see cref="Stream"/> for writing to the specified file.</returns>
    Stream OpenWrite(string path);
}

/// <summary>
/// Provides extension methods for <see cref="IFileProvider"/>.
/// </summary>
public static class FileProviderExtensions
{
    /// <summary>
    /// Reads all text from a file using the specified <see cref="IFileProvider"/>.
    /// </summary>
    /// <param name="fileProvider">The <see cref="IFileProvider"/> used to open the file.</param>
    /// <param name="path">The path of the file to read.</param>
    /// <returns>A string containing the file's contents.</returns>
    public static string ReadAllText(this IFileProvider fileProvider, string path)
    {
        using var stream = fileProvider.OpenRead(path);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
