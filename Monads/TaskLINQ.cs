using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    /// <summary>
    /// Methods in this class have been defined to enable LINQ query syntax on Task type.
    /// These methods should not generally be called directly from user code.
    /// LINQ queries on Error type will attach to these methods using duck typing.
    /// </summary>
    public static class TaskLINQ
    {
        /// <summary>
        /// Not to be called directly. Use Map if using method syntax.
        /// </summary>
        public static async Task<T2> Select<T1, T2>(this Task<T1> t, Func<T1, T2> f) => await t.Map(f);

        /// <summary>
        /// Not to be called directly. Use Bind if using method syntax.
        /// </summary>
        public static async Task<T3> SelectMany<T1, T2, T3>(this Task<T1> t, Func<T1, Task<T2>> f, Func<T1, T2, T3> g) 
            => await t.Bind(v1 => f(v1).Bind(v2 => Task.FromResult(g(v1, v2))));
    }
}