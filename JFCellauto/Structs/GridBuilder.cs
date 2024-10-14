using JFCellauto.Algorithms;

namespace JFCellauto.Structs;

/// <summary>
/// Represents a grid building stage where a bounds argument has been provided.
/// </summary>
/// <typeparam name="T">The type of the state value stored in each cell within the resultant grid.</typeparam>
public interface IGridBuilderStep2<T> where T : struct {
    /// <summary>
    /// Appends a collection of rules to the grid builder.
    /// </summary>
    /// <param name="rule">An array containing grid update rule instances to be added.</param>
    /// <returns>The builder object.</returns>
    IGridBuilderStep2<T> Rule(params Rule<T>[] rule);
    /// <summary>
    /// Asserts that the resultant grid should wrap around its edges.
    /// </summary>
    /// <returns>The builder object.</returns>
    IGridBuilderStep2<T> WrapEdges();
    /// <summary>
    /// Sets a value for the grid cells to be filled with.
    /// </summary>
    /// <param name="value">The default value for the grid cells.</param>
    /// <returns>The builder object.</returns>
    IGridBuilderStep3<T> Fill(T value);
}

/// <summary>
/// Represents a grid building stage where the cell state data has been provided.
/// </summary>
/// <typeparam name="T">The type of the state value stored in each cell within the resultant grid.</typeparam>
public interface IGridBuilderStep3<T> where T : struct {
    /// <summary>
    /// Appends a collection of rules to the grid builder.
    /// </summary>
    /// <param name="rule">An array containing grid update rule instances to be added.</param>
    /// <returns>The builder object.</returns>
    IGridBuilderStep3<T> Rule(params Rule<T>[] rule);
    /// <summary>
    /// Asserts that the resultant grid should wrap around its edges.
    /// </summary>
    /// <returns>The builder object.</returns>
    IGridBuilderStep3<T> WrapEdges();
    /// <summary>
    /// Sets an update strategy to be used by the resultant grid.
    /// </summary>
    /// <param name="updateStrategy">The update strategy to use.</param>
    /// <returns>The builder object.</returns>
    IGridBuilderStepFinal<T> UpdateStrategy(IGridUpdateStrategy<T> updateStrategy);
}

/// <summary>
/// Represents a grid building stage where all the required parameters for <see cref="Grid{T}"/> creation is provided.
/// </summary>
/// <typeparam name="T">The type of the state value stored in each cell within the resultant grid.</typeparam>
public interface IGridBuilderStepFinal<T> where T : struct {
    /// <summary>
    /// Appends a collection of rules to the grid builder.
    /// </summary>
    /// <param name="rule">An array containing grid update rule instances to be added.</param>
    /// <returns>The builder object.</returns>
    IGridBuilderStepFinal<T> Rule(params Rule<T>[] rule);
    /// <summary>
    /// Asserts that the resultant grid should wrap around its edges.
    /// </summary>
    /// <returns>The builder object.</returns>
    IGridBuilderStepFinal<T> WrapEdges();
    /// <summary>
    /// Creates the resultant grid object.
    /// </summary>
    /// <returns>A grid object.</returns>
    Grid<T> Build();
}

/// <summary>
/// A builder instance used to procedurally generate a <see cref="Grid{T}"/> object.
/// </summary>
/// <typeparam name="T">The type of the state value stored in each cell within the resultant grid.</typeparam>
public sealed class GridBuilder<T> where T : struct {
    internal List<Rule<T>> rules = [];
    internal bool wrapEdges = false;

    /// <summary>
    /// Appends a collection of rules to the grid builder.
    /// </summary>
    /// <param name="rule">An array containing grid update rule instances to be added.</param>
    /// <returns>The builder object.</returns>
    public GridBuilder<T> Rule(params Rule<T>[] rule) {
        rules.AddRange(rule);
        return this;
    }

    /// <summary>
    /// Asserts that the resultant grid should wrap around its edges.
    /// </summary>
    /// <returns>The builder object.</returns>
    public GridBuilder<T> WrapEdges() {
        wrapEdges = true;
        return this;
    }

    /// <summary>
    /// Supplies a boundary argument to the grid builder.
    /// </summary>
    /// <param name="bounds">The boundary, i.e. size of the resultant grid.</param>
    /// <returns>The builder object.</returns>
    public IGridBuilderStep2<T> Bounds(Vector bounds) {
        return new GridBuilderWithSize<T>(bounds) {
            rules = rules,
        };
    }

    /// <summary>
    /// Supplies cell state data to the grid builder.
    /// </summary>
    /// <param name="data">A two-dimensional array containing cell state data to be used.</param>
    /// <returns>The builder object.</returns>
    public IGridBuilderStep3<T> Data(T[,] data) {
        var bounds = new Vector(data.GetLength(0), data.GetLength(1));
        var flattenedData = new T[bounds.X * bounds.Y];

        for(var x = 0; x < bounds.X; x++) {
            for(var y = 0; y < bounds.Y; y++) {
                flattenedData[x * bounds.Y + y] = data[x, y];
            }
        }

        return new GridBuilderWithCellData<T>(flattenedData, bounds) {
            rules = rules,
        };
    }
}

internal sealed class GridBuilderWithSize<T>(Vector bounds) : IGridBuilderStep2<T> where T : struct {
    internal List<Rule<T>> rules = [];
    internal bool wrapEdges = false;

    private readonly T[] data = new T[bounds.X * bounds.Y];

    public IGridBuilderStep2<T> Rule(params Rule<T>[] rule) {
        rules.AddRange(rule);
        return this;
    }

    public IGridBuilderStep2<T> WrapEdges() {
        wrapEdges = true;
        return this;
    }

    public IGridBuilderStep3<T> Fill(T value) {
        Array.Fill(data, value);
        return new GridBuilderWithCellData<T>(data, bounds) {
            rules = rules,
        };
    }
}

internal sealed class GridBuilderWithCellData<T>(T[] data, Vector bounds) : IGridBuilderStep3<T> where T : struct {
    internal List<Rule<T>> rules = [];
    internal bool wrapEdges = false;

    public IGridBuilderStep3<T> Rule(params Rule<T>[] rule) {
        rules.AddRange(rule);
        return this;
    }

    public IGridBuilderStep3<T> WrapEdges() {
        wrapEdges = true;
        return this;
    }

    public IGridBuilderStepFinal<T> UpdateStrategy(IGridUpdateStrategy<T> updateStrategy) {
        return new GridBuilderWithUpdateStrategy<T>(data, bounds, updateStrategy) {
            rules = rules,
        };
    }
}

internal sealed class GridBuilderWithUpdateStrategy<T>(T[] data, Vector bounds, IGridUpdateStrategy<T> updateStrategy) : IGridBuilderStepFinal<T> where T : struct {
    internal List<Rule<T>> rules = [];
    internal IGridUpdateStrategy<T> updateStrategy = updateStrategy;
    internal bool wrapEdges = false;

    public IGridBuilderStepFinal<T> Rule(params Rule<T>[] rule) {
        rules.AddRange(rule);
        return this;
    }

    public IGridBuilderStepFinal<T> WrapEdges() {
        wrapEdges = true;
        return this;
    }

    public Grid<T> Build() {
        var grid = new Grid<T>(bounds, updateStrategy) {
            WrapEdges = wrapEdges,
        };

        for(var i = 0; i < data.Length; i++) {
            var x = i / grid.Bounds.Y;
            var y = i % grid.Bounds.Y;

            grid.Cells[x, y] = new Cell<T>(new Vector(x, y), data[i]);
        }
        grid.Rules.AddRange(rules);

        return grid;
    }
}
