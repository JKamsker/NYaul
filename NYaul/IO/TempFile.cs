﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NYaul.Internals;

namespace NYaul.IO;

/// <summary>
/// Represents a temporary file that is automatically deleted when disposed.
/// </summary>
/// <remarks>
/// This class provides functionality to create and manage temporary files with automatic cleanup.
/// It implements IDisposable to ensure proper resource cleanup when the temporary file is no longer needed.
/// </remarks>
public class TempFile : IDisposable
{
    /// <summary>
    /// Gets the full path to the temporary file.
    /// </summary>
    public string Location { get; }

    /// <summary>
    /// Gets a value indicating whether the temporary file exists on disk.
    /// </summary>
    public bool Exists => File.Exists(Location);

    /// <summary>
    /// Initializes a new instance of the <see cref="TempFile"/> class with a specified path.
    /// </summary>
    /// <param name="path">The full path for the temporary file.</param>
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

    /// <summary>
    /// Creates a temporary file with the specified extension.
    /// </summary>
    /// <param name="extension">The file extension (with or without leading dot).</param>
    /// <returns>A new TempFile instance with a unique random name and the specified extension.</returns>
    /// <remarks>
    /// This method is marked as not browsable in the editor as it's an implementation detail.
    /// Use <see cref="CreateWithExtension(FileExtension)"/> instead.
    /// </remarks>
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

    /// <summary>
    /// Asynchronously writes text content to the temporary file.
    /// </summary>
    /// <param name="content">The text content to write to the file.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public async Task WriteAllTextAsync(string content)
    {
        using var stream = File.OpenWrite(Location);
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(content);
    }

    // ReadAllTextAsync
    /// <summary>
    /// Asynchronously reads all text content from the temporary file.
    /// </summary>
    /// <returns>A task that represents the asynchronous read operation, containing the file's text content.</returns>
    public async Task<string> ReadAllTextAsync()
    {
        using var stream = File.OpenRead(Location);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Asynchronously writes byte array content to the temporary file.
    /// </summary>
    /// <param name="bytes">The byte array to write to the file.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public async Task WriteAllBytesAsync(byte[] bytes)
    {
        await FilePolyfill.WriteAllBytesAsync(Location, bytes);
    }

    /// <summary>
    /// Overwrites the temporary file with the contents of another file.
    /// </summary>
    /// <param name="sourcePath">The source file information.</param>
    /// <remarks>
    /// If the temporary file already exists, it will be overwritten.
    /// </remarks>
    public void OverwriteFrom(FileInfo sourcePath)
    {
        File.Copy(sourcePath.FullName, Location, true);
    }

    /// <summary>
    /// Overwrites the temporary file with the contents of another file.
    /// </summary>
    /// <param name="sourcePath">The path to the source file.</param>
    /// <remarks>
    /// If the temporary file already exists, it will be overwritten.
    /// </remarks>
    public void OverwriteFrom(string sourcePath)
    {
        File.Copy(sourcePath, Location, true);
    }

    /// <summary>
    /// Overwrites the temporary file with the contents of a stream.
    /// </summary>
    /// <param name="stream">The source stream to copy from.</param>
    /// <remarks>
    /// The stream is copied from its current position to its end.
    /// If the temporary file already exists, it will be overwritten.
    /// The file length will be adjusted to match the copied data length.
    /// </remarks>
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

    /// <summary>
    /// Opens the temporary file for writing.
    /// </summary>
    /// <returns>A write-only FileStream for the temporary file.</returns>
    /// <remarks>
    /// If the file already exists, it will be deleted before opening.
    /// The caller is responsible for properly disposing the returned stream.
    /// </remarks>
    public Stream OpenWrite()
    {
        FileEx.DeleteIfExists(Location);
        return File.OpenWrite(Location);
    }

    /// <summary>
    /// Performs cleanup by deleting the temporary file if it exists.
    /// </summary>
    public void Dispose()
    {
        FileEx.DeleteIfExists(Location);
    }
}
