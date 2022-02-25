namespace Monads.Tests;

public class ResultTests
{
    [Fact]
    public void Result_Ctor()
    {
        var emptyString = string.Empty;
        string? nullString = null;
        var ex = new Exception("Something happened");
        var e1 = new Result<int>(5);
        var e2 = new Result<int>(ex);
        var e3 = new Result<string>(emptyString);
        var e4 = new Result<string>(nullString);

        Assert.True(e1.IsValue);
        Assert.Null(e1.Exception);
        Assert.False(e2.IsValue);
        Assert.NotNull(e2.Exception);
        Assert.Equal(ex, e2.Exception);
        Assert.True(e3.IsValue);
        Assert.Null(e3.Exception);
        Assert.False(e4.IsValue);
        Assert.IsType<ArgumentNullException>(e4.Exception);
    }

    [Fact]
    public void Result_MatchAction()
    {
        var ex = new Exception("Something happened");
        var e1 = new Result<string>("HelloWorld");
        var e2 = new Result<string>(ex);

        bool whenValueCalled;
        bool whenExCalled;
        string? argStr;
        Exception argEx = new();

        void Reset() =>
            (whenValueCalled, whenExCalled, argStr) = (false, false, null);

        void whenValue(string s) =>
            (argStr, whenValueCalled) = (s, true);

        void whenEx(Exception ex) =>
            (argEx, whenExCalled) = (ex, true);

        var valAction = whenValue;
        var exAction = whenEx;

        Reset();
        var u = e1.Match(valAction.ToFunc(), exAction.ToFunc());
        Assert.Equal(Unit.Instance, u);
        Assert.True(whenValueCalled);
        Assert.False(whenExCalled);
        Assert.Equal("HelloWorld", argStr);

        Reset();
        u = e2.Match(valAction.ToFunc(), exAction.ToFunc());
        Assert.Equal(Unit.Instance, u);
        Assert.False(whenValueCalled);
        Assert.True(whenExCalled);
        Assert.Equal(ex, argEx);
    }

    [Fact]
    public async Task Result_MatchActionAsync()
    {
        var ex = new Exception("Something happened");
        var e1 = new Result<string>("HelloWorld");
        var e2 = new Result<string>(ex);

        bool whenValueCalled;
        bool whenExCalled;
        string? argStr;
        Exception argEx = new();

        void Reset() =>
            (whenValueCalled, whenExCalled, argStr) = (false, false, null);

        void whenValue(string s) =>
            (argStr, whenValueCalled) = (s, true);

        void whenEx(Exception ex) =>
            (argEx, whenExCalled) = (ex, true);

        var valAction = whenValue;
        var exAction = whenEx;

        Reset();
        var u = await e1.Match(valAction.ToFuncAsync(), exAction.ToFuncAsync());
        Assert.Equal(Unit.Instance, u);
        Assert.True(whenValueCalled);
        Assert.False(whenExCalled);
        Assert.Equal("HelloWorld", argStr);

        Reset();
        u = await e2.Match(valAction.ToFuncAsync(), exAction.ToFuncAsync());
        Assert.Equal(Unit.Instance, u);
        Assert.False(whenValueCalled);
        Assert.True(whenExCalled);
        Assert.Equal(ex, argEx);
    }

    [Fact]
    public void Result_MatchFunc()
    {
        var ex = new Exception("Something happened");
        var e1 = new Result<string>("HelloWorld");
        var e2 = new Result<string>(ex);

        int whenValue(string s) => s.Length;
        int whenEx(Exception ex) => ex.Message.Length;

        var r1 = e1.Match(whenValue, whenEx);
        var r2 = e2.Match(whenValue, whenEx);

        Assert.Equal("HelloWorld".Length, r1);
        Assert.Equal(ex.Message.Length, r2);
    }

    [Fact]
    public async Task Result_MatchFuncAsync()
    {
        var ex = new Exception("Something happened");
        var e1 = new Result<string>("HelloWorld");
        var e2 = new Result<string>(ex);

        Task<int> whenValue(string s) => Task.Run(() => s.Length);
        Task<int> whenEx(Exception ex) => Task.Run(() => ex.Message.Length);

        var r1 = await e1.Match(whenValue, whenEx);
        var r2 = await e2.Match(whenValue, whenEx);

        Assert.Equal("HelloWorld".Length, r1);
        Assert.Equal(ex.Message.Length, r2);
    }

    [Fact]
    public void Result_Map()
    {
        var ex = new Exception("Something happened");
        var e1 = new Result<string>("HelloWorld");
        var e2 = new Result<string>(ex);
        static int StrLen(string s) => s.Length;
        int StrThrow(string s) => throw ex;

        var r1 = e1.Map(StrLen);
        var r2 = e2.Map(StrLen);
        var r3 = e1.Map(StrThrow);
        var r4 = e2.Map(StrThrow);

        Assert.True(r1.IsValue);
        Assert.Equal("HelloWorld".Length, r1.Value);
        Assert.False(r2.IsValue);
        Assert.Equal(ex, r2.Exception);
        Assert.False(r3.IsValue);
        Assert.Equal(ex, r3.Exception);
        Assert.False(r4.IsValue);
        Assert.Equal(ex, r4.Exception);
    }

