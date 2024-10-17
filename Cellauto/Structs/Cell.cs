namespace JerichoFletcher.Cellauto.Structs;

/// <summary>
/// Represents a single cell in a <see cref="Grid{T}"/> that stores a state value.
/// </summary>
/// <typeparam name="T">The type of the state value stored in the cell.</typeparam>
/// <param name="coord">The grid-space coordinate associated with this cell.</param>
/// <param name="value">The state value of this cell.</param>
public struct Cell<T>(int x, int y, T value) where T : struct {
    /// <summary>
    /// Creates a cell instance with the given coordinate.
    /// </summary>
    /// <param name="coord">The coordinate vector of the cell.</param>
    /// <param name="value">The state value of this cell.</param>
    public Cell(Vector coord, T value)
        : this(coord.X, coord.Y, value) { }

    /// <summary>The current stored state of this cell.</summary>
    public T Value { get; set; } = value;
    /// <summary>The row coordinate of this cell.</summary>
    public int X { get; set; } = x;
    /// <summary>The column coordinate of this cell.</summary>
    public int Y { get; set; } = y;
}
