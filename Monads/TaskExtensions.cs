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
            
        public static Task<T2> Map<T1, T2>(this Task<T1> t, Func<T1, T2> f) 
            => t.ContinueWith(t1 => { return f(t1.Result); }, onlyOnSuccessfulCompletion);

        public static Task<T2> Bind<T1, T2>(this Task<T1> t, Func<T1, Task<T2>> f)
            => t.ContinueWith(t1 => { return f(t1.Result); }, onlyOnSuccessfulCompletion).Unwrap();
    }
}