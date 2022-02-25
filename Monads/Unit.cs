namespace Monads;

/// <summary>
/// Type in FP that represents absense of a return type in a method. Resembles the void return type in OOP.
/// </summary>
public record class Unit
{
    /// <summary>
    /// Returns the only instance of Unit ever needed.
    /// </summary>
    public static readonly Unit Instance = new();

    private Unit()
    { }
}
