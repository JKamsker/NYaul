using NYaul.IO.FileProvider;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

public class VirtualFileProvider : IFileProvider
{
    private readonly Dictionary<string, VirtualFileProviderEntry> _fileEntries = new Dictionary<string, VirtualFileProviderEntry>();

    public void AddFile(string path, byte[] content)
    {
        _fileEntries[path] = new VirtualFileProviderEntry(path, content);
    }

    public void AddFile(string path, string content)
    {
        _fileEntries[path] = new VirtualFileProviderEntry(path, content);
    }

    public bool FileExists(string path)
    {
        return _fileEntries.ContainsKey(path);
    }

    public Stream OpenRead(string path)
    {
        if (!_fileEntries.ContainsKey(path))
            throw new FileNotFoundException();

        byte[] content = _fileEntries[path].Content;
        return new MemoryStream(content);
    }

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
            Path = path;
            StringContent = content;
        }
    }
}