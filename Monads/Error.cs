using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    /// <summary>
    /// Represents an amplified (Error) T in which an Exception instead of a Value may be present.
    /// </summary>
    /// <typeparam name="T">The type of Value.</typeparam>
    public class Error<T>
    {
        /// <summary>
        /// Gets the Exception. Returns null if there is a Value present.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Get the value contained inside. The value is default(T) if there is an Exception present. 
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Returns true if Value is present, false if Exception is present.
        /// </summary>
        public bool IsValue { get; }

        /// <summary>
        /// Creates an instance of Error<T> containing value.
        /// </summary>
        /// <param name="value">Value to store inside the instance.</param>
        public Error(T value)
        {
            Value = value;
            IsValue = true;
        }

        /// <summary>
        /// Creates an instance of Error<T> containing Exception.
        /// </summary>
        /// <param name="ex">Exception to store inside the instance.</param>
        public Error(Exception ex)
        {
            Exception = ex;
            IsValue = false;
        }        

        /// <summary>
        /// Executes one of the given actions.
        /// </summary>
        /// <param name="whenValue">Action to execute on Value, if present.</param>
        /// <param name="whenEx">Action to execute on Exception, if present.</param>
        public void Match(Action<T> whenValue, Action<Exception> whenEx)
        {
            if (IsValue)
                whenValue(Value);
            else
                whenEx(Exception);
        }

        /// <summary>
        /// Executes one of the given actions asynchronously.
        /// </summary>
        /// <param name="whenValue">Asynchronous action to execute on Value, if present.</param>
        /// <param name="whenEx">Asynchronous action to execute on Exception, if present.</param>
        /// <returns>Task representing the asynchronous action that gets executed.</returns>
        public async Task Match(Func<T, Task> whenValue, Func<Exception, Task> whenEx)
            => await (IsValue ? whenValue(Value) : whenEx(Exception));
        
        /// <summary>
        /// Executes one of the given funcs and returns its result.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="whenValue">Func to execute on Value, if present.</param>
        /// <param name="whenEx">Func to execute on Exception, if present.</param>
        /// <returns>Result of the func that gets executed.</returns>
        public TResult Match<TResult>(Func<T, TResult> whenValue, Func<Exception, TResult> whenEx)
            => IsValue ? whenValue(Value) : whenEx(Exception);

        /// <summary>
        /// Executed one of the given asynchronous funcs and returns the task which will eventually yield a result.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="whenValue">Asynchronous func to execute on Value, if present.</param>
        /// <param name="whenEx">Asynchronous func to execute on Exception, if present.</param>
        /// <returns>Task which will yield the result on completion.</returns>
        public async Task<TResult> Match<TResult> (Func<T, Task<TResult>> whenValue, Func<Exception, Task<TResult>> whenEx) 
            => IsValue ?  await whenValue(Value) : await whenEx(Exception);

        /// <summary>
        /// Projects the Value, if present, using the supplied func which returns a TResult.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of projection operation.</typeparam>
        /// <param name="f">The func which operates on Value. It is executed inside a try/catch block.</param>
        /// <returns>
        /// The result of the projection operation as Error<TResult>. 
        /// If this object contains an exception then the result also contains the same exception.
        /// If the func throws an exception then the result contains that exception.
        /// </returns>
        public Error<TResult> Map<TResult>(Func<T, TResult> f)
            => IsValue ? Error.Try(f, Value) : new (Exception);

        /// <summary>
        /// Projects the Value, if present, using the supplied asynchronous func which returns a Task<TResult>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of projection operation.</typeparam>
        /// <param name="f">The func which operates on Value asynchronously.</param>
        /// <returns>
        /// Task which yields the result of the projection operation as Error<TResult>. 
        /// If this object contains an exception then the result also contains the same exception.
        /// If the func throws an exception then the result contains that exception.
        /// If the task gets canceled then the result contains OperationCancelledException.
        /// </returns>
        public async Task<Error<TResult>> Map<TResult>(Func<T, Task<TResult>> f)
            => IsValue ? await Error.Try(f, Value) : new (Exception);

        /// <summary>
        /// Projects the Value, if present, using the supplied func which returns an Error<TResult>.
        /// Bind flattens the result. Calling Map using the func will return an Error<Error<TResult>>.
        /// </summary>
        /// <typeparam name="TResult">The type of the raw value of the projection operation.</typeparam>
        /// <param name="f">The func which operates on Value.</param>
        /// <returns>
        /// The result of the projection operation as an Error<TResult>.
        /// If this object contains an exception then the result also contains the same exception.
        /// If the func throws an exception then the result contains that exception.
        /// </returns>
        public Error<TResult> Bind<TResult>(Func<T, Error<TResult>> f)
            => IsValue ? Error.Try(f, Value).Flatten() : new (Exception);

        /// <summary>
        /// Projects the Value, if present, using the supplied asynchronous func which returns a Task<Error<TResult>>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result of projection operation.</typeparam>
        /// <param name="f">The func which operates on Value asynchronously.</param>
        /// <returns>
        /// Task which yields the result of the projection operation as Error<TResult>. 
        /// If this object contains an exception then the result also contains the same exception.
        /// If the func throws an exception then the result contains that exception.
        /// If the task gets canceled then the result contains OperationCancelledException.
        /// </returns>
        public async Task<Error<TResult>> Bind<TResult>(Func<T, Task<Error<TResult>>> f)
            => IsValue ? (await Error.Try(f, Value)).Flatten() : new (Exception);

        /// <summary>
        /// Defaults an Error<T> to contain the specified value, if it contains an exception.
        /// </summary>
        /// <param name="val">Default value.</param>
        /// <returns>Same instance of the object if it contains a value. Otherwise a new object containing the specified default value.</returns>
        public Error<T> DefaultIfException(T val) => IsValue ? this : new (val);

        /// <summary>
        /// Defaults an Error<T> to contain a value computed lazily at runtime, if it contains an exception.
        /// </summary>
        /// <param name="f">func which provides the default value. It is executed only if Value is not present.</param>
        /// <returns>
        /// Same instance of this object if it contains Value. Otherwise a new object containing the result of func evaluation.
        /// If the func throws an exception then the object contains that exception.
        /// </returns>
        public Error<T> DefaultIfException(Func<T> f) => IsValue ? this : Error.Try(f);

        /// <summary>
        /// Defaults an Error<T> to contain a value computed lazily and asynchronously at runtime, if it contains an exception.
        /// </summary>
        /// <param name="f">func which provides the default value asynchronously. It is executed only if Value is not present.</param>
        /// <returns>
        /// Task which yields the same instance of this object if it contains Value. Otherwise value provided by the func as Error<TResult>. 
        /// If the func throws an exception then the result contains that exception.
        /// If the task gets canceled then the result contains OperationCancelledException.
        /// </returns>
        public async Task<Error<T>> DefaultIfException(Func<Task<T>> f) => IsValue ? this : await Error.Try(f);
    }

    /// <summary>
    /// Static class providing utility methods to create an instance of Error<T>.
    /// </summary>
    public static class Error
    {
        /// <summary>
        /// Creates an instance of Error<T> from an exception.
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="ex">Exception object.</param>
        /// <returns>An Error<T> object which contains the given exception.</returns>
        public static Error<T> Exception<T>(Exception ex) => new (ex);

        /// <summary>
        /// Creates an instance of Error<T> from a value.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <param name="value">Value object.</param>
        /// <returns>An Error<T> object which contains the given value.</returns>
        public static Error<T> Value<T>(T value) => new (value);

        /// <summary>
        /// Creates an instance of Error<T> by executing a parameterless func which may throw an exception.
        /// </summary>
        /// <typeparam name="T">Return type of the func.</typeparam>
        /// <param name="f">Func which gets executed in a try/catch block. </param>
        /// <returns>An Error<T> object which contains the func's result or exception thrown by it.</returns>
        public static Error<T> Try<T>(Func<T> f)
        {
            try
            {
                 return new (f());
            }
            catch (Exception ex)
            {
                return new (ex);
            }
        }

        /// <summary>
        /// Creates an instance of Error<T> by executing a parameterless func asynchronously which may throw an exception.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="f">Func which gets executed in a try/catch block. </param>
        /// <returns>A task object that yields an Error<T> containing the func's result or exception thrown by it.</returns>
        public static async Task<Error<T>> Try<T>(Func<Task<T>> f)
        {
            try
            {
                 return new (await f());
            }
            catch (Exception ex)
            {
                return new (ex);
            }
        }

        /// <summary>
        /// Creates an instance of Error<T> by executing a func which takes an input parameter and may throw.
        /// </summary>
        /// <typeparam name="T1">Type of input value to the func.</typeparam>
        /// <typeparam name="T2">Return type of the func.</typeparam>
        /// <param name="f">Func which gets executed in a try/catch block.</param>
        /// <param name="val">Input parameter to the func.</param>
        /// <returns>An Error<T> object which contains the func's result or exception thrown by it.</returns>
        public static Error<T2> Try<T1, T2>(Func<T1, T2> f, T1 val) => Try(() => f(val));

        /// <summary>
        /// Creates an instance of Error<T> by executing an asynchronous func which takes an input parameter and may throw.
        /// </summary>
        /// <typeparam name="T1">Type of input value to the func.</typeparam>
        /// <typeparam name="T2">Return type of the func.</typeparam>
        /// <param name="f">Func which gets executed in a try/catch block.</param>
        /// <param name="val">Input parameter to the func.</param>
        /// <returns>A task object that yields an Error<T> containing the func's result or exception thrown by it.</returns>
        public static async Task<Error<T2>> Try<T1, T2>(Func<T1, Task<T2>> f, T1 val) => await Try(() => f(val));
    }
}
