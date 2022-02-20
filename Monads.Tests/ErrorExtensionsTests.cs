namespace Monads.Tests;

public class ErrorExtensionsTests
{
    [Fact]
    public void Error_ToError()
    {
        var ex = new Exception("Something happened");
        var i = 5;
        var s = "Hellow World";
        var o1 = Optional.Some(10);
        var o2 = Optional<int>.None;

        var e1 = i.ToError();
        var e2 = s.ToError();
        var e3 = ex.ToError<int>();
        var e4 = o1.ToError(ex);
        var e5 = o2.ToError(ex);

        Assert.True(e1.IsValue);
        Assert.True(e2.IsValue);
        Assert.False(e3.IsValue);
        Assert.True(e4.IsValue);
        Assert.False(e5.IsValue);
        Assert.Equal(ex, e3.Exception);
        Assert.Equal(ex, e5.Exception);
    }

    [Fact]
    public void Error_Flatten()
    {
        var ex = new Exception("Something happened.");
        var ee1 = Error.Value(Error.Value(10));
        var ee2 = Error.Exception<Error<int>>(ex);

        var e1 = ee1.Flatten();
        Assert.True(e1.IsValue);
        Assert.Equal( e1.GetType(), ee1.Value?.GetType());
        Assert.Equal(10, e1.Value);

        var e2 = ee2.Flatten();
        Assert.False(e2.IsValue);
        Assert.Equal(ex, e2.Exception);
    }
}
