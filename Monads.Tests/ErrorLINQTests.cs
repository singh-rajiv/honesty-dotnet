using System;
using System.Threading.Tasks;
using System.Linq;
using Xunit;

namespace HonestyDotNet.Monads.Tests
{
    public class ErrorLINQTests
    {
        [Fact]
        public void Error_Select()
        {
            var hw = "Hello World";
            var ex = new Exception("Something happened");
            var e1 = Error.Value(hw);
            var e2 = Error.Ex<string>(ex);

            var l1 = from s in e1 
                     select s.Length;

            var l2 = from s in e2 
                     select s.Length;

            Assert.True(l1.IsValue);
            Assert.Equal(hw.Length, l1.Value);
            Assert.False(l2.IsValue);
            Assert.Equal(ex, l2.Ex);
        }

        [Fact]
        public void Error_SelectMany()
        {
            var h = "Hello ";
            var w = "World!";
            var ex = new Exception("Something happened");
            var e1 = Error.Value(h);
            var e2 = Error.Value(w);
            var e3 = Error.Ex<string>(ex);

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

            var e7 = from x in Error.Value(5)
                     from y in Error.Value(10)
                     from z in Error.Value(15)
                     select x + y + z;

            Assert.True(e4.IsValue);
            Assert.Equal(h + w, e4.Value);
            Assert.False(e5.IsValue);
            Assert.Equal(ex, e5.Ex);
            Assert.False(e6.IsValue);
            Assert.Equal(ex, e6.Ex);
            Assert.True(e7.IsValue);
            Assert.Equal(30, e7.Value);
        }

        [Fact]
        public async Task Error_SelectAsync()
        {
            var sHello = "Hello";
            string sNull = null;
            
            async Task<int> AsyncCodeOf(string s) => await Task.Run(() 
                => { return (int) Math.Sqrt(s.GetHashCode()); });
            
            async Task<int> AsyncSquare(int i) => await Task.Run(() => { return i * i; });
            
            var r1 = await 
                    (from v in await Error.Try(AsyncCodeOf, sHello)
                     select AsyncSquare(v));

            var r2 = await 
                    (from v in await Error.Try(AsyncCodeOf, sNull)
                     select AsyncSquare(v));
            
            Assert.True(r1.IsValue);
            Assert.False(r2.IsValue);
            Assert.NotNull(r2.Ex);
        }

        [Fact]
        public async Task Error_SelectManyAsyc()
        {
            var sHello = "Hello";
            var sWorld = "World";
            string sNull = null;
            
            async Task<int> AsyncCodeOf(string s) => await Task.Run(() 
                => { return (int) Math.Sqrt(s.GetHashCode()); });
            
            async Task<int> AsyncSum(params int[] args) => await Task.Run(() => { return args.Sum(); });

            var r1 = await
                     (from v1 in await Error.Try(AsyncCodeOf, sHello)
                      from v2 in       Error.Try(AsyncCodeOf, sWorld)
                      select AsyncSum(v1, v2));

            var r2 = await
                     (from v1 in await Error.Try(AsyncCodeOf, sHello)
                      from v2 in       Error.Try(AsyncCodeOf, sNull)
                      select AsyncSum(v1, v2));
        }
    }
}