using NYaul.IO.Streams;

using System;
using System.IO;

namespace NYaul.Extensions;

public static class FileStreamExtensions
{
    public static Stream TruncateOnDisposal(this Stream stream)
    {
        var wrapperStream = new TruncateOnDisposeStream(stream);
        return wrapperStream;
    }

    // Actionstream
    public static Stream OnDispose(this Stream stream, Action action)
    {
        var wrapperStream = new ActionStream(stream, action);
        return wrapperStream;
    }
}