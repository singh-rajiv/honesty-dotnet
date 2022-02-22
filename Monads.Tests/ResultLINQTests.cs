#pragma warning disable CS8848

namespace Monads.Tests;

public class ResultLINQTests
{
    [Fact]
    public void Result_Select()
    {
        var hw = "Hello World";
        var ex = new Exception("Something happened");
        var r1 = Result.Value(hw);
        var r2 = Result.Exception<string>(ex);

        var l1 = from s in r1
                 select s.Length;

        var l2 = from s in r2
                 select s.Length;

        Assert.True(l1.IsValue);
        Assert.Equal(hw.Length, l1.Value);
        Assert.False(l2.IsValue);
        Assert.Equal(ex, l2.Exception);
    }

    [Fact]
    public void Result_SelectMany()
    {
        var h = "Hello ";
        var w = "World!";
        var ex = new Exception("Something happened");
        var e1 = Result.Value(h);
        var e2 = Result.Value(w);
        var e3 = Result.Exception<string>(ex);

        var e4 = from s1 in e1
                 from s2 in e2
                 select s1 + s2;

        var e5 = from s1 in e1
                 from s3 in e3
                 select s1 + s3;

        var e6 = from s3 in e3
                 from s2 in e2
                 from s1 in e1
                 select s1 + s2 + s3;

        var e7 = from x in Result.Value(5)
                 from y in Result.Value(10)
                 from z in Result.Value(15)
                 select x + y + z;

        Assert.True(e4.IsValue);
        Assert.Equal(h + w, e4.Value);
        Assert.False(e5.IsValue);
        Assert.Equal(ex, e5.Exception);
        Assert.False(e6.IsValue);
        Assert.Equal(ex, e6.Exception);
        Assert.True(e7.IsValue);
        Assert.Equal(30, e7.Value);
    }

    [Fact]
    public async Task Result_SelectAsync()
    {
        var sHello = "Hello";
        string? sNull = null;
        async Task<int> AsyncCodeOf(string? s) => await Task.Run(() => (int)Math.Sqrt(s!.GetHashCode()));

        var r1 = await
                 from errOrVal in Result.Try(AsyncCodeOf, sHello)
                 select
                 (
                     from val in errOrVal
                     select val * val
                 );
        Assert.True(r1.IsValue);

        var r2 = await
                 from errOrVal in Result.Try(AsyncCodeOf, sNull)
                 select
                 (
                     from val in errOrVal
                     select val * val
                 );
        Assert.False(r2.IsValue);
        Assert.NotNull(r2.Exception);
    }

    [Fact]
    public async Task Result_SelectManyAsync()
    {
        var sHello = "Hello";
        var sWorld = "World";
        string? sNull = null;
        async Task<int> AsyncCodeOf(string? s) => await Task.Run(() => (int)Math.Sqrt(s!.GetHashCode()));

        var r1 = await
                 from errOrVal1 in Result.Try(AsyncCodeOf, sHello)
                 from errOrVal2 in Result.Try(AsyncCodeOf, sWorld)
                 from errOrVal3 in Result.Try(AsyncCodeOf, sHello + sWorld)
                 select
                 (
                     from val1 in errOrVal1
                     from val2 in errOrVal2
                     from val3 in errOrVal3
                     select val1 + val2 + val3
                 );
        Assert.True(r1.IsValue);

        var r2 = await
                 from errOrVal1 in Result.Try(AsyncCodeOf, sHello)
                 from errOrVal2 in Result.Try(AsyncCodeOf, sNull)
                 from errOrVal3 in Result.Try(AsyncCodeOf, sHello + sWorld)
                 select
                 (
                     from val1 in errOrVal1
                     from val2 in errOrVal2
                     from val3 in errOrVal3
                     select val1 + val2 + val3
                 );
        Assert.False(r2.IsValue);
        Assert.NotNull(r2.Exception);
    }
}
