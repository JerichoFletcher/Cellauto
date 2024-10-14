using JFCellauto.Impl;
using JFCellauto.Structs;
using System.Text;

namespace JFCellautoCLI;

public static class Program {
    private static string ColorOf(bool val) {
        return val ? "\x1b[37;47m" : "\x1b[30;40m";
    }

    private static void PrintGrid(Grid<bool> grid) {
        var lastState = grid.Cells[0, 0].Value;
        var str = new StringBuilder("\x1b[0;0H").Append(ColorOf(lastState));

        for(int x = 0; x < grid.Bounds.X; x++) {
            if(x > 0) str.AppendLine();

            for(int y = 0; y < grid.Bounds.Y; y++) {
                var state = grid.Cells[x, y].Value;
                if(state != lastState) {
                    str.Append(ColorOf(state));
                    lastState = state;
                }
                str.Append("  ");
            }
        }

        Console.Write(str.Append("\x1b[0m").ToString());
    }

    public static void Main(string[] args) {
        var rows = 30;
        var cols = 60;
        var randGridData = new bool[rows, cols];
        var r = new Random();
        for(int i = 0; i < rows * cols / 4; i++) {
            randGridData[r.Next(rows), r.Next(cols)] = true;
        }

        var grid = new ConwayLifeBuilderDirector().Make(new GridBuilder<bool>().Data(randGridData));

        while(true) {
            PrintGrid(grid);

            Thread.Sleep(50);
            grid.Update();
        }
    }
}
