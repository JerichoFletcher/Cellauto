namespace JFCellauto.Structs;

public class Grid<T> where T : struct, Enum {
    public Vector Bounds { get; }
    public Cell<T>[,] Cells { get; protected set; }
    public List<Rule<T>> Rules { get; }
    public bool WrapEdges { get; set; }

    internal Grid(Vector bounds) {
        Bounds = bounds;
        Cells = new Cell<T>[bounds.X, bounds.Y];
        Rules = [];
    }

    public static Grid<T> Filled(Vector bounds, T initialValue) {
        var grid = new Grid<T>(bounds);

        for(int x = 0; x < bounds.X; x++) {
            for(int y = 0; y < bounds.Y; y++) {
                grid.Cells[x, y] = new Cell<T>(new Vector(x, y), initialValue);
            }
        }

        return grid;
    }

    public IEnumerable<Cell<T>> Neighbors(int x, int y) {
        if(x < 0 || x >= Bounds.X) {
            throw new ArgumentOutOfRangeException(nameof(x));
        }
        if(y < 0 || y >= Bounds.Y) {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        for(int dx = -1; dx <= 1; dx++) {
            for(int dy = -1; dy <= 1; dy++) {
                if(dx == 0 && dy == 0) continue;

                var nx = x + dx;
                var ny = y + dy;

                if(nx < 0 || ny < 0 || nx >= Bounds.X || ny >= Bounds.Y) {
                    if(!WrapEdges) {
                        continue;
                    }

                    if(nx < 0) nx = Bounds.X + nx % Bounds.X;
                    if(ny < 0) ny = Bounds.Y + ny % Bounds.Y;
                    if(nx >= Bounds.X) nx %= Bounds.X;
                    if(ny >= Bounds.Y) ny %= Bounds.Y;
                }

                yield return Cells[nx, ny];
            }
        }
    }

    public void Step(bool parallel = false) {
        var newBoard = new Cell<T>[Bounds.X, Bounds.Y];
        var stepRules = Rules
            .OfType<StepRule<T>>()
            .ToArray();

        if(parallel) {
            Parallel.For(0, Bounds.X * Bounds.Y, i => {
                var x = i / Bounds.Y;
                var y = i % Bounds.Y;

                var oldCell = Cells[x, y];
                var newCell = new Cell<T>(oldCell.Coord, oldCell.Value);

                foreach(var stepRule in stepRules) {
                    newCell.Value = stepRule.Evaluate(newCell, this);
                }

                newBoard[x, y] = newCell;
            });
        } else {
            for(var x = 0; x < Bounds.X; x++) {
                for(var y = 0; y < Bounds.Y; y++) {
                    var oldCell = Cells[x, y];
                    var newCell = new Cell<T>(oldCell.Coord, oldCell.Value);

                    foreach(var stepRule in stepRules) {
                        newCell.Value = stepRule.Evaluate(newCell, this);
                    }

                    newBoard[x, y] = newCell;
                }
            }
        }

        Cells = newBoard;
    }
}
