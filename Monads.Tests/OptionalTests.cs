namespace Monads.Tests;

public class OptionalTests
{
    [Fact]
    public void Optional_Ctor()
    {
        var o1 = new Optional<int>(5);
        var o2 = new Optional<string>("Test");
        var o3 = new Optional<object>(null);
        var x = 100;
        Optional<int> o4 = x;

        Assert.True(o1.IsSome);
        Assert.True(o2.IsSome);
        Assert.False(o3.IsSome);
        Assert.True(o4.IsSome);
        Assert.Equal(5, o1.Value);
        Assert.Equal("Test", o2.Value);
        Assert.Null(o3.Value);
        Assert.Equal(o4.Value, x);
        Assert.Equal(o4, x);
    }

    [Fact]
    public void Optional_Equality()
    {
        var o1 = new Optional<int>(5);
        var o2 = new Optional<string>("Test");
        var o3 = new Optional<object>(null);

        var o4 = new Optional<int>(6);
        var o5 = new Optional<string>("Test2");

        var o11 = new Optional<int>(5);
        var o22 = new Optional<string>("Test");
        var o33 = new Optional<object>(null);

        var r1 = o1;
        var r2 = o2;
        var r3 = o3;
        object r4 = o4;
        object r5 = o5;

        Optional<int>? n1 = null;
        object? n2 = null;

        Assert.False(o1.Equals(n1));
        Assert.False(o1.Equals(n2));

        Assert.Equal(o1, o1);
        Assert.Equal(o2, o2);
        Assert.Equal(o3, o3);

        Assert.True(o1.Equals(r1));
        Assert.True(o2.Equals(r2));
        Assert.True(o3.Equals(r3));

        Assert.Equal(o1, o11);
        Assert.Equal(o2, o22);
        Assert.Equal(o3, o33);
        Assert.Equal(o3, Optional<object>.None);

        Assert.True(o1.Equals(o11));
        Assert.True(o2.Equals(o22));
        Assert.True(o3.Equals(o33));

        Assert.True(o4.Equals(r4));
        Assert.False(o4.Equals(r5));

        Assert.Equal(o1.GetHashCode(), o11.GetHashCode());
        Assert.Equal(o2.GetHashCode(), o22.GetHashCode());
        Assert.Equal(o3.GetHashCode(), o33.GetHashCode());

        Assert.True(o1 == o11);
        Assert.True(o2 == o22);
        Assert.True(o3 == o33);
        Assert.True(o1 != o4);
        Assert.True(o2 != o5);
        Assert.True(o2 != o3);
    }

    [Fact]
    public void Optional_PatternMatch_Action()
    {
        var o1 = new Optional<string>("Test");
        var o2 = new Optional<string>(null);

        bool whenSomeCalled;
        bool whenNoneCalled;
        string input;

        void Reset() =>
            (whenSomeCalled, whenNoneCalled, input) = (false, false, string.Empty);

        void whenSome(string s) =>
            (input, whenSomeCalled) = (s, true);

        void whenNone() => 
            whenNoneCalled = true;

        var someAction = whenSome;
        var noneAction = whenNone;

        Reset();
        var u = o1.Match(someAction.ToFunc(), noneAction.ToFunc());
        Assert.Equal(Unit.Instance, u);
        Assert.True(whenSomeCalled);
        Assert.False(whenNoneCalled);
        Assert.Equal(o1.Value, input);

        Reset();
        u = o2.Match(someAction.ToFunc(), noneAction.ToFunc());
        Assert.Equal(Unit.Instance, u);
        Assert.False(whenSomeCalled);
        Assert.True(whenNoneCalled);
        Assert.Equal(string.Empty, input);
    }

    [Fact]
    public void Optional_PatternMatch_Func()
    {
        var v1 = 10;
        var o1 = new Optional<int>(v1);
        var o2 = Optional<int>.None;
        var defaultResult = -1;

        int whenSome(int i) => i * i;
        int whenNone() => defaultResult;

        var r1 = o1.Match(whenSome, whenNone);
        var r2 = o2.Match(whenSome, whenNone);

        Assert.Equal(v1 * v1, r1);
        Assert.Equal(defaultResult, r2);
    }

