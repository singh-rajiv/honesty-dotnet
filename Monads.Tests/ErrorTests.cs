using System;
using System.Threading.Tasks;
using Xunit;

namespace HonestyDotNet.Monads.Tests
{
    public class ErrorTests
    {
        [Fact]
        public void Error_Ctor()
        {
            string emptyString = string.Empty;
            string nullString = null;
            Exception nullEx = null;
            var ex = new Exception("Something happened");
            var e1 = new Error<int>(5);
            var e2 = new Error<int>(ex);
            var e3 = new Error<string>(emptyString);
            var e4 = new Error<string>(nullString);
            var e5 = new Error<string>(nullEx);

            Assert.True(e1.IsValue);
            Assert.Null(e1.Exception);
            Assert.False(e2.IsValue);
            Assert.NotNull(e2.Exception);
            Assert.Equal(ex, e2.Exception);
            Assert.True(e3.IsValue);
            Assert.Null(e3.Exception);
            Assert.False(e4.IsValue);
            Assert.IsType<ArgumentNullException>(e4.Exception);
            Assert.False(e5.IsValue);
            Assert.IsType<ArgumentNullException>(e5.Exception);
        }

        [Fact]
        public void Error_MatchAction()
        {
            var ex = new Exception("Something happened");
            var e1 = new Error<string>("HelloWorld");
            var e2 = new Error<string>(ex);

            bool whenValueCalled;
            bool whenExCalled;
            string argStr;
            Exception argEx;
            
            void Reset()
            {
                whenValueCalled = false;
                whenExCalled = false;
                argStr = null;
                argEx = null;
            }

            void whenValue(string s)
            {
                argStr = s;
                whenValueCalled = true;
            }

            void whenEx(Exception ex) 
            {
                argEx = ex; 
                whenExCalled = true; 
            }

            Reset();
            e1.Match(whenValue, whenEx);
            Assert.True(whenValueCalled);
            Assert.False(whenExCalled);
            Assert.Equal("HelloWorld", argStr);

            Reset();
            e2.Match(whenValue, whenEx);
            Assert.False(whenValueCalled);
            Assert.True(whenExCalled);
            Assert.Equal(ex, argEx);
        }

        [Fact]
        public async Task Error_MatchActionAsync()
        {
            var ex = new Exception("Something happened");
            var e1 = new Error<string>("HelloWorld");
            var e2 = new Error<string>(ex);

            bool whenValueCalled;
            bool whenExCalled;
            string argStr;
            Exception argEx;
            
            void Reset()
            {
                whenValueCalled = false;
                whenExCalled = false;
                argStr = null;
                argEx = null;
            }

            Task whenValue(string s) => Task.Run(() =>
            {
                argStr = s;
                whenValueCalled = true;
            });

            Task whenEx(Exception ex) => Task.Run(() =>
            {
                argEx = ex; 
                whenExCalled = true; 
            });

            Reset();
            await e1.Match(whenValue, whenEx);
            Assert.True(whenValueCalled);
            Assert.False(whenExCalled);
            Assert.Equal("HelloWorld", argStr);

            Reset();
            await e2.Match(whenValue, whenEx);
            Assert.False(whenValueCalled);
            Assert.True(whenExCalled);
            Assert.Equal(ex, argEx);
        }

        [Fact]
        public void Error_MatchFunc()
        {
            var ex = new Exception("Something happened");
            var e1 = new Error<string>("HelloWorld");
            var e2 = new Error<string>(ex);
           
            int whenValue(string s) => s.Length;
            int whenEx(Exception ex) => ex.Message.Length;

            var r1 = e1.Match(whenValue, whenEx);
            var r2 = e2.Match(whenValue, whenEx);

            Assert.Equal("HelloWorld".Length, r1);
            Assert.Equal(ex.Message.Length, r2);
        }

        [Fact]
        public async Task Error_MatchFuncAsync()
        {
            var ex = new Exception("Something happened");
            var e1 = new Error<string>("HelloWorld");
            var e2 = new Error<string>(ex);
           
            Task<int> whenValue(string s) => Task.Run(() => s.Length);
            Task<int> whenEx(Exception ex) => Task.Run(() => ex.Message.Length);

            var r1 = await e1.Match(whenValue, whenEx);
            var r2 = await e2.Match(whenValue, whenEx);

            Assert.Equal("HelloWorld".Length, r1);
            Assert.Equal(ex.Message.Length, r2);
        }

