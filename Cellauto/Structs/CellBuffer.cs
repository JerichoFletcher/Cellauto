namespace JerichoFletcher.Cellauto.Structs;

/// <summary>
/// Acts as a facade for accessing a grid cell array.
/// </summary>
/// <typeparam name="T">The type of the cell state value data stored in the buffer.</typeparam>
/// <param name="data">The cell state data.</param>
/// <param name="rowCount">The number of rows in the buffer.</param>
/// <param name="colCount">The number of columns in the buffer.</param>
public sealed class CellBuffer<T>(T[] data, int rowCount, int colCount) where T : struct {
    private readonly T[] data = data;

    /// <summary>
    /// Creates an empty grid cell buffer.
    /// </summary>
    /// <param name="rowCount">The number of rows in the buffer.</param>
    /// <param name="colCount">The number of columns in the buffer.</param>
    public CellBuffer(int rowCount, int colCount)
        : this(new T[rowCount * colCount], rowCount, colCount) { }

    /// <summary>Accesses the cell at the specified coordinate.</summary>
    /// <param name="row">The row coordinate of the cell.</param>
    /// <param name="col">The column coordinate of the cell.</param>
    /// <returns>The cell at the specified coordinate in the buffer.</returns>
    public T this[int row, int col] {
        get => data[row * ColumnCount + col];
        set => data[row * ColumnCount + col] = value;
    }
    /// <summary>The number of rows in the buffer.</summary>
    public int RowCount { get; } = rowCount;
    /// <summary>The number of columns in the buffer.</summary>
    public int ColumnCount { get; } = colCount;

    /// <summary>
    /// Gets a read-only access to the inner data buffer.
    /// </summary>
    /// <returns>The inner data buffer as a <see cref="ReadOnlySpan{T}"/>.</returns>
    public ReadOnlySpan<T> AsReadOnlySpan() => (ReadOnlySpan<T>)data;
    /// <summary>
    /// Gets a reference to the inner data buffer.
    /// </summary>
    /// <returns>The raw inner data buffer.</returns>
    public T[] InnerBuffer() => data;
}
