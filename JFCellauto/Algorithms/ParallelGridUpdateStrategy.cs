using JFCellauto.Structs;

namespace JFCellauto.Algorithms;

/// <summary>
/// An implementation of <see cref="IGridUpdateStrategy{T}"/> that processes each <see cref="Cell{T}"/> in a <see cref="Grid{T}"/>
/// in parallel, using the Task Parallel Library.
/// </summary>
/// <param name="rules">An array of grid rules to be used by the update strategy.</param>
/// <typeparam name="T">The type of the state value stored in each <see cref="Cell{T}"/>.</typeparam>
public sealed class ParallelGridUpdateStrategy<T>(params StepRule<T>[] rules) : IGridUpdateStrategy<T> where T : struct {
    /// <summary>An array of grid rules to be used by the update strategy.</summary>
    public StepRule<T>[] Rules { get; } = rules;

    public void GetNextGeneration(Grid<T> grid, CellBuffer<T> outBuffer) {
        Parallel.For(0, grid.Bounds.X * grid.Bounds.Y, i => {
            var x = i / grid.Bounds.Y;
            var y = i % grid.Bounds.Y;

            var current = new Cell<T>(x, y, grid[x, y]);

            foreach(var stepRule in Rules) {
                current.Value = stepRule.Evaluate(current, grid);
            }

            outBuffer[x, y] = current.Value;
        });
    }
}
