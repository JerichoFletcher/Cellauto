using JFCellauto.Algorithms;
using JFCellauto.Structs;

namespace JFCellauto.Impl;

/// <summary>
/// A builder director that constructs <see cref="Grid{T}"/> instances following the specification and predefined rules
/// of Conway's Game of Life.
/// </summary>
/// <param name="updateStrategy">The update strategy to supply to the resultant <see cref="Grid{T}"/>.</param>
public sealed class ConwayLifeBuilderDirector(IGridUpdateStrategy<bool> updateStrategy) {
    /// <summary>
    /// Constructs a <see cref="ConwayLifeBuilderDirector"/> that uses the default parallelized update strategy.
    /// </summary>
    public ConwayLifeBuilderDirector() : this(new ParallelGridUpdateStrategy<bool>()) { }

    /// <summary>
    /// Creates an empty Conway's Game of Life grid.
    /// </summary>
    /// <param name="gridBuilder">A <see cref="GridBuilder{T}"/> object that has been supplied with a bounds parameter.</param>
    /// <returns>A grid object filled with empty cells and provided with the Conway's Game of Life update rule.</returns>
    public Grid<bool> Make(IGridBuilderStep2<bool> gridBuilder) {
        return Make(gridBuilder.Fill(false));
    }

    /// <summary>
    /// Creates a Conway's Game of Life grid.
    /// </summary>
    /// <param name="gridBuilder">A <see cref="GridBuilder{T}"/> object that has been supplied with cell data.</param>
    /// <returns>A grid object filled with cells of the given data and provided with the Conway's Game of Life update rule.</returns>
    public Grid<bool> Make(IGridBuilderStep3<bool> gridBuilder) {
        return gridBuilder
            .UpdateStrategy(updateStrategy)
            .Rule(new StepRule<bool>((cell, grid) => {
                var aliveNeighbors = grid.Neighbors(cell.Coord.X, cell.Coord.Y)
                    .Where(cell => cell.Value)
                    .Count();

                if(cell.Value) {
                    return 2 <= aliveNeighbors && aliveNeighbors <= 3;
                } else {
                    return aliveNeighbors == 3;
                }
            }))
            .Build();
    }
}
