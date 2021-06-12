using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class OptionalExtensions
    {
        public static Optional<T> ToOptional<T>(this T val) => val;
        
        public static Optional<T> IfTrue<T>(this bool b, Func<T> f) => b ? f() : Optional<T>.None;

        public static Optional<T> IfTrue<T>(this bool b, Func<Optional<T>> f) => b ? f() : Optional<T>.None;

        public static Optional<T> Flatten<T>(this Optional<Optional<T>> oo) => oo.Bind(oo => oo);
    }
}