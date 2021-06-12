using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public class Optional<T> : IEquatable<Optional<T>>
    {
        public T Value { get; }
        public bool IsSome { get; }
        public static Optional<T> None => new();

        public Optional(T value)
        {
            Value = value;
            IsSome = value != null;
        }

        private Optional()
        {
            IsSome = false;
        }

        public bool Equals(Optional<T> other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(other, this) || (!other.IsSome && !IsSome))
                return true;

            return other.IsSome == IsSome && EqualityComparer<T>.Default.Equals(other.Value, Value);
        }

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

        public override int GetHashCode() => IsSome ? Value.GetHashCode() : 0;

        public static bool operator ==(Optional<T> first, Optional<T> second) 
            => Equals(first, second);

        public static bool operator !=(Optional<T> first, Optional<T> second) 
            => !Equals(first, second);

        public static implicit operator Optional<T>(T val) => new(val);

        public void Match(Action<T> whenSome)
        { 
            Match(whenSome, () => { });
        }

        public void Match(Action<T> whenSome, Action whenNone)
        {
            if (IsSome)
                whenSome(Value);
            else
                whenNone();
        }        
        
        public TResult Match<TResult>(Func<T, TResult> whenSome, Func<TResult> whenNone) 
            => IsSome ? whenSome(Value) : whenNone();

        public async Task Match(Func<T, Task> whenSome) 
            => await Match(whenSome, () => Task.CompletedTask);

        public async Task Match(Func<T, Task> whenSome, Func<Task> whenNone)
            => await (IsSome ? whenSome(Value) : whenNone());


        public async Task<TResult> Match<TResult> (Func<T, Task<TResult>> whenSome, Func<Task<TResult>> whenNone) 
            => IsSome ?  await whenSome(Value) : await whenNone();

        public Optional<TResult> Map<TResult>(Func<T, TResult> f) 
            => IsSome ?  f(Value) : Optional<TResult>.None;

        public async Task<Optional<TResult>> Map<TResult>(Func<T, Task<TResult>> f) 
            => IsSome ? await f(Value) : Optional<TResult>.None;

        public Optional<TResult> Bind<TResult>(Func<T, Optional<TResult>> f)
            => IsSome ? f(Value) : Optional<TResult>.None;

        public async Task<Optional<TResult>> Bind<TResult>(Func<T, Task<Optional<TResult>>> f) =>
            IsSome ? await f(Value) : Optional<TResult>.None;

        public Optional<T> Where(Func<T, bool> predicate) 
            => IsSome && predicate(Value) ? this : None;

        public async Task<Optional<T>> Where(Func<T, Task<bool>> predicate)
            => IsSome && await predicate(Value) ? this : None;

        public Optional<T> DefaultIfNone(T val) => IsSome ? this : val;

        public Optional<T> DefaultIfNone(Func<T> f) => IsSome ? this : f();

        public async Task<Optional<T>> DefaultIfNone(Func<Task<T>> f)
            => IsSome ? this : await f();
    }

    public static class Optional
    {
        public static Optional<T> Some<T>(T value) => value;

        public static Optional<T> None<T>(T _) => Optional<T>.None;

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

        public static Optional<T2> Try<T1, T2>(Func<T1, T2> f, T1 val) => Try(() => f(val));
    }
}
