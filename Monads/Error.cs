using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public class Error<T>
    {
        public Exception Ex { get;}
        public T Value { get; }
        public bool IsValue { get; }


        public Error(T value)
        {
            Value = value;
            IsValue = true;
        }

        public Error(Exception ex)
        {
            Ex = ex;
            IsValue = false;
        }        

        public void Match(Action<T> whenValue, Action<Exception> whenEx)
        {
            if (IsValue)
                whenValue(Value);
            else
                whenEx(Ex);
        }

        public async Task Match(Func<T, Task> whenValue, Func<Exception, Task> whenEx)
            => await (IsValue ? whenValue(Value) : whenEx(Ex));
        
        public TResult Match<TResult>(Func<T, TResult> whenValue, Func<Exception, TResult> whenEx)
            => IsValue ? whenValue(Value) : whenEx(Ex);

        public async Task<TResult> Match<TResult> (Func<T, Task<TResult>> whenValue, Func<Exception, Task<TResult>> whenEx) 
            => IsValue ?  await whenValue(Value) : await whenEx(Ex);

        public Error<TResult> Map<TResult>(Func<T, TResult> f)
        {
            if (!IsValue)
                return new (Ex);

            try
            {
                return new (f(Value));
            }
            catch (Exception ex)
            {
                return new (ex);
            }
        }

        public async Task<Error<TResult>> Map<TResult>(Func<T, Task<TResult>> f)
        {
            if (!IsValue)
                return new (Ex);

            try
            {
                return new (await f(Value));
            }
            catch (Exception ex)
            {
                return new (ex);
            }
        }

        public Error<TResult> Bind<TResult>(Func<T, Error<TResult>> f)
            => IsValue ? f(Value) : new (Ex);

        public async Task<Error<TResult>> Bind<TResult>(Func<T, Task<Error<TResult>>> f)
        {
            if (!IsValue)
                return new (Ex);

            try
            {
                return await f(Value);
            }
            catch (Exception ex)
            {
                return new (ex);
            }
        }

        public Error<T> DefaultIfError(T val) => IsValue ? this : new (val);

        public Error<T> DefaultIfError(Func<T> f)
        {
            if (IsValue)
                return this;

            try
            {
                return new (f());
            }
            catch (Exception ex)
            {
                return new (ex);
            }
        }

        public async Task<Error<T>> DefaultIfError(Func<Task<T>> f)
        {
            if (IsValue)
                return this;

            try
            {
                return new (await f());
            }
            catch (Exception ex)
            {
                return new (ex);
            }
        }
    }

    public static class Error
    {
        public static Error<T> Ex<T>(Exception ex) => new (ex);

        public static Error<T> Value<T>(T value) => new (value);

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

        public static Error<T2> Try<T1, T2>(Func<T1, T2> f, T1 val) => Try(() => f(val));

        public static async Task<Error<T2>> Try<T1, T2>(Func<T1, Task<T2>> f, T1 val) => await Try(() => f(val));
    }
}
