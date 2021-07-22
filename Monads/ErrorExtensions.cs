using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class ErrorExtensions
    {
        /// <summary>
        /// Amplifies the given value to an Error<T>.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="val">Value to amplify.</param>
        /// <returns>Error<T> containing the value.</returns>
        public static Error<T> ToError<T>(this T val) => new (val);

        /// <summary>
        /// Amplifies the given exception to an Error<T>.
        /// </summary>
        /// <typeparam name="T">Type of value if it was present.</typeparam>
        /// <param name="ex">Exception to amplify.</param>
        /// <returns>Error<T> containing the exception.</returns>
        public static Error<T> ToError<T>(this Exception ex) => new (ex);

        /// <summary>
        /// Converts an Optional<T> to Error<T> setting the given exception if value is not present.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="o">Optional to convert.</param>
        /// <param name="ex">Exception to set if the Optional does not contain a value.</param>
        /// <returns>An Error<T> containing the Optional's value if present or the given exception.</returns>
        public static Error<T> ToError<T>(this Optional<T> o, Exception ex) =>
            o.Match<Error<T>>(val => new (val), () => new (ex));

        /// <summary>
        /// Unwraps an Error<Error<T>> to Error<T>.
        /// </summary>
        /// <typeparam name="T">Type of the underlying value.</typeparam>
        /// <param name="ee">Error<Error<T>></param>
        /// <returns>Inner Error<T>.</returns>
        public static Error<T> Flatten<T>(this Error<Error<T>> ee) => ee.IsValue ? ee.Value : new (ee.Exception);
    }
}