        [Fact]
        public void Error_Map()
        {
            var ex = new Exception("Something happened");
            var e1 = new Error<string>("HelloWorld");
            var e2 = new Error<string>(ex);
            static int StrLen(string s) => s.Length;
            int StrThrow(string s)
            {
                throw ex;
            }

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
        public async Task Error_MapAsync()
        {
            var ex = new Exception("Something happened");
            var e1 = new Error<string>("HelloWorld");
            var e2 = new Error<string>(ex);
            static Task<int> StrLen(string s) => Task.Run(() => s.Length);
            int StrThrow(string s)
            {
                throw ex;
            }

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
        public void Error_Bind()
        {
            var ex = new Exception("Something happened");
            var e1 = new Error<string>("HelloWorld");
            var e2 = new Error<string>(ex);
            static Error<int> StrLen(string s) => new (s.Length);        

            var r1 = e1.Bind(StrLen);
            var r2 = e2.Bind(StrLen);

            Assert.True(r1.IsValue);
            Assert.Equal("HelloWorld".Length, r1.Value);
            Assert.False(r2.IsValue);
            Assert.Equal(ex, r2.Exception);
        }

        [Fact]
        public async Task Error_BindAsync()
        {
            var ex = new Exception("Something happened");
            var e1 = new Error<string>("HelloWorld");
            var e2 = new Error<string>(ex);
            static Task<Error<int>> StrLen(string s) => Task.Run(() => new Error<int>(s.Length));

            var r1 = await e1.Bind(StrLen);
            var r2 = await e2.Bind(StrLen);
            var r3 = await e1.Bind(v => Task.FromException<Error<int>>(ex));
            var r4 = await e2.Bind(v => Task.FromException<Error<int>>(ex));            

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
        public async Task Error_DefaultIfError()
        {
            var defaultVal = 1;
            var ex = new Exception("Something happened");
            var e1 = new Error<int>(5);
            var e2 = new Error<int>(ex);
            int DefaultThrow()
            {
                throw ex;
            }            

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
        public void Error_Static()
        {
            var ex = new Exception("Something happened");
            var e1 = Error.Value(10);
            var e2 = Error.Exception<int>(ex);

            Assert.True(e1.IsValue);
            Assert.Equal(10, e1.Value);
            Assert.False(e2.IsValue);
        }

        [Fact]
        public void Error_Try()
        {
            static int InvalidOp() => (new [] {10})[2];
            static int LengthOf(string s) => s.Length;
            Func<int> nullFunc1 = null;
            Func<string, int> nullFunc2 = null;

            var e1 = Error.Try(() => 10);
            var e2 = Error.Try(InvalidOp);
            var e3 = Error.Try(LengthOf, "Hello");
            var e4 = Error.Try(LengthOf, "");
            var e5 = Error.Try<string, int>(LengthOf, null);
            var e6 = Error.Try(nullFunc1);
            var e7 = Error.Try(nullFunc2, "Hello");

            Assert.True(e1.IsValue);
            Assert.Equal(10, e1.Value);
            Assert.False(e2.IsValue);
            Assert.True(e3.IsValue);
            Assert.Equal("Hello".Length, e3.Value);
            Assert.True(e4.IsValue);
            Assert.Equal(0, e4.Value);
            Assert.False(e5.IsValue);
            Assert.NotNull(e5.Exception);
            Assert.False(e6.IsValue);
            Assert.IsType<ArgumentNullException>(e6.Exception);
            Assert.False(e7.IsValue);
            Assert.IsType<NullReferenceException>(e7.Exception);
        }

        [Fact]
        public async Task Error_TryAsync()
        {
            static Task<int> InvalidOp() => Task.Run(() => (new [] {10})[2]);
            static Task<int> LengthOf(string s) => Task.Run(() => s.Length);
            var e1 = await Error.Try(() => Task.FromResult(10));
            var e2 = await Error.Try(InvalidOp);
            Func<Task<int>> nullFunc1 = null;
            Func<string, Task<int>> nullFunc2 = null;
            var e3 = await Error.Try(LengthOf, "Hello");
            var e4 = await Error.Try(LengthOf, "");
            var e5 = await Error.Try<string, int>(LengthOf, null);
            var e6 = await Error.Try(nullFunc1);
            var e7 = await Error.Try(nullFunc2, "Hello");

            Assert.True(e1.IsValue);
            Assert.Equal(10, e1.Value);
            Assert.False(e2.IsValue);
            Assert.True(e3.IsValue);
            Assert.Equal("Hello".Length, e3.Value);
            Assert.True(e4.IsValue);
            Assert.Equal(0, e4.Value);
            Assert.False(e5.IsValue);
            Assert.NotNull(e5.Exception);
            Assert.False(e6.IsValue);
            Assert.IsType<ArgumentNullException>(e6.Exception);
            Assert.False(e7.IsValue);
            Assert.IsType<NullReferenceException>(e7.Exception);
        }
    }
}