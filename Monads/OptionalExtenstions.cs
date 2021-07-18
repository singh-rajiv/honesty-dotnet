using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class OptionalExtensions
    {
        /// <summary>
        /// Amplies the given value to an Optional.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="val">Value to be amplified.</param>
        /// <returns>Optional containing the Value.</returns>
        public static Optional<T> ToOptional<T>(this T val) => val;
        
        /// <summary>
        /// Creates an Optional by executing a function if boolean value is true.
        /// </summary>
        /// <typeparam name="T">Type of result returned by function call.</typeparam>
        /// <param name="b">Boolean value.</param>
        /// <param name="f">Function to execute.</param>
        /// <returns>Optional containing value returned by function if boolean is true, None otherwise.</returns>
        public static Optional<T> IfTrue<T>(this bool b, Func<T> f) => b ? f() : Optional<T>.None;

        /// <summary>
        /// Creates an Optional by executing an Optional returning function if boolean value is true.
        /// </summary>
        /// <typeparam name="T">Type of result contained by the Optional returned by the function call.</typeparam>
        /// <param name="b">Boolean value.</param>
        /// <param name="f">Function to execute.</param>
        /// <returns>Optional returned by the function if boolean is true, None otherwise.</returns>
        public static Optional<T> IfTrue<T>(this bool b, Func<Optional<T>> f) => b ? f() : Optional<T>.None;

        /// <summary>
        /// Unwraps an Optional of an Optional to just an Optional.
        /// </summary>
        /// <typeparam name="T">Type of contained value.</typeparam>
        /// <param name="oo">Optional of an Optional.</param>
        /// <returns>Inner Optional.</returns>
        public static Optional<T> Flatten<T>(this Optional<Optional<T>> oo) => oo.Bind(o => o);
    }
}