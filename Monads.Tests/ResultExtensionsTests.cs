namespace Monads.Tests;

public class ResultExtensionsTests
{
    [Fact]
    public void Result_ToResult()
    {
        var ex = new Exception("Something happened");
        var i = 5;
        var s = "Hellow World";
        var o1 = Optional.Some(10);
        var o2 = Optional<int>.None;

        var e1 = i.ToResult();
        var e2 = s.ToResult();
        var e3 = ex.ToResult<int>();
        var e4 = o1.ToResult(ex);
        var e5 = o2.ToResult(ex);

        Assert.True(e1.IsValue);
        Assert.True(e2.IsValue);
        Assert.False(e3.IsValue);
        Assert.True(e4.IsValue);
        Assert.False(e5.IsValue);
        Assert.Equal(ex, e3.Exception);
        Assert.Equal(ex, e5.Exception);
    }

    [Fact]
    public void Result_Flatten()
    {
        var ex = new Exception("Something happened.");
        var ee1 = Result.Value(Result.Value(10));
        var ee2 = Result.Exception<Result<int>>(ex);

        var e1 = ee1.Flatten();
        Assert.True(e1.IsValue);
        Assert.Equal( e1.GetType(), ee1.Value.GetType());
        Assert.Equal(10, e1.Value);

        var e2 = ee2.Flatten();
        Assert.False(e2.IsValue);
        Assert.Equal(ex, e2.Exception);
    }
}
