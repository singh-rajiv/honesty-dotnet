namespace Monads;

/// <summary>
/// Represents an amplified type of T, Optional monad, in which a value may or may not be present.
/// </summary>
/// <typeparam name="T">The type of value.</typeparam>
public readonly struct Optional<T> : IEquatable<Optional<T>>
{
    /// <summary>
    /// Returns the value stored inside the monad. The value is default(T) if not present. 
    /// </summary>
    public T? Value { get; } = default;

    /// <summary>
    /// Returns true if value is present or false otherwise.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSome { get; }

    /// <summary>
    /// Returns the single None instance of Optional.
    /// </summary>
    public static Optional<T> None => new();

    /// <summary>
    /// Creates an instance of Optional monad.
    /// </summary>
    /// <param name="value">Value to store inside the instance. If value is null then IsSome is set to false, otherwise it is set to true.</param>
    public Optional(T? value) => (IsSome, Value) = (value != null, value);

    /// <summary>
    /// Checks for equality with another Optional monad object.
    /// </summary>
    /// <param name="other">Other Optional monad object to compare this object with.</param>
    /// <returns>true if references are equal or both have IsSome false or both have IsSome true and raw values are also equal, false otherwise.</returns>
    public bool Equals(Optional<T> other) => other.IsSome == IsSome && EqualityComparer<T>.Default.Equals(other.Value, Value);

    /// <summary>
    /// Checks for equality with an object.
    /// </summary>
    /// <param name="obj">Object to compare with.</param>
    /// <returns>false if obj is null or its type differs from this object's type. Then further as per Optional equality rules.</returns>
    public override bool Equals(object? obj) => (obj is Optional<T> thisObj) && Equals(thisObj);

    /// <summary>
    /// Returns hashcode.
    /// </summary>
    /// <returns>0 if the Optional does not contain a value, otherwise hashcode of the Value.</returns>
    public override int GetHashCode() => IsSome ? Value.GetHashCode() : 0;

    /// <summary>
    /// Checks if two Optional objects are equal.
    /// </summary>
    /// <param name="first">First Optional object.</param>
    /// <param name="second">Second Optional object.</param>
    /// <returns>true if objects are equal, false otherwise.</returns>
    public static bool operator ==(Optional<T> first, Optional<T> second) => first.Equals(second);

    /// <summary>
    /// Checks if two Optional objects are unequal.
    /// </summary>
    /// <param name="first">First Optional object.</param>
    /// <param name="second">Second Optional object.</param>
    /// <returns>false if objects are equal, true otherwise.</returns>
    public static bool operator !=(Optional<T> first, Optional<T> second) => !first.Equals(second);

    /// <summary>
    /// Automatically converts a value to the Optional type during assignment or passing to a function call if the receiving variables is of Optional type.
    /// </summary>
    /// <param name="val">The value to amplify.</param>
    public static implicit operator Optional<T>(T? val) => new(val);

    /// <summary>
    /// Executes one of the given funcs based on whether Value is present.
    /// </summary>
    /// <typeparam name="TResult">The funcs' return type.</typeparam>
    /// <param name="whenSome">Func to execute on Value, if present.</param>
    /// <param name="whenNone">Func to execute when Value is not present.</param>
    /// <returns>The result of the func that gets executed.</returns>
    public TResult? Match<TResult>(Func<T, TResult?> whenSome, Func<TResult?> whenNone) => IsSome ? whenSome(Value) : whenNone();

    /// <summary>
    /// Executes one of the given funcs asynchronously based on whether Value is present.
    /// </summary>
    /// <typeparam name="TResult">Return type of the result of the asynchronous func call.</typeparam>
    /// <param name="whenSome">The asynchronous func to execute on Value.</param>
    /// <param name="whenNone">The asynchronous func to execute when Value is not present.</param>
    /// <returns>Task yielding the result on completion of the func that gets executed.</returns>
    public async Task<TResult> Match<TResult>(Func<T, Task<TResult>> whenSome, Func<Task<TResult>> whenNone) 
        => IsSome ? await whenSome(Value) : await whenNone();

    /// <summary>
    /// Projects the Value, if present, using the supplied func.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of projection operation.</typeparam>
    /// <param name="f">The func which operates on Value.</param>
    /// <returns>
    /// The result of the projection operation as an Optional.
    /// The Optional is None if Value is not present or the func is null or it throws.
    /// </returns>
    public Optional<TResult> Map<TResult>(Func<T, TResult?> f) => IsSome ? Optional.Try(f, Value) : Optional<TResult>.None;

    /// <summary>
    /// Projects the Value, if present, using the supplied asynchronous func.
    /// </summary>
    /// <typeparam name="TResult">The type of the result yielded by the asynchronous projection operation.</typeparam>
    /// <param name="f">The func which operates on Value asynchronously.</param>
    /// <returns>
    /// Task which yields the result on completion of the asynchronous projection operation as an Optional. 
    /// The Optional is None if Value is not present or the asynchronous operation is cancelled or the func is null or it throws.
    /// </returns>
    public async Task<Optional<TResult>> Map<TResult>(Func<T, Task<TResult?>> f) 
        => IsSome ? await Optional.Try(f, Value) : Optional<TResult>.None;

    /// <summary>
    /// Projects the Value, if present, using the supplied func which also returns an Optional.
    /// Bind also flattens the result. Calling Map using the func will return an Optional of an Optional.
    /// </summary>
    /// <typeparam name="TResult">The type of the raw value of the projection operation.</typeparam>
    /// <param name="f">The func which operates on Value.</param>
    /// <returns>
    /// The result of the projection operation as an Optional.
    /// The Optional is None if Value is not present or the func is null or it throws.
    /// </returns>
    public Optional<TResult> Bind<TResult>(Func<T, Optional<TResult>> f) 
        => IsSome ? Optional.Try(f, Value).Flatten() : Optional<TResult>.None;

    /// <summary>
    /// Projects the Value, if present, using the supplied asynchronous func.
    /// Bind also flattens the result. Calling Map using the func will return a Task of an Optional of an Optional.
    /// </summary>
    /// <typeparam name="TResult">The type of the raw value of the projection operation.</typeparam>
    /// <param name="f">The asynchronous func which operates on Value.</param>
    /// <returns>
    /// The result of the projection operation as an Optional.
    /// The Optional is None if Value is not present or the task is cancelled or the func is null or it throws.
    /// </returns>
    public async Task<Optional<TResult>> Bind<TResult>(Func<T, Task<Optional<TResult>>> f) 
    {
        if (IsSome)
        {
            try
            {
                return await f(Value);
            }
            catch
            {
                return Optional<TResult>.None;
            }
        }
        return Optional<TResult>.None;
    }

    /// <summary>
    /// Executes a predicate on the Value if present.
    /// </summary>
    /// <param name="predicate">Filter condition to execute.</param>
    /// <returns>Same instance of the Optional if Value meets the filter condition, None otherwise.</returns>
    public Optional<T> Where(Func<T, bool> predicate) 
        => IsSome && Optional.Try(predicate, Value).Match(v => v, () => false) ? this : None;

    /// <summary>
    /// Executes a predicate asynchronously on the Value if present.
    /// </summary>
    /// <param name="predicate">Asynchronous filter condition to execute.</param>
    /// <returns>Task yielding same instance of this Optional if Value meets the filter condition, None otherwise.</returns>
    public async Task<Optional<T>> Where(Func<T, Task<bool>> predicate) 
        => IsSome && (await Optional.Try(predicate, Value)).Match(v => v, () => false) ? this : None;

    /// <summary>
    /// Defaults an Optional to contain the specified value if it does not contain Value.
    /// </summary>
    /// <param name="val">Default value.</param>
    /// <returns>Same instance of the Optional if it already contains a value. Otherwise an Optional containing the specified default value.</returns>
    public Optional<T> DefaultIfNone(T val) => IsSome ? this : val;

    /// <summary>
    /// Defaults an Optional to contain the result of the specified func evaluation lazily.
    /// </summary>
    /// <param name="f">func which provides the default value. It is executed only if a value is not present.</param>
    /// <returns>
    /// Same instance of this Optional if it contains a value. Otherwise an Optional containing the result of func evaluation.
    /// If the func is null or throws an exception then the Optional contains None.
    /// </returns>
    public Optional<T> DefaultIfNone(Func<T?> f) => IsSome ? this : Optional.Try(f);

    /// <summary>
    /// Defaults an Optional to contain the result of the specified func evaluation lazily and asynchronously.
    /// </summary>
    /// <param name="f">Asynchronous func to execute only if Value is not present.</param>
    /// <returns>
    /// Same instance of this Optional if it contains Value. 
    /// Otherwise a Task yielding Optional containing the result of func evaluation.
    /// The Optional is None if the task is cancelled or the func is null or it throws.
    /// </returns>
    public async Task<Optional<T>> DefaultIfNone(Func<Task<T?>> f) => IsSome ? this : await Optional.Try(f);
}

