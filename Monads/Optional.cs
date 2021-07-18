using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    /// <summary>
    /// Represents an amplified (Optional) T where a value may or may not be present.
    /// </summary>
    /// <typeparam name="T">The type to amplify.</typeparam>
    public class Optional<T> : IEquatable<Optional<T>>
    {
        /// <summary>
        /// Returns the raw value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Returns true if value is present or false otherwise.
        /// </summary>
        public bool IsSome { get; }

        /// <summary>
        /// Returns the single None instance of Optional<T>.
        /// </summary>
        public static Optional<T> None => new();

        /// <summary>
        /// Creates an instance of Optional<T>.
        /// </summary>
        /// <param name="value">The value to amplify. If value is null then IsSome is set to false, otherwise it is set to true.</param>
        public Optional(T value)
        {
            Value = value;
            IsSome = value != null;
        }

        private Optional()
        {
            IsSome = false;
        }

        /// <summary>
        /// Checks for equality with another Optional<T> object.
        /// </summary>
        /// <param name="other">Other Optional<T> object to compare this object with.</param>
        /// <returns>true if references are equal or both have IsSome false or both have IsSome true and raw values are also equal, false otherwise.</returns>
        public bool Equals(Optional<T> other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(other, this) || (!other.IsSome && !IsSome))
                return true;

            return other.IsSome == IsSome && EqualityComparer<T>.Default.Equals(other.Value, Value);
        }

        /// <summary>
        /// Checks for equality with an object
        /// </summary>
        /// <param name="obj">Object to compare with.</param>
        /// <returns>false if obj is null or its type differs from this object's type.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(obj, this))
                return true;
            
            if (obj.GetType() != GetType())
                return false;

            return Equals(obj as Optional<T>);
        }

        /// <summary>
        /// Returns hashcode.
        /// </summary>
        /// <returns>0 if IsSome is false, hashcode of Value property otherwise.</returns>
        public override int GetHashCode() => IsSome ? Value.GetHashCode() : 0;

        /// <summary>
        /// Checks if two Optional objects are equal
        /// </summary>
        /// <param name="first">First Optional object.</param>
        /// <param name="second">Second Optional object.</param>
        /// <returns>true if objects are equal, false otherwise.</returns>
        public static bool operator ==(Optional<T> first, Optional<T> second) 
            => Equals(first, second);

        /// <summary>
        /// Checks if two Optional objects are unequal.
        /// </summary>
        /// <param name="first">First Optional object.</param>
        /// <param name="second">Second Optional object.</param>
        /// <returns>false if objects are equal, true otherwise.</returns>
        public static bool operator !=(Optional<T> first, Optional<T> second) 
            => !Equals(first, second);

        /// <summary>
        /// Automatically converts a value to the amplied Optional type during assignment or passing to a function call if the receiving variables is of Optional type.
        /// </summary>
        /// <param name="val">The value to amplify.</param>
        public static implicit operator Optional<T>(T val) => new(val);

        /// <summary>
        /// Executes an action on Value if present.
        /// </summary>
        /// <param name="whenSome">The action to execute.</param>
        public void Match(Action<T> whenSome)
        { 
            Match(whenSome, () => { });
        }

        /// <summary>
        /// Executes one of the given actions based on whether the Value is present.
        /// </summary>
        /// <param name="whenSome">Action to execute on Value, if present.</param>
        /// <param name="whenNone">Action to execute when Value is not present.</param>
        public void Match(Action<T> whenSome, Action whenNone)
        {
            if (IsSome)
                whenSome(Value);
            else
                whenNone();
        }        
        
        /// <summary>
        /// Executes one of the given funcs based on whether the Value is present.
        /// </summary>
        /// <typeparam name="TResult">The funcs' return type.</typeparam>
        /// <param name="whenSome">Func to execute on Value, if present.</param>
        /// <param name="whenNone">Func to execute when Value is not present.</param>
        /// <returns>The result of the func that gets executed.</returns>
        public TResult Match<TResult>(Func<T, TResult> whenSome, Func<TResult> whenNone) 
            => IsSome ? whenSome(Value) : whenNone();

        /// <summary>
        /// Executes an action on Value asynchronously if Value is present.
        /// </summary>
        /// <param name="whenSome">The asynchronous task returning func to execute.</param>
        /// <returns>
        /// Task representing asynchronous completion of the given action. 
        /// A Task.CompletedTask is returned when Value is not present.
        /// </returns>
        public async Task Match(Func<T, Task> whenSome) 
            => await Match(whenSome, () => Task.CompletedTask);

        /// <summary>
        /// Executes one of the given actions asynchronously based on whether the Value is present.
        /// </summary>
        /// <param name="whenSome">The asynchronous task returning func to execute on Value.</param>
        /// <param name="whenNone">The asynchronous task returning func to execute without using Value.</param>
        /// <returns>The task object representing completion of the action that gets executed.</returns>
        public async Task Match(Func<T, Task> whenSome, Func<Task> whenNone)
            => await (IsSome ? whenSome(Value) : whenNone());

        /// <summary>
        /// Executes one of the given funcs asynchronously based on whether the Value is present.
        /// </summary>
        /// <typeparam name="TResult">Return type of the result of the asynchronous func call.</typeparam>
        /// <param name="whenSome">The asynchronous func to run on Value.</param>
        /// <param name="whenNone">The asynchronous func to run when Value is not present.</param>
        /// <returns>Task yielding the result on completion of the func that gets executed.</returns>
        public async Task<TResult> Match<TResult> (Func<T, Task<TResult>> whenSome, Func<Task<TResult>> whenNone) 
            => IsSome ?  await whenSome(Value) : await whenNone();

        /// <summary>
        /// Projects the Value, if present, using the supplied func potentially changing the type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of projection operation.</typeparam>
        /// <param name="f">The func which operates on Value.</param>
        /// <returns>
        /// The result of the projection operation as an Optional. 
        /// The Optional is None if Value is not present.
        /// </returns>
        public Optional<TResult> Map<TResult>(Func<T, TResult> f) 
            => IsSome ?  f(Value) : Optional<TResult>.None;

        /// <summary>
        /// Projects the Value, if present, using the supplied asynchronous func potentially changing the type..
        /// </summary>
        /// <typeparam name="TResult">The type of the result yielded by the asynchronous projection operation.</typeparam>
        /// <param name="f">The func which operates on Value asynchronously.</param>
        /// <returns>
        /// Task which yields the result on completion of the asynchronous projection operation as an Optional. 
        /// The Optional is None if Value is not present.
        /// </returns>
        public async Task<Optional<TResult>> Map<TResult>(Func<T, Task<TResult>> f) 
            => IsSome ? await f(Value) : Optional<TResult>.None;

        /// <summary>
        /// Projects the Value, if present, using the supplied Optional returning func potentially changing the type. 
        /// Bind automatically flattens the result. Calling Map using the func will return an Optional of Optional of TResult.
        /// </summary>
        /// <typeparam name="TResult">The type of the raw value of the projection operation.</typeparam>
        /// <param name="f">The Optional returning func which operates on Value.</param>
        /// <returns>The result of the projection operation as an Optional. The Optional is None if Value is not present.</returns>
        public Optional<TResult> Bind<TResult>(Func<T, Optional<TResult>> f)
            => IsSome ? f(Value) : Optional<TResult>.None;

        /// <summary>
        /// Projects the Value, if present, using the supplied Optional returning asynchronous func potentially changing the type. 
        /// Bind automatically flattens the result. Calling Map using the func will return an Optional of Optional of TResult.
        /// </summary>
        /// <typeparam name="TResult">The type of the raw value of the projection operation.</typeparam>
        /// <param name="f">The Optional returning asynchronous func which operates on Value.</param>
        /// <returns>The result of the projection operation as an Optional. The Optional is None if Value is not present.</returns>
        public async Task<Optional<TResult>> Bind<TResult>(Func<T, Task<Optional<TResult>>> f) =>
            IsSome ? await f(Value) : Optional<TResult>.None;

        /// <summary>
        /// Executes a predicate on the Value if present.
        /// </summary>
        /// <param name="predicate">FIlter condition to execute.</param>
        /// <returns>Same instance of this Optional if Value meets the filter condition, None otherwise.</returns>
        public Optional<T> Where(Func<T, bool> predicate) 
            => IsSome && predicate(Value) ? this : None;

        /// <summary>
        /// Executes a predicate asynchronously on the Value if present.
        /// </summary>
        /// <param name="predicate">Asynchronous filter condition to execute.</param>
        /// <returns>Task yielding same instance of this Optional if Value meets the filter condition, None otherwise.</returns>
        public async Task<Optional<T>> Where(Func<T, Task<bool>> predicate)
            => IsSome && await predicate(Value) ? this : None;

        /// <summary>
        /// Defaults an Optional to contain the specified value if it does not contain Value.
        /// </summary>
        /// <param name="val">Default value.</param>
        /// <returns>Same instance of this Optional if it contains Value. Otherwise an Optional containing the specified default value.</returns>
        public Optional<T> DefaultIfNone(T val) => IsSome ? this : val;

        /// <summary>
        /// Defaults an Optional to contain the result of the specified func evaluation lazily.
        /// </summary>
        /// <param name="f">func to execute only if Value is not present.</param>
        /// <returns>Same instance of this Optional if it contains Value. Otherwise an Optional containing the result of func evaluation.</returns>
        public Optional<T> DefaultIfNone(Func<T> f) => IsSome ? this : f();

        /// <summary>
        /// Defaults an Optional to contain the result of the specified func evaluation lazily and asynchronously.
        /// </summary>
        /// <param name="f">Asynchronous func to execute only if Value is not present.</param>
        /// <returns>
        /// Same instance of this Optional if it contains Value. 
        /// Otherwise a Task yielding Optional containing the result of func evaluation.
        /// </returns>
        public async Task<Optional<T>> DefaultIfNone(Func<Task<T>> f)
            => IsSome ? this : await f();
    }

    /// <summary>
    /// Static class providing utility methods to create an Optional from an object or function call.
    /// </summary>
    public static class Optional
    {
        /// <summary>
        /// Creates an Optional from the given value.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="value">Value to be amplified.</param>
        /// <returns>An Optional containing value.</returns>
        public static Optional<T> Some<T>(T value) => value;

        /// <summary>
        /// Creates an empty Optional of the type of the given value.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="_">The param value is ignored.</param>
        /// <returns>An empty Optional of type T.</returns>
        public static Optional<T> None<T>(T _) => Optional<T>.None;

        /// <summary>
        /// Amplifies the result of a function call to an Optional. The function is executed inside a try/catch block.
        /// </summary>
        /// <typeparam name="T">Type of value returned by the function.</typeparam>
        /// <param name="f">Function to execute.</param>
        /// <returns>
        /// Optional containing the result on successful execution or None if function call throws an exception.
        /// The exception information is lost. Use Error.Try<T> if exception information is required.
        /// </returns>
        public static Optional<T> Try<T>(Func<T> f)
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
        /// Awaiting the returned task is guaranteed to not fail even if the original task is faulted or canceled.
        /// The exception information is lost. Use Error.Try<T> if exception information is required.
        /// </returns>
        public static async Task<Optional<T>> Try<T>(Func<Task<T>> f)
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
        /// Optional containing the result on successful execution or None if function call throws an exception.
        /// The exception information is lost. Use Error.Try<T> if exception information is required.
        /// </returns>
        public static Optional<T2> Try<T1, T2>(Func<T1, T2> f, T1 val) => Try(() => f(val));

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
        /// or None if the operation fails or task is canceled.
        /// Awaiting the returned task is guaranteed to not fail even if the original task is faulted or canceled.
        /// The exception information is lost. Use Error.Try<T> if exception information is required.
        /// </returns>
        public static async Task<Optional<T2>> Try<T1, T2>(Func<T1, Task<T2>> f, T1 val) => await Try(() => f(val));
    }
}
