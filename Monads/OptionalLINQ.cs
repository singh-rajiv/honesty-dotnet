namespace Monads;

/// <summary>
/// Methods in this class have been defined to enable LINQ query syntax on Optional type.
/// These methods should not generally be called directly from user code.
/// LINQ queries on Optional type will attach to these methods using duck typing.
/// </summary>
public static class OptionalLINQ
{
    /// <summary>
    /// Not to be called directly. Use Map if using method syntax.
    /// </summary>
    public static Optional<T2> Select<T1, T2>(this Optional<T1> o, Func<T1, T2> f) => o.Map(f);

    /// <summary>
    /// Not to be called directly. Use Bind if using method syntax.
    /// </summary>
    public static Optional<T3> SelectMany<T1, T2, T3>(this Optional<T1> o, Func<T1, Optional<T2>> f, Func<T1, T2, T3> g)
        => o.Bind(v1 => f(v1).Bind(v2 => g(v1, v2).ToOptional()));
}
