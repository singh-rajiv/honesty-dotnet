using System;
using System.Threading.Tasks;
using Xunit;

namespace HonestyDotNet.Monads.Tests
{
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

            Assert.Equal(o1, o1);
            Assert.Equal(o2, o2);
            Assert.Equal(o3, o3);

            Assert.Equal(o1, o11);
            Assert.Equal(o2, o22);
            Assert.Equal(o3, o33);
            Assert.Equal(o3, Optional<object>.None);

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
        public void Optional_MatchAction()
        {
            var o1 = new Optional<string>("Test");
            var o2 = new Optional<string>(null);

            bool whenSomeCalled;
            bool whenNoneCalled;
            string input;
            
            void Reset()
            {
                whenSomeCalled = false;
                whenNoneCalled = false;
                input = string.Empty;
            }

            void whenSome(string s)
            {
                input = s;
                whenSomeCalled = true;
            }

            void whenNone() 
            { 
                whenNoneCalled = true; 
            }

            Reset();
            o1.Match(whenSome, whenNone);
            Assert.True(whenSomeCalled);
            Assert.False(whenNoneCalled);
            Assert.Equal(o1.Value, input);

            Reset();
            o2.Match(whenSome, whenNone);
            Assert.False(whenSomeCalled);
            Assert.True(whenNoneCalled);
            Assert.Equal(string.Empty, input);

            Reset();
            o1.Match(whenSome);
            Assert.True(whenSomeCalled);
            Assert.Equal(o1.Value, input);

            Reset();
            o2.Match(whenSome);
            Assert.False(whenSomeCalled);
        }

        [Fact]
        public void Optional_MatchFunc()
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
        public async Task Optional_MatchActionAsync()
        {
            var o1 = new Optional<string>("Test");
            var o2 = new Optional<string>(null);

            bool whenSomeExecuted;
            bool whenNoneExecuted;
            string input;
            
            void Reset()
            {
                whenSomeExecuted = false;
                whenNoneExecuted = false;
                input = string.Empty;
            }

            Task whenSome(string s) => Task.Run(() =>
            {
                input = s;
                whenSomeExecuted = true;
            });

            Task whenNone() => Task.Run(() =>
            { 
                whenNoneExecuted = true; 
            });

            Reset();
            await o1.Match(whenSome, whenNone);
            Assert.True(whenSomeExecuted);
            Assert.False(whenNoneExecuted);
            Assert.Equal(o1.Value, input);

            Reset();
            await o2.Match(whenSome, whenNone);
            Assert.False(whenSomeExecuted);
            Assert.True(whenNoneExecuted);
            Assert.Equal(string.Empty, input);

            Reset();
            await o1.Match(whenSome);
            Assert.True(whenSomeExecuted);
            Assert.Equal(o1.Value, input);

            Reset();
            await o2.Match(whenSome);
            Assert.False(whenSomeExecuted);            
        }

        [Fact]
        public async Task Optional_MatchFuncAsync()
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
        }

        [Fact]
        public async Task Optional_DefaultIfNoneAsync()
        {
            var o1 = await Optional<int>.None.DefaultIfNone(() => Task.FromResult(30));
            Assert.True(o1.IsSome);
            Assert.Equal(30, o1.Value);
        }        

        [Fact]
        public void Optional_Map()
        {
            var o1 = Optional.Try(() => 5);
            var o2 = Optional.Try<int>(() => { throw new Exception(); });
            var os1 = o1.Map(v => v * v);
            var os2 = o2.Map(v => v * v);

            Assert.True(os1.IsSome);
            Assert.False(os2.IsSome);
            Assert.Equal(25, os1.Value);
        }                  

        [Fact]
        public void Optional_Bind()
        {
            var h = "Hello ";
            var w = "World!";
            var oh = Optional.Try(() => h);
            var ow = Optional.Try(() => w);
            var ox = Optional.Try<string>(() => { throw new Exception(); });
            var ohw = oh.Bind(v1 => ow.Map(v2 => v1 + v2));
            var ohx = oh.Bind(v1 => ox.Map(v2 => v1 + v2));

            Assert.True(ohw.IsSome);
            Assert.Equal(h + w, ohw.Value);
            Assert.False(ohx.IsSome);
        }        

        [Fact]
        public void Optional_Static()
        {
            var o1 = Optional.Some(5);
            var o2 = Optional.None(5);
            var o3 = Optional.Try(() => 5);
            var o4 = Optional.Try<int>(() => { throw new Exception(); });
            var o5 = Optional.Try((string s) => s.Length, "Test");
            var o6 = Optional.Try((string s) => s.Length, string.Empty);
            var o7 = Optional.Try((string s) => s.Length, null);

            Assert.True(o1.IsSome);
            Assert.False(o2.IsSome);
            Assert.True(o3.IsSome);
            Assert.Equal(5, o3.Value);
            Assert.False(o4.IsSome);
            Assert.True(o5.IsSome);
            Assert.Equal(4, o5.Value);
            Assert.True(o6.IsSome);
            Assert.Equal(0, o6.Value);
            Assert.False(o7.IsSome);
        }
    }
}
