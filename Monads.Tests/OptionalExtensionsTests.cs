using System;
using System.Threading.Tasks;
using Xunit;

namespace HonestyDotNet.Monads.Tests
{
    public class OptionalExtensionsTests
    {
        [Fact]
        public void Optional_IfTrue()
        {
            var flag = true;
            var o1 = flag.IfTrue(() => 100);
            flag = false;
            var o2 = flag.IfTrue(() => 10);
            Assert.True(o1.IsSome);
            Assert.Equal(100, o1.Value);
            Assert.False(o2.IsSome);

            flag = true;
            var o3 = flag.IfTrue(() => Optional.Some(50));
            flag = false;
            var o4 = flag.IfTrue(() => Optional.Some(20));
            Assert.True(o3.IsSome);
            Assert.Equal(50, o3.Value);
            Assert.False(o4.IsSome);           
        }

        [Fact]
        public void Optional_Flatten()
        {
            var oo1 = Optional.Some(Optional.Some(60));
            var oo2 = Optional.None(Optional.Some(60));

            var o1 = oo1.Flatten();
            Assert.True(o1.IsSome);
            Assert.Equal(o1.GetType(), oo1.Value.GetType());
            Assert.Equal(60, o1.Value);

            var o2 = oo2.Flatten();
            Assert.False(o2.IsSome);
        }

        [Fact]
        public void Optional_ToOptional()
        {
            var x = 7;
            var s1 = "Hello";
            string s2 = null;
            var ox = x.ToOptional();
            var os1 = s1.ToOptional();
            var os2 = s2.ToOptional();

            Assert.True(ox.IsSome);
            Assert.True(os1.IsSome);
            Assert.False(os2.IsSome);
            Assert.Equal(x, ox.Value);
            Assert.Equal(s1, os1.Value);
        }
    }
}