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
            var o2 = Optional.Try<string>(() => { throw new Exception(); });

            var ol1 =   from s in o1 
                        select s.Length;

            var ol2 =   from s in o2 
                        select s.Length;

            Assert.True(ol1.IsSome);
            Assert.Equal(hw.Length, ol1.Value);                        
            Assert.False(ol2.IsSome);
        }

        [Fact]
        public void Optional_SelectManyT()
        {
            var h = "Hello ";
            var w = "World!";
            var oh = Optional.Try(() => h);
            var ow = Optional.Try(() => w);
            var ox = Optional.Try<string>(() => { throw new Exception(); });

            var ohw =   from s1 in oh
                        from s2 in ow
                        select s1 + s2;

            var ohx =   from s1 in oh
                        from s2 in ox
                        select s1 + s2;

            Assert.True(ohw.IsSome);
            Assert.Equal(h + w, ohw.Value);
            Assert.False(ohx.IsSome);            
        }

        [Fact]
        public void Optional_SelectManyT1T2()
        {
            var h = "High ";
            var f = 5;
            var oh = Optional.Try(() => h);
            var of = Optional.Try(() => f);
            var ox = Optional.Try<string>(() => { throw new Exception(); });

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
            var o1 = 10.ToOptional();
            async Task<string> ComputeString(int x) => await Task.Run(() => { return x.ToString(); });
            
            var r1 = await (from v in o1
                     select ComputeString(v));
            
            Assert.True(r1.IsSome);
            Assert.Equal("10", r1.Value);
        }

        [Fact]
        public async Task Optional_SelectManyAsyc()
        {
            var o1 = Optional.Some(10);
            async Task<Optional<string>> ComputeString(int x) => await Task.Run(() => { return x.ToString(); });
            async Task<bool> LengthCheck(int a, string s) => await Task.Run(() => a + s.Length > 7);

            var r1 = await
                     (from v1 in o1
                     from v2 in ComputeString(v1)
                     select LengthCheck(v1, v2));
            Assert.True(r1.IsSome);
            Assert.True(r1.Value);
        }
    }
}