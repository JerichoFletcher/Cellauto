using JerichoFletcher.Cellauto.Algorithms;

namespace JerichoFletcher.Cellauto.Structs;

/// <summary>
/// Represents a grid of cells each storing a state value that is manipulated in iterations (or generations) using a set of rules.
/// </summary>
/// <typeparam name="T">The type of the state value stored in each cell within the grid.</typeparam>
public class Grid<T> where T : struct {

    /// <summary>How to update cells in the grid on each generation.</summary>
    public IGridUpdateStrategy<T> UpdateStrategy { get; set; }
    /// <summary>The bounds, i.e. size of the grid. All coordinates defined within the grid are between (0, 0) and this point.</summary>
    public Vector Bounds { get; }
    /// <summary>The current active cell buffer of the grid.</summary>
    public CellBuffer<T> FrontBuffer;
    /// <summary>The working cell buffer of the grid.</summary>
    public CellBuffer<T> BackBuffer;

    /// <summary>Accesses a cell in the grid at the specified coordinate.</summary>
    /// <param name="x">The row coordinate of the cell.</param>
    /// <param name="y">The column coordinate of the cell.</param>
    /// <returns>The cell object at the specified coordinate.</returns>
    public T this[int x, int y] {
        get => FrontBuffer[x, y];
        set => FrontBuffer[x, y] = value;
    }
    /// <summary>Whether the grid space wraps around on its edges.</summary>
    public bool WrapEdges { get; set; }

    internal Grid(Vector bounds, IGridUpdateStrategy<T> updateStrategy) {
        UpdateStrategy = updateStrategy;
        Bounds = bounds;

        // Create front and back cell buffers
        FrontBuffer = new CellBuffer<T>(bounds.X, bounds.Y);
        BackBuffer = new CellBuffer<T>(bounds.X, bounds.Y);

        // Fill back cell buffer with empty cells
        for(var x = 0; x < bounds.X; x++) {
            for(var y = 0; y < bounds.Y; y++) {
                BackBuffer[x, y] = default;
            }
        }
    }

    /// <summary>
    /// Iterates each neighbor of the given cell.
    /// </summary>
    /// <param name="cell">The origin cell.</param>
    /// <returns>An enumerable that iterates through each direct neighbor of <paramref name="cell"/>.</returns>
    public IEnumerable<T> Neighbors(Cell<T> cell) => Neighbors(cell.X, cell.Y);

    /// <summary>
    /// Iterates each neighbor of the cell at the given coordinate.
    /// </summary>
    /// <param name="coord">The coordinate of the origin cell.</param>
    /// <returns>An enumerable that iterates through each direct neighbor of the cell at <paramref name="coord"/>.</returns>
    public IEnumerable<T> Neighbors(Vector coord) => Neighbors(coord.X, coord.Y);

    /// <summary>
    /// Iterates each neighbor of the cell at the given X- and Y-coordinate.
    /// </summary>
    /// <param name="x">The X-coordinate of the origin cell.</param>
    /// <param name="y">The Y-coordinate of the origin cell.</param>
    /// <returns>An enumerable that iterates through each direct neighbor of the cell at (<paramref name="x"/>, <paramref name="y"/>).</returns>
    /// <exception cref="ArgumentOutOfRangeException">The given coordinate is outside of the bounds of the grid.</exception>
    public IEnumerable<T> Neighbors(int x, int y) {
        // Validate that the given coordinate is within the grid bounds
        if(x < 0 || x >= Bounds.X) {
            throw new ArgumentOutOfRangeException(nameof(x));
        }
        if(y < 0 || y >= Bounds.Y) {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        // Iterate through each 8-way neighboring coordinates
        for(int dx = -1; dx <= 1; dx++) {
            for(int dy = -1; dy <= 1; dy++) {
                if(dx == 0 && dy == 0) continue;

                var nx = x + dx;
                var ny = y + dy;

                if(nx < 0 || ny < 0 || nx >= Bounds.X || ny >= Bounds.Y) {
                    // Ignore off boundary coordinates if the grid does not wrap around...
                    if(!WrapEdges) {
                        continue;
                    }

                    // ...otherwise, use the coordinate from the other side of the grid
                    if(nx < 0) nx = Bounds.X + nx % Bounds.X;
                    if(ny < 0) ny = Bounds.Y + ny % Bounds.Y;
                    if(nx >= Bounds.X) nx %= Bounds.X;
                    if(ny >= Bounds.Y) ny %= Bounds.Y;
                }

                yield return this[nx, ny];
            }
        }
    }

    /// <summary>
    /// Advances the grid to the next generation by updating the cell values based on the defined grid update strategy.
    /// </summary>
    public void Update() {
        UpdateStrategy.GetNextGeneration(this, BackBuffer);
        (BackBuffer, FrontBuffer) = (FrontBuffer, BackBuffer);
    }
}
