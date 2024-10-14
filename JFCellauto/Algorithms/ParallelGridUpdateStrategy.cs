using JFCellauto.Structs;

namespace JFCellauto.Algorithms;

/// <summary>
/// An implementation of <see cref="IGridUpdateStrategy{T}"/> that processes each <see cref="Cell{T}"/> in a <see cref="Grid{T}"/>
/// in parallel, using the Task Parallel Library.
/// </summary>
/// <typeparam name="T">The type of the state value stored in each <see cref="Cell{T}"/>.</typeparam>
public sealed class ParallelGridUpdateStrategy<T> : IGridUpdateStrategy<T> where T : struct {
    public Cell<T>[,] GetNextGeneration(Grid<T> grid) {
        var newBoard = new Cell<T>[grid.Bounds.X, grid.Bounds.Y];
        var stepRules = grid.Rules
            .OfType<StepRule<T>>()
            .ToArray();

        Parallel.For(0, grid.Bounds.X * grid.Bounds.Y, i => {
            var x = i / grid.Bounds.Y;
            var y = i % grid.Bounds.Y;

            var oldCell = grid.Cells[x, y];
            var newCell = new Cell<T>(oldCell.Coord, oldCell.Value);

            foreach(var stepRule in stepRules) {
                newCell.Value = stepRule.Evaluate(newCell, grid);
            }

            newBoard[x, y] = newCell;
        });

        return newBoard;
    }
}
