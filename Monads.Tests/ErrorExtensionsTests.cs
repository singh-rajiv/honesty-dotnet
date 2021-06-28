using System;
using System.Threading.Tasks;
using Xunit;

namespace HonestyDotNet.Monads.Tests
{
    public class ErrorExtensionsTests
    {
        [Fact]
        public void Error_ToError()
        {
            var ex = new Exception("Something happened");
            var i = 5;
            var s = "Hellow World";
            var o1 = Optional.Some(10);
            var o2 = Optional<int>.None;

            var e1 = i.ToError();
            var e2 = s.ToError();
            var e3 = ex.ToError<int>();
            var e4 = o1.ToError(() => ex);
            var e5 = o2.ToError(() => ex);

            Assert.True(e1.IsValue);
            Assert.True(e2.IsValue);
            Assert.False(e3.IsValue);
            Assert.True(e4.IsValue);
            Assert.False(e5.IsValue);
            Assert.Equal(ex, e3.Ex);
            Assert.Equal(ex, e5.Ex);
        }

        [Fact]
        public void Error_Flatten()
        {
            var ee = Error.Value(Error.Value(10));
            var e = ee.Flatten();
            Assert.True(e.IsValue);
            Assert.Equal(ee.Value.GetType(), e.GetType());
            Assert.Equal(10, e.Value);
        }
    }
}