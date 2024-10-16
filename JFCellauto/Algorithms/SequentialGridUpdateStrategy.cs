using JFCellauto.Structs;

namespace JFCellauto.Algorithms;

/// <summary>
/// An implementation of <see cref="IGridUpdateStrategy{T}"/> that processes each <see cref="Cell{T}"/> in a <see cref="Grid{T}"/>
/// sequentially in a single thread.
/// </summary>
/// <typeparam name="T">The type of the state value stored in each <see cref="Cell{T}"/>.</typeparam>
public sealed class SequentialGridUpdateStrategy<T> : IGridUpdateStrategy<T> where T : struct {
    public void GetNextGeneration(Grid<T> grid, Cell<T>[,] outBuffer) {
        var stepRules = grid.Rules
            .OfType<StepRule<T>>()
            .ToArray();

        for(var x = 0; x < grid.Bounds.X; x++) {
            for(var y = 0; y < grid.Bounds.Y; y++) {
                var outCell = outBuffer[x, y];
                outCell.Value = grid.Cells[x, y].Value;

                foreach(var stepRule in stepRules) {
                    outBuffer[x, y].Value = stepRule.Evaluate(outCell, grid);
                }
            }
        }
    }
}
