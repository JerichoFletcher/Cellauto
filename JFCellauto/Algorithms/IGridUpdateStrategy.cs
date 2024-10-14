using JFCellauto.Structs;

namespace JFCellauto.Algorithms;

/// <summary>
/// An abstraction of an object that provides a method by which to map the collection of <see cref="Cell{T}"/> in a <see cref="Grid{T}"/>
/// from one generation to the next.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGridUpdateStrategy<T> where T : struct {
    /// <summary>
    /// Processes a <see cref="Grid{T}"/> and determines the cells that should be in its next generation.
    /// </summary>
    /// <param name="grid">The grid to process.</param>
    /// <returns>The new cell state values for the next generation.</returns>
    Cell<T>[,] GetNextGeneration(Grid<T> grid);
}
