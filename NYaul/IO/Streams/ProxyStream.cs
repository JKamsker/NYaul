using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NYaul.IO;

#if NET5_0_OR_GREATER
public abstract class ProxyStream : Stream
{
    private readonly Stream _stream;

    public ProxyStream(Stream stream)
    {
        _stream = stream;
    }

    public override bool CanRead => _stream.CanRead;

    public override bool CanSeek => _stream.CanSeek;

    public override bool CanWrite => _stream.CanWrite;

    public override long Length => _stream.Length;

    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public override bool CanTimeout => base.CanTimeout;

    public override int ReadTimeout { get => base.ReadTimeout; set => base.ReadTimeout = value; }
    public override int WriteTimeout { get => base.WriteTimeout; set => base.WriteTimeout = value; }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return _stream.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return _stream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void CopyTo(Stream destination, int bufferSize)
    {
        _stream.CopyTo(destination, bufferSize);
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        return _stream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return _stream.EndRead(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        _stream.EndWrite(asyncResult);
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return _stream.FlushAsync(cancellationToken);
    }

    public override int Read(Span<byte> buffer)
    {
        return _stream.Read(buffer);
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _stream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _stream.ReadAsync(buffer, cancellationToken);
    }

    public override int ReadByte()
    {
        return _stream.ReadByte();
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        _stream.Write(buffer);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _stream.WriteAsync(buffer, cancellationToken);
    }

    public override void WriteByte(byte value)
    {
        _stream.WriteByte(value);
    }

    public override ValueTask DisposeAsync()
    {
        return _stream.DisposeAsync();
    }

    protected override void Dispose(bool disposing)
    {
        _stream.Dispose();
    }

    public override void Close()
    {
        _stream.Close();
    }
}
#else

public abstract class ProxyStream : Stream
{
    private readonly Stream _stream;

    public ProxyStream(Stream stream)
    {
        _stream = stream;
    }

    public override bool CanRead => _stream.CanRead;

    public override bool CanSeek => _stream.CanSeek;

    public override bool CanWrite => _stream.CanWrite;

    public override long Length => _stream.Length;

    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public override bool CanTimeout => base.CanTimeout;

    public override int ReadTimeout { get => base.ReadTimeout; set => base.ReadTimeout = value; }
    public override int WriteTimeout { get => base.WriteTimeout; set => base.WriteTimeout = value; }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _stream.Write(buffer, offset, count);
    }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return _stream.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return _stream.BeginWrite(buffer, offset, count, callback, state);
    }

    //public override void CopyTo(Stream destination, int bufferSize)
    //{
    //    _stream.CopyTo(destination, bufferSize);
    //}

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        return _stream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return _stream.EndRead(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        _stream.EndWrite(asyncResult);
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return _stream.FlushAsync(cancellationToken);
    }

    //public override int Read(Span<byte> buffer)
    //{
    //    return _stream.Read(buffer);
    //}

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _stream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    //public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    //{
    //    return _stream.ReadAsync(buffer, cancellationToken);
    //}

    public override int ReadByte()
    {
        return _stream.ReadByte();
    }

    //public override void Write(ReadOnlySpan<byte> buffer)
    //{
    //    _stream.Write(buffer);
    //}

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return _stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    //public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    //{
    //    return _stream.WriteAsync(buffer, cancellationToken);
    //}

    public override void WriteByte(byte value)
    {
        _stream.WriteByte(value);
    }

    //public override ValueTask DisposeAsync()
    //{
    //    return _stream.DisposeAsync();
    //}

    protected override void Dispose(bool disposing)
    {
        _stream.Dispose();
    }

    public override void Close()
    {
        _stream.Close();
    }
}

#endif