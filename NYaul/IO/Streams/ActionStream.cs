using NYaul.IO;

using System;
using System.IO;
using System.Threading.Tasks;

namespace NYaul.IO.Streams;

public class ActionStream : ProxyStream
{
    private readonly Action _action;

    private bool _actionInvoked;

    public Stream InnerStream { get; }

    public ActionStream(Stream innerStream, Action action) : base(innerStream)
    {
        _action = action;
        InnerStream = innerStream;
    }

    private void InvokeAction()
    {
        if (!_actionInvoked)
        {
            _actionInvoked = true;
            _action();
        }
    }

    public override void Close()
    {
        InvokeAction();
        base.Close();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            InvokeAction();
        }

        base.Dispose(disposing);
    }

#if NET5_0_OR_GREATER
    public override ValueTask DisposeAsync()
    {
        InvokeAction();
        return base.DisposeAsync();
    }
#endif
}