    [Fact]
    public async Task Optional_PatternMatch_ActionAsync()
    {
        var o1 = new Optional<string>("Test");
        var o2 = new Optional<string>(null);

        bool whenSomeCalled;
        bool whenNoneCalled;
        string input;

        void Reset() =>
            (whenSomeCalled, whenNoneCalled, input) = (false, false, string.Empty);

        void whenSome(string s) =>
            (input, whenSomeCalled) = (s, true);

        void whenNone() => 
            whenNoneCalled = true;

        var someAction = whenSome;
        var noneAction = whenNone;

        Reset();
        var u = await o1.Match(someAction.ToFuncAsync(), noneAction.ToFuncAsync());
        Assert.Equal(Unit.Instance, u);
        Assert.True(whenSomeCalled);
        Assert.False(whenNoneCalled);
        Assert.Equal(o1.Value, input);

        Reset();
        u = await o2.Match(someAction.ToFuncAsync(), noneAction.ToFuncAsync());
        Assert.Equal(Unit.Instance, u);
        Assert.False(whenSomeCalled);
        Assert.True(whenNoneCalled);
        Assert.Equal(string.Empty, input);
    }

    [Fact]
    public async Task Optional_PatternMatch_FuncAsync()
    {
        var v1 = 10;
        var o1 = v1.ToOptional();
        var o2 = Optional<int>.None;
        var defaultResult = -1;

        Task<int> whenSome(int i) => Task.Run(() => i * i);
        Task<int> whenNone() => Task.Run(() => defaultResult);

        var r1 = await o1.Match(whenSome, whenNone);
        var r2 = await o2.Match(whenSome, whenNone);

        Assert.Equal(v1 * v1, r1);
        Assert.Equal(defaultResult, r2);
    }

    [Fact]
    public void Optional_Where()
    {
        var o1 = new Optional<int>(5);
        var r1 = o1.Where(o => o == 5);
        var r2 = o1.Where(o => o % 2 == 0);
        Assert.Equal(o1, r1);
        Assert.False(r2.IsSome);
    }

    [Fact]
    public async Task Optional_WhereAsync()
    {
        var o1 = new Optional<int>(5);
        Task<bool> Is5(int x) => Task.Run(() => x == 5);
        Task<bool> IsEven(int x) => Task.Run(() => x % 2 == 0);
        var r1 = await o1.Where(Is5);
        var r2 = await o1.Where(IsEven);
        Assert.Equal(o1, r1);
        Assert.False(r2.IsSome);
    }

    [Fact]
    public void Optional_DefaultIfNone()
    {
        var o1 = Optional<int>.None.DefaultIfNone(30);
        var o2 = Optional<int>.None.DefaultIfNone(() => 40);
        Assert.True(o1.IsSome);
        Assert.Equal(30, o1.Value);
        Assert.True(o2.IsSome);
        Assert.Equal(40, o2.Value);

        var o3 = Optional.Some("Hello").DefaultIfNone("Hi");
        var o4 = Optional.Some(string.Empty).DefaultIfNone(() => "Hi");
        Assert.Equal("Hello", o3.Value);
        Assert.Equal(string.Empty, o4.Value);
    }

    [Fact]
    public async Task Optional_DefaultIfNoneAsync()
    {
        var o1 = await Optional<int>.None.DefaultIfNone(() => Task.FromResult(30));
        Assert.True(o1.IsSome);
        Assert.Equal(30, o1.Value);

        var o2 = await Optional.Some(25).DefaultIfNone(() => Task.FromResult(1));
        Assert.Equal(25, o2.Value);
    }

    [Fact]
    public void Optional_Map()
    {
        var o1 = Optional.Try(() => 5);
        var o2 = Optional<int>.None;
        var os1 = o1.Map(v => v * v);
        var os2 = o2.Map(v => v * v);

        Assert.True(os1.IsSome);
        Assert.False(os2.IsSome);
        Assert.Equal(25, os1.Value);
    }

