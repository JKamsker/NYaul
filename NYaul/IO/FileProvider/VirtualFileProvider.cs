﻿using System;
using NYaul.IO.FileProvider;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

/// <summary>
/// Provides an in-memory file provider for testing or virtual file scenarios.
/// </summary>
/// <remarks>
/// This class implements <see cref="IFileProvider"/> by storing file data in memory. It can be used to simulate a file system
/// without writing any files to disk. Good for unit tests or other scenarios where ephemeral file data is needed.
/// </remarks>
public class VirtualFileProvider : IFileProvider
{
    private readonly Dictionary<string, VirtualFileProviderEntry> _fileEntries = new Dictionary<string, VirtualFileProviderEntry>();

    /// <summary>
    /// Adds a file to the virtual file system with the specified path and binary content.
    /// </summary>
    /// <param name="path">The path at which to add the file.</param>
    /// <param name="content">The binary content of the file.</param>
    public void AddFile(string path, byte[] content)
    {
        _fileEntries[path] = new VirtualFileProviderEntry(path, content);
    }

    /// <summary>
    /// Adds a file to the virtual file system with the specified path and string content.
    /// </summary>
    /// <param name="path">The path at which to add the file.</param>
    /// <param name="content">The text content of the file, which is converted to UTF-8 bytes internally.</param>
    public void AddFile(string path, string content)
    {
        _fileEntries[path] = new VirtualFileProviderEntry(path, content);
    }

    /// <summary>
    /// Determines whether the specified file exists in the virtual file system.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>true if the file exists; otherwise, false.</returns>
    public bool FileExists(string path)
    {
        return _fileEntries.ContainsKey(path);
    }

    /// <summary>
    /// Opens a file for read operations in the virtual file system.
    /// </summary>
    /// <param name="path">The path of the file to open.</param>
    /// <returns>A read-only stream referencing the file's in-memory data.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
    public Stream OpenRead(string path)
    {
        if (!_fileEntries.ContainsKey(path))
            throw new FileNotFoundException();

        byte[] content = _fileEntries[path].Content;
        return new MemoryStream(content);
    }

    /// <summary>
    /// Opens a file for write operations in the virtual file system.
    /// </summary>
    /// <param name="path">The path of the file to open.</param>
    /// <returns>A writeable stream referencing the file's in-memory data. When the stream is disposed, the file's content is updated with any changes.</returns>
    /// <remarks>
    /// If the file does not exist, a new file is created. If it exists, the current content is loaded into the stream,
    /// and upon disposal, the updated content is saved back.
    /// </remarks>
    public Stream OpenWrite(string path)
    {
        MemoryStream result;
        if (_fileEntries.TryGetValue(path, out var entry))
        {
            result = new MemoryStream(entry.Content.Length);
            result.Write(entry.Content, 0, entry.Content.Length);
            result.Position = result.Length;

            result.OnDispose(() =>
            {
                entry.Content = result.ToArray();
            });
        }
        else
        {
            result = new MemoryStream();
            result.OnDispose(() =>
            {
                _fileEntries[path] = new VirtualFileProviderEntry(path, result.ToArray());
            });
        }

        return result;
    }

    [DebuggerDisplay("Path = {Path}, Content = {StringContent}")]
    private class VirtualFileProviderEntry
    {
        public string Path { get; set; }
        public byte[] Content { get; set; }

        public string StringContent
        {
            get => Encoding.UTF8.GetString(Content);
            set => Content = Encoding.UTF8.GetBytes(value);
        }

        public VirtualFileProviderEntry(string path, byte[] content)
        {
            Path = path;
            Content = content;
        }
        
        
        public VirtualFileProviderEntry(string path, string content)
        {
            Content = Array.Empty<byte>();
            Path = path;
            StringContent = content;
        }
    }
}
