using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class TaskLINQ
    {
        public static async Task<T2> Select<T1, T2>(this Task<T1> t, Func<T1, T2> f) => await t.Map(f);

        public static async Task<T3> SelectMany<T1, T2, T3>(this Task<T1> t, Func<T1, Task<T2>> f, Func<T1, T2, T3> g) 
            => await t.Bind(v1 => f(v1).Bind(v2 => Task.FromResult(g(v1, v2))));
    }
}