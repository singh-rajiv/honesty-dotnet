#pragma warning disable CS1591
namespace Monads;

public static class ErrorExtensions
{
    /// <summary>
    /// Amplifies the given value to an Error.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="val">Value to be amplified.</param>
    /// <returns>Error containing the value.</returns>
    public static Error<T> ToError<T>(this T val) => new(val);

    /// <summary>
    /// Amplifies the given exception to an Error.
    /// </summary>
    /// <typeparam name="T">Type of value if it was present.</typeparam>
    /// <param name="ex">Exception to be amplified.</param>
    /// <returns>Error containing the exception.</returns>
    public static Error<T> ToError<T>(this Exception ex) => new(ex);

    /// <summary>
    /// Converts an Optional to Error setting the given exception if value is not present.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="o">Optional to convert.</param>
    /// <param name="ex">Exception to set if the Optional does not contain a value.</param>
    /// <returns>An Error containing the Optional's value if present or the given exception.</returns>
    public static Error<T> ToError<T>(this Optional<T> o, Exception ex) =>
        o.Match<Error<T>>(val => new(val), () => new(ex));

    /// <summary>
    /// Unwraps an Error of an Error to just an Error.
    /// </summary>
    /// <typeparam name="T">Type of contained value.</typeparam>
    /// <param name="ee">Error of an Error.</param>
    /// <returns>Inner Error.</returns>
    public static Error<T> Flatten<T>(this Error<Error<T>> ee) => ee.IsValue ? ee.Value : new(ee.Exception);
}
