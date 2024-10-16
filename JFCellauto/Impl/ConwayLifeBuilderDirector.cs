using JFCellauto.Algorithms;
using JFCellauto.Structs;

namespace JFCellauto.Impl;

/// <summary>
/// A builder director that constructs <see cref="Grid{T}"/> instances following the specification and predefined rules
/// of Conway's Game of Life.
/// </summary>
public sealed class ConwayLifeBuilderDirector {
    public enum UpdateStrategyMode {
        Sequential,
        Parallel,
        //Vectorized,
    }

    /// <summary>
    /// Creates an empty Conway's Game of Life grid.
    /// </summary>
    /// <param name="gridBuilder">A <see cref="GridBuilder{T}"/> object that has been supplied with a bounds parameter.</param>
    /// <returns>A grid object filled with empty cells and provided with the Conway's Game of Life update rule.</returns>
    public Grid<bool> Make(IGridBuilderStep2<bool> gridBuilder, UpdateStrategyMode mode) {
        return Make(gridBuilder.Fill(false), mode);
    }

    /// <summary>
    /// Creates a Conway's Game of Life grid.
    /// </summary>
    /// <param name="gridBuilder">A <see cref="GridBuilder{T}"/> object that has been supplied with cell data.</param>
    /// <returns>A grid object filled with cells of the given data and provided with the Conway's Game of Life update rule.</returns>
    public Grid<bool> Make(IGridBuilderStep3<bool> gridBuilder, UpdateStrategyMode mode) {
        IGridUpdateStrategy<bool> updateStrategy = mode switch {
            UpdateStrategyMode.Sequential => new SequentialGridUpdateStrategy<bool>(new StepRule<bool>((cell, grid) => {
                var aliveNeighbors = grid.Neighbors(cell.Coord.X, cell.Coord.Y)
                    .Where(cell => cell.Value)
                    .Count();

                return aliveNeighbors == 3 || (aliveNeighbors == 2 && cell.Value);
            })),
            UpdateStrategyMode.Parallel => new ParallelGridUpdateStrategy<bool>(new StepRule<bool>((cell, grid) => {
                var aliveNeighbors = grid.Neighbors(cell.Coord.X, cell.Coord.Y)
                    .Where(cell => cell.Value)
                    .Count();

                return aliveNeighbors == 3 || (aliveNeighbors == 2 && cell.Value);
            })),
            _ => throw new ArgumentException("Unknown update strategy mode", nameof(mode))
        };

        return gridBuilder
            .UpdateStrategy(updateStrategy)
            .Build();
    }
}
