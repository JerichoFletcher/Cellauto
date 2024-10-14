namespace JFCellauto.Structs;

/// <summary>
/// Represents a single cell in a <see cref="Grid{T}"/> that stores a state value.
/// </summary>
/// <typeparam name="T">The type of the state value stored in the cell.</typeparam>
/// <param name="coord">The grid-space coordinate associated with this cell.</param>
/// <param name="value">The value supplied as the initial state for this cell.</param>
public class Cell<T>(Vector coord, T value) where T : struct {
    /// <summary>The grid-space coordinate associated with this cell.</summary>
    public Vector Coord { get; set; } = coord;
    /// <summary>The current stored state of this cell.</summary>
    public T Value { get; set; } = value;
}
