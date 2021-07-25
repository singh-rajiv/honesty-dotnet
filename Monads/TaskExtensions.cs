using System;
using System.Threading.Tasks;

namespace HonestyDotNet.Monads
{
    public static class TaskExtensions
    {
        private static readonly TaskContinuationOptions onlyOnSuccessfulCompletion = 
            TaskContinuationOptions.OnlyOnRanToCompletion | 
            TaskContinuationOptions.NotOnFaulted | 
            TaskContinuationOptions.NotOnCanceled;

        /// <summary>
        /// Projects the value yielded by a task by executing the given continuation, only on successful completion.
        /// </summary>
        /// <typeparam name="T1">Type of value yielded by the source task.</typeparam>
        /// <typeparam name="T2">Type of the result of the projection operation.</typeparam>
        /// <param name="t">Task which yields the input to the projection.</param>
        /// <param name="f">Continuation which projects the input.</param>
        /// <returns>A new task which yields the projected value on successful completion of the source task.</returns>
        public static Task<T2> Map<T1, T2>(this Task<T1> t, Func<T1, T2> f) 
            => t.ContinueWith(t1 => { return f(t1.Result); }, onlyOnSuccessfulCompletion);

        /// <summary>
        /// Projects the value yielded by a task by executing the given asynchronous continuation, only on successful completion.
        /// </summary>
        /// <typeparam name="T1">Type of value yielded by the source task.</typeparam>
        /// <typeparam name="T2">Type of the result of the projection operation.</typeparam>
        /// <param name="t">Task which yields the input to the projection.</param>
        /// <param name="f">Continuation which projects the input asynchronously.</param>
        /// <returns>A new task which yields the projected value on successful completion of the source task.</returns>
        public static Task<T2> Bind<T1, T2>(this Task<T1> t, Func<T1, Task<T2>> f)
            => t.ContinueWith(t1 => { return f(t1.Result); }, onlyOnSuccessfulCompletion).Unwrap();
    }
}