/// <summary>
/// Static class providing utility methods to create an instance of Optional monad.
/// </summary>
public static class Optional
{
    /// <summary>
    /// Creates an Optional from the given value.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="value">Value to be amplified.</param>
    /// <returns>An Optional containing value.</returns>
    public static Optional<T> Some<T>(T? value) => value;

    /// <summary>
    /// Creates an empty Optional.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <param name="_">The param value is ignored.</param>
    /// <returns>An empty Optional of type T.</returns>
    public static Optional<T> None<T>(T? _) => Optional<T>.None;

    /// <summary>
    /// Amplifies the result of a function call to an Optional. The function is executed inside a try/catch block.
    /// </summary>
    /// <typeparam name="T">Type of value returned by the function.</typeparam>
    /// <param name="f">Function to execute.</param>
    /// <returns>
    /// Optional containing the result on successful execution or None if function is null or it throws an exception.
    /// The exception information is lost. Use Result.Try and Result monad if exception information is required.
    /// </returns>
    public static Optional<T> Try<T>(Func<T?> f)
    {
        try
        {
            return f();
        }
        catch
        {
            return Optional<T>.None;
        }
    }

    /// <summary>
    /// Amplifies the result of an asynchronous function call to an Optional. 
    /// The function is executed and resulting task awaited inside a try/catch block.
    /// </summary>
    /// <typeparam name="T">Type of the result yielded by the asynchronous function.</typeparam>
    /// <param name="f">Asynchronous function to execute.</param>
    /// <returns>
    /// A task which yields an Optional, containing the result on successful completion of the asynchronous operation, 
    /// or None if the operation fails or task is canceled.
    /// Awaiting the returned task is guaranteed to not fail even if the original task is faulted or cancelled.
    /// The exception information is lost. Use Result.Try and Result monad if exception information is required.
    /// </returns>
    public static async Task<Optional<T>> Try<T>(Func<Task<T?>> f)
    {
        try
        {
            return await f();
        }
        catch
        {
            return Optional<T>.None;
        }
    }

