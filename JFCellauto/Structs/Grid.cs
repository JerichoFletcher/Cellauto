namespace JFCellauto.Structs;

public class Grid<T> where T : struct, Enum {
    public Vector Bounds { get; }
    public Cell<T>[,] Cells { get; protected set; }
    public List<Rule<T>> Rules { get; }

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

        for(int nx = x - 1; nx <= x + 1; nx++) {
            for(int ny = y - 1; ny <= y + 1; ny++) {
                if(nx == x && ny == y) continue;
                if(nx < 0 || ny < 0 || nx >= Bounds.X || ny >= Bounds.Y) continue;

                yield return Cells[nx, ny];
            }
        }
    }

    public void Step() {
        var newBoard = new Cell<T>[Bounds.X, Bounds.Y];
        var stepRules = Rules
            .OfType<StepRule<T>>()
            .ToList();

        for(int x = 0; x < Bounds.X; x++) {
            for(int y = 0; y < Bounds.Y; y++) {
                var oldCell = Cells[x, y];
                var newCell = new Cell<T>(oldCell.Coord, oldCell.Value);

                foreach(var stepRule in stepRules) {
                    newCell.Value = stepRule.Evaluate(newCell, this);
                }

                newBoard[x, y] = newCell;
            }
        }

        Cells = newBoard;
    }
}
