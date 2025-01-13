using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYaul.IO;

public record struct FileExtension
{
    public FileExtension Dll => ".dll";
    public FileExtension Exe => ".exe";
    public FileExtension Txt => ".txt";
    public FileExtension Json => ".json";
    public FileExtension Xml => ".xml";

    public string Extension { get; }

    public FileExtension(string extension)
    {
        if (extension == null)
        {
            throw new ArgumentNullException(nameof(extension));
        }

        if (!extension.StartsWith('.'))
        {
            extension = '.' + extension;
        }

        Extension = extension;
    }

    // Implicit
    public static implicit operator string(FileExtension extension) => extension.Extension;

    public static implicit operator FileExtension(string extension) => new(extension);
}