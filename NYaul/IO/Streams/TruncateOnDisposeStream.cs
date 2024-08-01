using NYaul.IO;

using System;
using System.IO;
using System.Threading.Tasks;

namespace NYaul.IO.Streams;

public class TruncateOnDisposeStream : ProxyStream
{
    private readonly Stream _innerStream;
    private long _length;

    public TruncateOnDisposeStream(Stream innerStream) : base(innerStream)
    {
        _innerStream = innerStream;
        _length = innerStream.Length;
    }

    public override void Close()
    {
        Truncate();
        base.Close();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Truncate();
        }

        base.Dispose(disposing);
    }

#if NET5_0_OR_GREATER
    public override ValueTask DisposeAsync()
    {
        Truncate();

        return base.DisposeAsync();
    }
#endif

    private void Truncate()
    {
        if (Position >= _length)
        {
            return;
        }

        _innerStream.SetLength(Position);
        _length = Position;
    }
}