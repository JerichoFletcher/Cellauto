using JerichoFletcher.Cellauto.Structs;

namespace JerichoFletcher.Cellauto.Algorithms;

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
    /// <param name="outBuffer">A cell buffer array where the new cell state values will be put.</param>
    void GetNextGeneration(Grid<T> grid, CellBuffer<T> outBuffer);
}
