namespace Monads;

/// <summary>
/// Methods in this class have been defined to enable LINQ query syntax on Result type.
/// These methods should not generally be called directly from user code.
/// LINQ queries on Result type will attach to these methods using duck typing.
/// </summary>
public static class ResultLINQ
{
    /// <summary>
    /// Not to be called directly. Use Map if using method syntax.
    /// </summary>
    public static Result<T2> Select<T1, T2>(this Result<T1> e, Func<T1, T2?> f) => e.Map(f);

    /// <summary>
    /// Not to be called directly. Use Bind if using method syntax.
    /// </summary>
    public static Result<T3> SelectMany<T1, T2, T3>(this Result<T1> e, Func<T1, Result<T2>> f, Func<T1, T2, T3?> g)
        => e.Bind(v1 => f(v1).Bind(v2 => g(v1, v2).ToResult()));
}