    [Fact]
    public async Task Optional_MapAsync()
    {
        var o1 = Optional.Try(() => 5);
        var o2 = Optional<int>.None;
        Task<int> Square(int x) => Task.Run(() => x * x);

        var os1 = await o1.Map(Square);
        var os2 = await o2.Map(Square);

        Assert.True(os1.IsSome);
        Assert.False(os2.IsSome);
        Assert.Equal(25, os1.Value);
    }

    [Fact]
    public void Optional_Bind()
    {
        static int SquareOf(int x) => checked(x * x);

        var o1 = Optional.Try(SquareOf, 5);
        var o2 = Optional.Try(SquareOf, int.MaxValue);
        var so1 = o1.Bind(v => Optional.Try(SquareOf, v));
        var so2 = o2.Bind(v => Optional.Try(SquareOf, v));

        Assert.True(o1.IsSome);
        Assert.False(o2.IsSome);
        Assert.Equal(25, o1.Value);
        Assert.True(so1.IsSome);
        Assert.Equal(625, so1.Value);
        Assert.False(so2.IsSome);
    }

    [Fact]
    public async Task Optional_BindAsync()
    {
        static Task<int> SquareOf(int x) => checked(Task.Run(() => x * x));

        var o1 = await Optional.Try(SquareOf, 5);
        var o2 = await Optional.Try(SquareOf, int.MaxValue);
        var so1 = await o1.Bind(v => Optional.Try(SquareOf, v));
        var so2 = await o2.Bind(v => Optional.Try(SquareOf, v));
        var so3 = await o1.Bind(v => Optional.Try(SquareOf, v * int.MaxValue));

        Assert.True(o1.IsSome);
        Assert.False(o2.IsSome);
        Assert.Equal(25, o1.Value);
        Assert.True(so1.IsSome);
        Assert.Equal(625, so1.Value);
        Assert.False(so2.IsSome);
        Assert.False(so3.IsSome);
    }

    [Fact]
    public void Optional_Static()
    {
        var o1 = Optional.Some(5);
        var o2 = Optional.None(5);

        Assert.True(o1.IsSome);
        Assert.False(o2.IsSome);
    }

    [Fact]
    public void Optional_Try()
    {
        static int InvalidOp() => (new[] { 10 })[2];
        var o3 = Optional.Try(() => 5);
        var o4 = Optional.Try(InvalidOp);
        var o5 = Optional.Try((s) => s.Length, "Test");
        var o6 = Optional.Try((s) => s.Length, string.Empty);
        var o7 = Optional.Try((string? s) => s!.Length, null); //force NullReferenceException

        Assert.True(o3.IsSome);
        Assert.Equal(5, o3.Value);
        Assert.False(o4.IsSome);
        Assert.True(o5.IsSome);
        Assert.Equal(4, o5.Value);
        Assert.True(o6.IsSome);
        Assert.Equal(0, o6.Value);
        Assert.False(o7.IsSome);
    }

    [Fact]
    public async Task Optional_TryAsync()
    {
        static Task<int> Process(string? s) => Task.Run(() => s!.Length); //force NullReferenceException
        var o1 = await Optional.Try(() => Task.FromResult(5));
        var o2 = await Optional.Try(() => Task.Run(() => (new[] { 10 })[2]));
        var o3 = await Optional.Try(Process, "Test");
        var o4 = await Optional.Try(Process, string.Empty);
        var o5 = await Optional.Try<string?, int>(Process, null);

        Assert.True(o1.IsSome);
        Assert.Equal(5, o1.Value);
        Assert.False(o2.IsSome);
        Assert.True(o3.IsSome);
        Assert.Equal(4, o3.Value);
        Assert.True(o4.IsSome);
        Assert.Equal(0, o4.Value);
        Assert.False(o5.IsSome);
    }
}
