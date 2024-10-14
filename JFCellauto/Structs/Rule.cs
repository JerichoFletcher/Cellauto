namespace JFCellauto.Structs;

/// <summary>
/// An abstraction of various grid rules to be applied at various stages of the grid lifecycle.
/// </summary>
/// <typeparam name="T">The type of the state value stored in cells within grids this rule applies to.</typeparam>
public abstract class Rule<T> where T : struct { }

/// <summary>
/// Grid rules intended to be applied on generation advancements.
/// </summary>
/// <typeparam name="T">The type of the state value stored in cells within grids this rule applies to.</typeparam>
/// <param name="rule">The cell mapping function to be applied on each cell within a grid.</param>
public class StepRule<T>(Func<Cell<T>, Grid<T>, T> rule) : Rule<T> where T : struct {
    private readonly Func<Cell<T>, Grid<T>, T> rule = rule;

    /// <summary>
    /// Maps the state value of <paramref name="cell"/> to a new state value.
    /// </summary>
    /// <param name="cell">The reference cell to apply the rule on.</param>
    /// <param name="grid">The grid that contains the reference cell.</param>
    /// <returns>The new state value computed from <paramref name="cell"/> and <paramref name="grid"/>.</returns>
    public T Evaluate(Cell<T> cell, Grid<T> grid) => rule(cell, grid);
}
