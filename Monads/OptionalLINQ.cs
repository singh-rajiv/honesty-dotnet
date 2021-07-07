using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class OptionalLINQ
    {
        public static Optional<T2> Select<T1, T2>(this Optional<T1> o, Func<T1, T2> f) => o.Map(f);

        public static Optional<T3> SelectMany<T1, T2, T3>(this Optional<T1> o, Func<T1, Optional<T2>> f, Func<T1, T2, T3> g) 
            => o.Bind(v1 => f(v1).Bind(v2 => g(v1, v2).ToOptional()));
    }
}