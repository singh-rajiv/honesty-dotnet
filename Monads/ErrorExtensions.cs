using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class ErrorExtensions
    {
        public static Error<T> ToError<T>(this T val) => new (val);

        public static Error<T> ToError<T>(this Exception ex) => new (ex);

        public static Error<T> ToError<T>(this Optional<T> o, Func<Exception> whenNone) =>
            o.Match(val => new Error<T>(val), () => new Error<T>(whenNone()));

        public static Error<T> Flatten<T>(this Error<Error<T>> ee) => ee.Bind(e => e);
    }
}