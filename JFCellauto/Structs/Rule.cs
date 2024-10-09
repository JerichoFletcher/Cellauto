namespace JFCellauto.Structs;

public abstract class Rule<T> where T : struct, Enum { }

public class StepRule<T>(Func<Cell<T>, Grid<T>, T> rule) : Rule<T> where T : struct, Enum {
    private readonly Func<Cell<T>, Grid<T>, T> rule = rule;

    public T Evaluate(Cell<T> cell, Grid<T> grid) => rule(cell, grid);
}
