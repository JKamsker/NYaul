using FluentAssertions;
using NYaul.Disposable;

namespace NYaul.Tests;

public class ActionDisposableTest
{
    [Fact]
    public void ActionDisposable_Should_Reset_State()
    {
        var state = true;
        {
            using var _ = ActionDisposable.Create(false, true, ns => state = ns);
            state.Should().BeFalse();
        }
        
        state.Should().BeTrue();
        
    }
}