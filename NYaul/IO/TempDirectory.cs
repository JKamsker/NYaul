﻿using System;
using System.IO;
using System.Text;

using NYaul.Internals;

//using Random = NYaul.Internals.RandomEx;

namespace NYaul.IO;

/// <summary>
/// Represents a temporary directory that is automatically deleted when disposed.
/// </summary>
/// <remarks>
/// This class provides functionality to create and manage temporary directories with automatic cleanup.
/// It implements IDisposable to ensure proper resource cleanup when the temporary directory is no longer needed.
/// </remarks>
public class TempDirectory : IDisposable
{
    /// <summary>
    /// Gets the full path to the temporary directory.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets a value indicating whether the temporary directory exists on disk.
    /// </summary>
    public bool Exists => Directory.Exists(Path);

    /// <summary>
    /// Initializes a new instance of the <see cref="TempDirectory"/> class with a specified path.
    /// </summary>
    /// <param name="path">The full path for the temporary directory.</param>
    public TempDirectory(string path)
    {
        Path = path;
    }

    // alias CreateTempDirectory
    /// <summary>
    /// Creates a new temporary directory with a random name in the system's temp directory.
    /// </summary>
    /// <returns>A new TempDirectory instance.</returns>
    public static TempDirectory CreateTempDirectory()
        => CreateRandom();

    /// <summary>
    /// Creates a new temporary directory with a random name in the system's temp directory.
    /// </summary>
    /// <returns>A new TempDirectory instance with a randomly generated name.</returns>
    /// <remarks>
    /// The directory is created immediately on disk with a random name to ensure uniqueness.
    /// The directory will be created in the system's temporary directory.
    /// </remarks>
    public static TempDirectory CreateRandom()
    {
        var tempDirectory = System.IO.Path.GetTempPath();
        while (true)
        {
            var randomName = RandomPolyfill.Shared.NextString(10);
            var sb = new StringBuilder();
            sb.Append(tempDirectory);
            sb.Append(randomName);

            var newPath = sb.ToString();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
                return new TempDirectory(newPath);
            }
        }
    }

    /// <summary>
    /// Performs cleanup by recursively deleting the temporary directory and all its contents.
    /// </summary>
    public void Dispose()
    {
        Directory.Delete(Path, true);
    }

    /// <summary>
    /// Gets a FileInfo instance for a file in the temporary directory.
    /// </summary>
    /// <param name="name">The name of the file.</param>
    /// <returns>A FileInfo instance representing the specified file.</returns>
    /// <remarks>
    /// This method does not create the file; it only provides information about the file path.
    /// </remarks>
    public FileInfo GetFile(string name)
    {
        var path = System.IO.Path.Combine(Path, name);
        return new FileInfo(path);
    }

    /// <summary>
    /// Gets a DirectoryInfo instance for a subdirectory in the temporary directory.
    /// </summary>
    /// <param name="name">The name of the subdirectory.</param>
    /// <returns>A DirectoryInfo instance representing the specified subdirectory.</returns>
    /// <remarks>
    /// This method does not create the directory; it only provides information about the directory path.
    /// </remarks>
    public DirectoryInfo GetDirectory(string name)
    {
        var path = System.IO.Path.Combine(Path, name);
        return new DirectoryInfo(path);
    }

    /// <summary>
    /// Creates a new file with a random name and specified extension in the temporary directory.
    /// </summary>
    /// <param name="extension">The file extension (with or without leading dot).</param>
    /// <returns>A FileInfo instance representing the created file.</returns>
    /// <remarks>
    /// The file is created immediately on disk with a random name to ensure uniqueness.
    /// The file is created empty and can be written to after creation.
    /// </remarks>
    public FileInfo CreateFileWithExtension(string extension)
    {
        var path = System.IO.Path.Combine(Path, RandomPolyfill.Shared.NextString(10) + extension);
        File.Create(path).Dispose();
        return new FileInfo(path);
    }

    // CreateDirectory
    /// <summary>
    /// Creates a new subdirectory with the specified name in the temporary directory.
    /// </summary>
    /// <param name="name">The name of the subdirectory to create.</param>
    /// <returns>A DirectoryInfo instance representing the created subdirectory.</returns>
    /// <remarks>
    /// If the subdirectory already exists, this method returns the existing directory information.
    /// </remarks>
    public DirectoryInfo CreateSubDirectory(string name)
    {
        var path = System.IO.Path.Combine(Path, name);
        return Directory.CreateDirectory(path);
    }
}
