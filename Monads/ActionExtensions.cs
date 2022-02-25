#pragma warning disable CS1591
namespace Monads;

public static class ActionExtensions
{
    /// <summary>
    /// Converts an Action to a Unit returning Func. An exception thrown by the Action is NOT caught.
    /// </summary>
    /// <typeparam name="T">Type of argument accepted by the Action.</typeparam>
    /// <param name="action">Action to convert.</param>
    /// <returns>Returns a Func that executes the action.</returns>
    public static Func<T, Unit> ToFunc<T>(this Action<T> action) =>
        arg =>
        {
            action(arg);
            return Unit.Instance;
        };

    /// <summary>
    /// Converts an Action to a Unit returning Func. An exception thrown by the Action is NOT caught.
    /// </summary>
    /// <param name="action">Action to convert.</param>
    /// <returns>Returns a Func that executes the action.</returns>
    public static Func<Unit> ToFunc(this Action action) =>
        () =>
        {
            action();
            return Unit.Instance;
        };

    public static Func<T, Task<Unit>> ToFuncAsync<T>(this Action<T> action) => 
        arg => Task.Run(() => action.ToFunc()(arg));

    public static Func<Task<Unit>> ToFuncAsync(this Action action) =>
        () => Task.Run(() => action.ToFunc()());
}