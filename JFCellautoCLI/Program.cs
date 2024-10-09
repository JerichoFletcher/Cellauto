using JFCellauto.Structs;
using System.Text;

namespace JFCellautoCLI;

public static class Program {
    public enum CellVal {
        Dead, Alive
    }

    private static void PrintGrid(Grid<CellVal> grid) {
        var str = new StringBuilder("\x1b[0;0H");

        for(int x = 0; x < grid.Bounds.X; x++) {
            for(int y = 0; y < grid.Bounds.Y; y++) {
                str.Append(grid.Cells[x, y].Value == CellVal.Alive ? "O " : "_ ");
            }
            str.AppendLine();
        }

        Console.WriteLine(str.ToString());
    }

    public static void Main(string[] args) {
        var grid = Grid<CellVal>.Filled(new Vector(28, 60), CellVal.Dead);
        grid.WrapEdges = true;

        var r = new Random();

        for(var i = 0; i < grid.Bounds.X * grid.Bounds.Y / 3; i++) {
            grid.Cells[r.Next(grid.Bounds.X), r.Next(grid.Bounds.Y)].Value = CellVal.Alive;
        }

        grid.Rules.Add(new StepRule<CellVal>((cell, grid) => {
            var aliveNeighbors = grid
                .Neighbors(cell.Coord.X, cell.Coord.Y)
                .Where(cell => cell.Value == CellVal.Alive)
                .Count();

            if(aliveNeighbors == 3 || aliveNeighbors == 2 && cell.Value == CellVal.Alive) return CellVal.Alive;
            return CellVal.Dead;
        }));

        while(true) {
            PrintGrid(grid);

            Thread.Sleep(30);
            grid.Step(true);
        }
    }
}
