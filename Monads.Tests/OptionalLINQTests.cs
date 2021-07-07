#pragma warning disable CS8848
using System;
using System.Threading.Tasks;
using Xunit;

namespace HonestyDotNet.Monads.Tests
{
    public class OptionalLINQTests
    {
        [Fact]
        public void Optional_Select()
        {
            var hw = "Hello World";
            var o1 = Optional.Try(() => hw);
            var o2 = Optional<string>.None;

            var ol1 =   from s in o1 
                        select s.Length;

            var ol2 =   from s in o2 
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

            var ohw =   from s in oh
                        from i in of
                        select s + i;

            var ohx =   from v1 in oh
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
            string sNull = null;

            async Task<int> AsyncCodeOf(string i) => await Task.Run(() 
                => { return (int) Math.Sqrt(i.GetHashCode()); });
            
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
            string sNull = null;

            async Task<int> AsyncCodeOf(string i) => await Task.Run(() 
                => { return (int) Math.Sqrt(i.GetHashCode()); });
            
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
    }
}