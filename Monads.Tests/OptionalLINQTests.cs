#pragma warning disable CS8848
namespace Monads.Tests;

public class OptionalLINQTests
{
    [Fact]
    public void Optional_Select()
    {
        var hw = "Hello World";
        var o1 = Optional.Try(() => hw);
        var o2 = Optional<string>.None;

        var ol1 = from s in o1
                  select s.Length;

        var ol2 = from s in o2
                  select s.Length;

        Assert.True(ol1.IsSome);
        Assert.Equal(hw.Length, ol1.Value);
        Assert.False(ol2.IsSome);
    }

    [Fact]
    public void Optional_SelectMany()
    {
        var h = "High ";
        var f = 5;
        var oh = Optional.Try(() => h);
        var of = Optional.Try(() => f);
        var ox = Optional<string>.None;

        var ohw = from s in oh
                  from i in of
                  select s + i;

        var ohx = from v1 in oh
                  from v2 in ox
                  select v1 + v2;

        Assert.True(ohw.IsSome);
        Assert.Equal(h + f, ohw.Value);
        Assert.False(ohx.IsSome);
    }

    [Fact]
    public async Task Optional_SelectAsync()
    {
        var sHello = "Hello";
        string? sNull = null;

        Task<int> AsyncCodeOf(string? i) => Task.Run(() => (int)Math.Sqrt(i!.GetHashCode()));

        var r1 = await
                 from maybeValue in Optional.Try(AsyncCodeOf, sHello)
                 select
                 (
                     from value in maybeValue
                     select value * value
                 );
        Assert.True(r1.IsSome);

        var r2 = await
                 from maybeValue in Optional.Try(AsyncCodeOf, sNull)
                 select
                 (
                     from value in maybeValue
                     select value * value
                 );
        Assert.False(r2.IsSome);
    }

    [Fact]
    public async Task Optional_SelectManyAsyc()
    {
        var sHello = "Hello";
        var sWorld = "World";
        string? sNull = null;

        Task<int> AsyncCodeOf(string? i) => Task.Run(() => (int)Math.Sqrt(i!.GetHashCode()));

        var r1 = await
                 from maybeValue1 in Optional.Try(AsyncCodeOf, sHello)
                 from maybeValue2 in Optional.Try(AsyncCodeOf, sWorld)
                 from maybeValue3 in Optional.Try(AsyncCodeOf, sHello + sWorld)
                 select
                 (
                     from value1 in maybeValue1
                     from value2 in maybeValue2
                     from value3 in maybeValue3
                     select value1 + value2 + value3
                 );
        Assert.True(r1.IsSome);

        var r2 = await
                 from maybeValue1 in Optional.Try(AsyncCodeOf, sHello)
                 from maybeValue2 in Optional.Try(AsyncCodeOf, sNull)
                 from maybeValue3 in Optional.Try(AsyncCodeOf, sHello + sWorld)
                 select
                 (
                     from value1 in maybeValue1
                     from value2 in maybeValue2
                     from value3 in maybeValue3
                     select value1 + value2 + value3
                 );
        Assert.False(r2.IsSome);
    }

    [Fact]
    public void Optional_Where()
    {
        var o1 = 5.ToOptional();
        var o2 = from i in o1
                 where i % 2 == 1
                 select i;
        var o3 = from i in o1
                 where i % 2 == 0
                 select i;

        Assert.True(o2.IsSome);
        Assert.False(o3.IsSome);

        var o4 = Optional<int>.None;
        var o5 = from i in o4
                 where i % 2 == 1
                 select i;
        Assert.False(o5.IsSome);
    }

    [Fact]
    public async Task Optional_WhereAsync()
    {
        Task<bool> AsyncPredicate(int i) => Task.Run(() => i % 2 == 1);

        var o1 = 5.ToOptional();
        var o2 = await
                 from i in o1
                 where AsyncPredicate(i)
                 select i;

        Assert.True(o2.IsSome);
    }
}
