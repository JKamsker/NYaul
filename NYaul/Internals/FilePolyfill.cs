using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYaul.Internals;

internal class FilePolyfill
{
    // WriteAllBytesAsync
#if NET6_0_OR_GREATER
    public static async Task WriteAllBytesAsync(string path, byte[] bytes)
    {
        await System.IO.File.WriteAllBytesAsync(path, bytes);
    }
#else

    public static Task WriteAllBytesAsync(string path, byte[] bytes)
    {
        using var fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
        return fs.WriteAsync(bytes, 0, bytes.Length);
    }

#endif
}