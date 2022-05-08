namespace Monads.Tests;

public class ResultTests
{
    [Fact]
    public void Result_Ctor()
    {
        var emptyString = string.Empty;
        string? nullString = null;
        var ex = new Exception("Something happened");
        var r1 = new Result<int>(5);
        var r2 = new Result<int>(ex);
        var r3 = new Result<string>(emptyString);
        var r4 = new Result<string>(nullString);

        Assert.True(r1.IsValue);
        Assert.Null(r1.Exception);
        Assert.False(r2.IsValue);
        Assert.NotNull(r2.Exception);
        Assert.Equal(ex, r2.Exception);
        Assert.True(r3.IsValue);
        Assert.Null(r3.Exception);
        Assert.False(r4.IsValue);
        Assert.IsType<ArgumentNullException>(r4.Exception);
    }

    [Fact]
    public void Result_Deconstruct()
    {
        var ex = new Exception("something happened.");
        var r1 = Result.Value("HelloWorld");
        var r2 = Result.Exception<string>(ex);

        var (isValue, value, exception) = r1;
        Assert.True(isValue);
        Assert.Equal("HelloWorld", value);
        Assert.Null(exception);

        (isValue, value, exception) = r2;
        Assert.False(isValue);
        Assert.Null(value);
        Assert.Equal(ex, exception);
    }

    [Fact]
    public void Result_PatternMatch_Action()
    {
        var ex = new Exception("Something happened");
        var r1 = new Result<string>("HelloWorld");
        var r2 = new Result<string>(ex);

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
        var u = r1.Match(valAction.ToFunc(), exAction.ToFunc());
        Assert.Equal(Unit.Instance, u);
        Assert.True(whenValueCalled);
        Assert.False(whenExCalled);
        Assert.Equal("HelloWorld", argStr);

        Reset();
        u = r2.Match(valAction.ToFunc(), exAction.ToFunc());
        Assert.Equal(Unit.Instance, u);
        Assert.False(whenValueCalled);
        Assert.True(whenExCalled);
        Assert.Equal(ex, argEx);
    }

    [Fact]
    public async Task Result_PatternMatch_ActionAsync()
    {
        var ex = new Exception("Something happened");
        var r1 = new Result<string>("HelloWorld");
        var r2 = new Result<string>(ex);

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
        var u = await r1.Match(valAction.ToFuncAsync(), exAction.ToFuncAsync());
        Assert.Equal(Unit.Instance, u);
        Assert.True(whenValueCalled);
        Assert.False(whenExCalled);
        Assert.Equal("HelloWorld", argStr);

        Reset();
        u = await r2.Match(valAction.ToFuncAsync(), exAction.ToFuncAsync());
        Assert.Equal(Unit.Instance, u);
        Assert.False(whenValueCalled);
        Assert.True(whenExCalled);
        Assert.Equal(ex, argEx);
    }

    [Fact]
    public void Result_PatternMatch_Func()
    {
        var ex = new Exception("Something happened");
        var r1 = new Result<string>("HelloWorld");
        var r2 = new Result<string>(ex);

        int whenValue(string s) => s.Length;
        int whenEx(Exception ex) => ex.Message.Length;

        var v1 = r1.Match(whenValue, whenEx);
        var v2 = r2.Match(whenValue, whenEx);

        Assert.Equal("HelloWorld".Length, v1);
        Assert.Equal(ex.Message.Length, v2);
    }

    [Fact]
    public async Task Result_PatternMatch_FuncAsync()
    {
        var ex = new Exception("Something happened");
        var r1 = new Result<string>("HelloWorld");
        var r2 = new Result<string>(ex);

        Task<int> whenValue(string s) => Task.Run(() => s.Length);
        Task<int> whenEx(Exception ex) => Task.Run(() => ex.Message.Length);

        var v1 = await r1.Match(whenValue, whenEx);
        var v2 = await r2.Match(whenValue, whenEx);

        Assert.Equal("HelloWorld".Length, v1);
        Assert.Equal(ex.Message.Length, v2);
    }

    [Fact]
    public void Result_Map()
    {
        var ex = new Exception("Something happened");
        var r1 = new Result<string>("HelloWorld");
        var r2 = new Result<string>(ex);
        static int StrLen(string s) => s.Length;
        int StrThrow(string s) => throw ex;

        var r3 = r1.Map(StrLen);
        var r4 = r2.Map(StrLen);
        var r5 = r1.Map(StrThrow);
        var r6 = r2.Map(StrThrow);

        Assert.True(r3.IsValue);
        Assert.Equal("HelloWorld".Length, r3.Value);
        Assert.False(r4.IsValue);
        Assert.Equal(ex, r4.Exception);
        Assert.False(r5.IsValue);
        Assert.Equal(ex, r5.Exception);
        Assert.False(r6.IsValue);
        Assert.Equal(ex, r6.Exception);
    }

    [Fact]
    public async Task Result_MapAsync()
    {
        var ex = new Exception("Something happened");
        var r1 = new Result<string>("HelloWorld");
        var r2 = new Result<string>(ex);
        static Task<int> StrLen(string s) => Task.Run(() => s.Length);
        int StrThrow(string s) => throw ex;

        var r3 = await r1.Map(StrLen);
        var r4 = await r2.Map(StrLen);
        var r5 = await r1.Map(v => Task.Run(() => StrThrow(v)));
        var r6 = await r2.Map(v => Task.Run(() => StrThrow(v)));

        Assert.True(r3.IsValue);
        Assert.Equal("HelloWorld".Length, r3.Value);
        Assert.False(r4.IsValue);
        Assert.Equal(ex, r4.Exception);
        Assert.False(r5.IsValue);
        Assert.Equal(ex, r5.Exception);
        Assert.False(r6.IsValue);
        Assert.Equal(ex, r6.Exception);
    }

