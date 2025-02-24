﻿using System.IO;

//using Random = NYaul.Internals.RandomEx;

namespace NYaul.IO;

/// <summary>
/// Provides extended file operations for common file system tasks.
/// </summary>
public static class FileEx
{
    /// <summary>
    /// Deletes a file if it exists at the specified path.
    /// </summary>
    /// <param name="filename">The path of the file to delete.</param>
    /// <remarks>
    /// This method performs a safe delete operation by first checking if the file exists,
    /// preventing exceptions that would occur when trying to delete a non-existent file.
    /// </remarks>
    public static void DeleteIfExists(string filename)
    {
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }
    }

    /// <summary>
    /// Opens a file for asynchronous read operations.
    /// </summary>
    /// <param name="filename">The path of the file to open.</param>
    /// <returns>A FileStream configured for asynchronous read operations.</returns>
    /// <remarks>
    /// The returned FileStream is configured with:
    /// - FileMode.Open: Opens an existing file
    /// - FileAccess.Read: Read-only access
    /// - FileShare.Read: Allows other processes to read the file concurrently
    /// - Buffer size: 4096 bytes
    /// - UseAsync flag: true for asynchronous operations
    /// </remarks>
    public static FileStream OpenReadAsync(string filename)
    {
        return new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
    }
}
