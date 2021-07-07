using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class ErrorLINQ
    {
        public static Error<T2> Select<T1, T2>(this Error<T1> e, Func<T1, T2> f) => e.Map(f);

        public static Error<T3> SelectMany<T1, T2, T3>(this Error<T1> e, Func<T1, Error<T2>> f, Func<T1, T2, T3> g) 
            => e.Bind(v1 => f(v1).Bind(v2 => g(v1, v2).ToError()));
   }
}