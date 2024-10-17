using JerichoFletcher.Cellauto.Structs;

namespace JerichoFletcher.Cellauto.Algorithms;

/// <summary>
/// An implementation of <see cref="IGridUpdateStrategy{T}"/> that processes each <see cref="Cell{T}"/> in a <see cref="Grid{T}"/>
/// sequentially in a single thread.
/// </summary>
/// <param name="rules">An array of grid rules to be used by the update strategy.</param>
/// <typeparam name="T">The type of the state value stored in each <see cref="Cell{T}"/>.</typeparam>
public sealed class SequentialGridUpdateStrategy<T>(params Rule<T>[] rules) : IGridUpdateStrategy<T> where T : struct {
    /// <summary>An array of grid rules to be used by the update strategy.</summary>
    public Rule<T>[] Rules { get; } = rules;

    public void GetNextGeneration(Grid<T> grid, CellBuffer<T> outBuffer) {
        var stepRules = Rules
            .OfType<StepRule<T>>()
            .ToArray();

        for(var x = 0; x < grid.Bounds.X; x++) {
            for(var y = 0; y < grid.Bounds.Y; y++) {
                var current = new Cell<T>(x, y, grid[x, y]);

                foreach(var stepRule in stepRules) {
                    current.Value = stepRule.Evaluate(current, grid);
                }

                outBuffer[x, y] = current.Value;
            }
        }
    }
}