    /// <summary>
    /// Amplifies the result of a function call on given value to an Optional. The function is executed inside a try/catch block.
    /// </summary>
    /// <typeparam name="T1">Type of input value to the function.</typeparam>
    /// <typeparam name="T2">Type of value returned by the function.</typeparam>
    /// <param name="f">Function to execute.</param>
    /// <param name="val">Input parameter to the function.</param>
    /// <returns>
    /// Optional containing the result on successful execution or None if function is null or it throws an exception.
    /// The exception information is lost. Use Result.Try and Result monad if exception information is required.
    /// </returns>
    public static Optional<T2> Try<T1, T2>(Func<T1, T2?> f, T1 val) => Try(() => f(val));

    /// <summary>
    /// Amplifies the result of an asynchronous function call on given value to an Optional. 
    /// The function is executed and resulting task awaited inside a try/catch block.
    /// </summary>
    /// <typeparam name="T1">Type of input value to the asynhronous function.</typeparam>
    /// <typeparam name="T2">Type of the result yielded by the asynchronous function.</typeparam>
    /// <param name="f">Asynchronous function to execute on value.</param>
    /// <param name="val">Input parameter to the function.</param>
    /// <returns>
    /// A task which yields an Optional, containing the result on successful completion of the asynchronous operation, 
    /// or None if the operation fails or task is cancelled.
    /// Awaiting the returned task is guaranteed to not fail even if the original task is faulted or cancelled.
    /// The exception information is lost. Use Result.Try and Result monad if exception information is required.
    /// </returns>
    public static async Task<Optional<T2>> Try<T1, T2>(Func<T1, Task<T2?>> f, T1 val) => await Try(() => f(val));
}
