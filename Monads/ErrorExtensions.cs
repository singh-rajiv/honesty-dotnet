#pragma warning disable CS1591
namespace Monads;

public static class ResultExtensions
{
    /// <summary>
    /// Amplifies the given value to a Result.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="val">Value to be amplified.</param>
    /// <returns>Result containing the value.</returns>
    public static Result<T> ToResult<T>(this T? val) => new(val);

    /// <summary>
    /// Amplifies the given exception to a Result.
    /// </summary>
    /// <typeparam name="T">Type of value if it was present.</typeparam>
    /// <param name="ex">Exception to be amplified.</param>
    /// <returns>Result containing the exception.</returns>
    public static Result<T> ToResult<T>(this Exception ex) => new(ex);

    /// <summary>
    /// Converts an Optional to Result setting the given exception if value is not present.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="o">Optional to convert.</param>
    /// <param name="ex">Exception to set if the Optional does not contain a value.</param>
    /// <returns>A Result containing the Optional's value if present or the given exception.</returns>
    public static Result<T> ToResult<T>(this Optional<T> o, Exception ex) 
        => o.IsSome ? new(o.Value) : new(ex);

    /// <summary>
    /// Unwraps a Result of a Result to just a Result.
    /// </summary>
    /// <typeparam name="T">Type of contained value.</typeparam>
    /// <param name="ee">Result of a Result.</param>
    /// <returns>Inner Result.</returns>
    public static Result<T> Flatten<T>(this Result<Result<T>> ee) => ee.IsValue ? ee.Value : new(ee.Exception);
}
