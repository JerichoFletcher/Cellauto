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
        var grid = Grid<CellVal>.Filled(new Vector(20, 20), CellVal.Dead);
        grid.Cells[0, 1].Value = CellVal.Alive;
        grid.Cells[1, 2].Value = CellVal.Alive;
        grid.Cells[2, 0].Value = CellVal.Alive;
        grid.Cells[2, 1].Value = CellVal.Alive;
        grid.Cells[2, 2].Value = CellVal.Alive;

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

            Thread.Sleep(100);
            grid.Step();
        }
    }
}