    [Fact]
    public void Result_Bind()
    {
        var ex = new Exception("Something happened");
        var r1 = new Result<string>("HelloWorld");
        var r2 = new Result<string>(ex);
        static Result<int> StrLen(string s) => new(s.Length);

        var r3 = r1.Bind(StrLen);
        var r4 = r2.Bind(StrLen);

        Assert.True(r3.IsValue);
        Assert.Equal("HelloWorld".Length, r3.Value);
        Assert.False(r4.IsValue);
        Assert.Equal(ex, r4.Exception);
    }

    [Fact]
    public async Task Result_BindAsync()
    {
        var ex = new Exception("Something happened");
        var r1 = new Result<string>("HelloWorld");
        var r2 = new Result<string>(ex);
        static Task<Result<int>> StrLen(string s) => Task.Run(() => new Result<int>(s.Length));

        var r3 = await r1.Bind(StrLen);
        var r4 = await r2.Bind(StrLen);
        var r5 = await r1.Bind(v => Task.FromException<Result<int>>(ex));
        var r6 = await r2.Bind(v => Task.FromException<Result<int>>(ex));

        Assert.True(r3.IsValue);
        Assert.Equal("HelloWorld".Length, r3.Value);
        Assert.False(r4.IsValue);
        Assert.Equal(ex, r4.Exception);
        Assert.False(r5.IsValue);
        Assert.Equal(ex, r5.Exception);
        Assert.False(r6.IsValue);
        Assert.Equal(ex, r6.Exception);
    }

    [Fact]
    public async Task Result_DefaultIfException()
    {
        var defaultVal = 1;
        var ex = new Exception("Something happened");
        var r1 = new Result<int>(5);
        var r2 = new Result<int>(ex);
        int DefaultThrow() => throw ex;

        var r3 = r1.DefaultIfException(defaultVal);
        var r4 = r1.DefaultIfException(() => defaultVal);
        var r5 = await r1.DefaultIfException(() => Task.FromResult(defaultVal));

        var r6 = r2.DefaultIfException(defaultVal);
        var r7 = r2.DefaultIfException(() => defaultVal);
        var r8 = await r2.DefaultIfException(() => Task.FromResult(defaultVal));
        var r9 = r2.DefaultIfException(DefaultThrow);
        var r10 = await r2.DefaultIfException(() => Task.FromException<int>(ex));

        Assert.True(r3.IsValue);
        Assert.True(r4.IsValue);
        Assert.True(r5.IsValue);
        Assert.True(r6.IsValue);
        Assert.True(r7.IsValue);
        Assert.True(r8.IsValue);
        Assert.False(r9.IsValue);
        Assert.False(r10.IsValue);

        Assert.Equal(r1.Value, r3.Value);
        Assert.Equal(r1.Value, r4.Value);
        Assert.Equal(r1.Value, r5.Value);

        Assert.Equal(defaultVal, r6.Value);
        Assert.Equal(defaultVal, r7.Value);
        Assert.Equal(defaultVal, r8.Value);
        Assert.Equal(ex, r9.Exception);
        Assert.Equal(ex, r10.Exception);
    }

    [Fact]
    public void Result_Static()
    {
        var ex = new Exception("Something happened");
        var r1 = Result.Value(10);
        var r2 = Result.Exception<int>(ex);

        Assert.True(r1.IsValue);
        Assert.Equal(10, r1.Value);
        Assert.False(r2.IsValue);
    }

    [Fact]
    public void Result_Try()
    {
        static int InvalidOp() => (new[] { 10 })[2];
        static int LengthOf(string? s) => s!.Length;

        var r1 = Result.Try(() => 10);
        var r2 = Result.Try(InvalidOp);
        var r3 = Result.Try(LengthOf, "Hello");
        var r4 = Result.Try(LengthOf, "");
        var r5 = Result.Try<string?, int>(LengthOf, null);

        Assert.True(r1.IsValue);
        Assert.Equal(10, r1.Value);
        Assert.False(r2.IsValue);
        Assert.True(r3.IsValue);
        Assert.Equal("Hello".Length, r3.Value);
        Assert.True(r4.IsValue);
        Assert.Equal(0, r4.Value);
        Assert.False(r5.IsValue);
        Assert.NotNull(r5.Exception);
    }

    [Fact]
    public async Task Result_TryAsync()
    {
        static Task<int> InvalidOp() => Task.Run(() => (new[] { 10 })[2]);
        static Task<int> LengthOf(string? s) => Task.Run(() => s!.Length);
        var r1 = await Result.Try(() => Task.FromResult(10));
        var r2 = await Result.Try(InvalidOp);
        var r3 = await Result.Try(LengthOf, "Hello");
        var r4 = await Result.Try(LengthOf, "");
        var r5 = await Result.Try<string?, int>(LengthOf, null);

        Assert.True(r1.IsValue);
        Assert.Equal(10, r1.Value);
        Assert.False(r2.IsValue);
        Assert.True(r3.IsValue);
        Assert.Equal("Hello".Length, r3.Value);
        Assert.True(r4.IsValue);
        Assert.Equal(0, r4.Value);
        Assert.False(r5.IsValue);
        Assert.NotNull(r5.Exception);
    }
}
