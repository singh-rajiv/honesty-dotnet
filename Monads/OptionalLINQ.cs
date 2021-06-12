using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class OptionalLINQ
    {
        public static Optional<T2> Select<T1, T2>(this Optional<T1> o, Func<T1, T2> f) => o.Map(f);

        public static async Task<Optional<T2>> Select<T1, T2>(this Optional<T1> o, Func<T1, Task<T2>> f) => await o.Map(f);

        public static Optional<T2> SelectMany<T1, T2>(this Optional<T1> o, Func<T1, Optional<T2>> f) => o.Bind(f);

        public static async Task<Optional<T2>> SelectMany<T1, T2>(this Optional<T1> o, Func<T1, Task<Optional<T2>>> f) => await o.Bind(f);

        public static Optional<T3> SelectMany<T1, T2, T3>(this Optional<T1> o, Func<T1, Optional<T2>> f, Func<T1, T2, T3> g) 
            => o.Bind(v1 => f(v1).Bind(v2 => g(v1, v2).ToOptional()));

        public static async Task<Optional<T3>> SelectMany<T1, T2, T3>(this Optional<T1> o, Func<T1, Task<Optional<T2>>> f, Func<T1, T2, Task<T3>> g) 
            => await o.Bind(async v1 => await (await f(v1)).Bind(async v2 => (await g(v1, v2)).ToOptional()));
    }
}