    [Fact]
    public async Task Result_MapAsync()
    {
        var ex = new Exception("Something happened");
        var e1 = new Result<string>("HelloWorld");
        var e2 = new Result<string>(ex);
        static Task<int> StrLen(string s) => Task.Run(() => s.Length);
        int StrThrow(string s) => throw ex;

        var r1 = await e1.Map(StrLen);
        var r2 = await e2.Map(StrLen);
        var r3 = await e1.Map(v => Task.Run(() => StrThrow(v)));
        var r4 = await e2.Map(v => Task.Run(() => StrThrow(v)));

        Assert.True(r1.IsValue);
        Assert.Equal("HelloWorld".Length, r1.Value);
        Assert.False(r2.IsValue);
        Assert.Equal(ex, r2.Exception);
        Assert.False(r3.IsValue);
        Assert.Equal(ex, r3.Exception);
        Assert.False(r4.IsValue);
        Assert.Equal(ex, r4.Exception);
    }

    [Fact]
    public void Result_Bind()
    {
        var ex = new Exception("Something happened");
        var e1 = new Result<string>("HelloWorld");
        var e2 = new Result<string>(ex);
        static Result<int> StrLen(string s) => new(s.Length);

        var r1 = e1.Bind(StrLen);
        var r2 = e2.Bind(StrLen);

        Assert.True(r1.IsValue);
        Assert.Equal("HelloWorld".Length, r1.Value);
        Assert.False(r2.IsValue);
        Assert.Equal(ex, r2.Exception);
    }

    [Fact]
    public async Task Result_BindAsync()
    {
        var ex = new Exception("Something happened");
        var e1 = new Result<string>("HelloWorld");
        var e2 = new Result<string>(ex);
        static Task<Result<int>> StrLen(string s) => Task.Run(() => new Result<int>(s.Length));

        var r1 = await e1.Bind(StrLen);
        var r2 = await e2.Bind(StrLen);
        var r3 = await e1.Bind(v => Task.FromException<Result<int>>(ex));
        var r4 = await e2.Bind(v => Task.FromException<Result<int>>(ex));

        Assert.True(r1.IsValue);
        Assert.Equal("HelloWorld".Length, r1.Value);
        Assert.False(r2.IsValue);
        Assert.Equal(ex, r2.Exception);
        Assert.False(r3.IsValue);
        Assert.Equal(ex, r3.Exception);
        Assert.False(r4.IsValue);
        Assert.Equal(ex, r4.Exception);
    }

    [Fact]
    public async Task Result_DefaultIfException()
    {
        var defaultVal = 1;
        var ex = new Exception("Something happened");
        var e1 = new Result<int>(5);
        var e2 = new Result<int>(ex);
        int DefaultThrow() => throw ex;

        var r1 = e1.DefaultIfException(defaultVal);
        var r2 = e1.DefaultIfException(() => defaultVal);
        var r3 = await e1.DefaultIfException(() => Task.FromResult(defaultVal));
        var d1 = e2.DefaultIfException(defaultVal);
        var d2 = e2.DefaultIfException(() => defaultVal);
        var d3 = await e2.DefaultIfException(() => Task.FromResult(defaultVal));
        var d4 = e2.DefaultIfException(DefaultThrow);
        var d5 = await e2.DefaultIfException(() => Task.FromException<int>(ex));

        Assert.True(r1.IsValue);
        Assert.True(r2.IsValue);
        Assert.True(r3.IsValue);
        Assert.True(d1.IsValue);
        Assert.True(d2.IsValue);
        Assert.True(d3.IsValue);
        Assert.False(d4.IsValue);
        Assert.False(d5.IsValue);

        Assert.Equal(e1.Value, r1.Value);
        Assert.Equal(e1.Value, r2.Value);
        Assert.Equal(e1.Value, r3.Value);

        Assert.Equal(defaultVal, d1.Value);
        Assert.Equal(defaultVal, d2.Value);
        Assert.Equal(defaultVal, d3.Value);
        Assert.Equal(ex, d4.Exception);
        Assert.Equal(ex, d5.Exception);
    }

    [Fact]
    public void Result_Static()
    {
        var ex = new Exception("Something happened");
        var e1 = Result.Value(10);
        var e2 = Result.Exception<int>(ex);

        Assert.True(e1.IsValue);
        Assert.Equal(10, e1.Value);
        Assert.False(e2.IsValue);
    }

    [Fact]
    public void Result_Try()
    {
        static int InvalidOp() => (new[] { 10 })[2];
        static int LengthOf(string? s) => s!.Length;

        var e1 = Result.Try(() => 10);
        var e2 = Result.Try(InvalidOp);
        var e3 = Result.Try(LengthOf, "Hello");
        var e4 = Result.Try(LengthOf, "");
        var e5 = Result.Try<string?, int>(LengthOf, null);

        Assert.True(e1.IsValue);
        Assert.Equal(10, e1.Value);
        Assert.False(e2.IsValue);
        Assert.True(e3.IsValue);
        Assert.Equal("Hello".Length, e3.Value);
        Assert.True(e4.IsValue);
        Assert.Equal(0, e4.Value);
        Assert.False(e5.IsValue);
        Assert.NotNull(e5.Exception);
    }

    [Fact]
    public async Task Result_TryAsync()
    {
        static Task<int> InvalidOp() => Task.Run(() => (new[] { 10 })[2]);
        static Task<int> LengthOf(string? s) => Task.Run(() => s!.Length);
        var e1 = await Result.Try(() => Task.FromResult(10));
        var e2 = await Result.Try(InvalidOp);
        var e3 = await Result.Try(LengthOf, "Hello");
        var e4 = await Result.Try(LengthOf, "");
        var e5 = await Result.Try<string?, int>(LengthOf, null);

        Assert.True(e1.IsValue);
        Assert.Equal(10, e1.Value);
        Assert.False(e2.IsValue);
        Assert.True(e3.IsValue);
        Assert.Equal("Hello".Length, e3.Value);
        Assert.True(e4.IsValue);
        Assert.Equal(0, e4.Value);
        Assert.False(e5.IsValue);
        Assert.NotNull(e5.Exception);
    }
}
