using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    /// <summary>
    /// Methods in this class have been defined to enable LINQ query syntax on Error type.
    /// These methods should not generally be called directly from user code.
    /// LINQ queries on Error type will attach to these methods using duck typing.
    /// </summary>
    public static class ErrorLINQ
    {
        /// <summary>
        /// Not to be called directly. Use Map if using method syntax.
        /// </summary>
        public static Error<T2> Select<T1, T2>(this Error<T1> e, Func<T1, T2> f) => e.Map(f);

        /// <summary>
        /// Not to be called directly. Use Bind if using method syntax.
        /// </summary>
        public static Error<T3> SelectMany<T1, T2, T3>(this Error<T1> e, Func<T1, Error<T2>> f, Func<T1, T2, T3> g) 
            => e.Bind(v1 => f(v1).Bind(v2 => g(v1, v2).ToError()));
   }
}