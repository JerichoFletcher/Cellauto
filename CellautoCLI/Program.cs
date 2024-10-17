using JerichoFletcher.Cellauto.Impl;
using JerichoFletcher.Cellauto.Structs;
using System.Diagnostics;
using System.Text;

namespace JerichoFletcher.CellautoCLI;

public static class Program {
    private static string ColorOf(bool val) {
        return val ? "\x1b[37;47m" : "\x1b[30;40m";
    }

    private static void PrintGrid(Grid<bool> grid) {
        var lastState = grid[0, 0];
        var str = new StringBuilder("\x1b[0;0H").Append(ColorOf(lastState));

        for(int x = 0; x < grid.Bounds.X; x++) {
            if(x > 0) str.AppendLine();

            for(int y = 0; y < grid.Bounds.Y; y++) {
                var state = grid[x, y];
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
        var lifeBDir = new ConwayLifeBuilderDirector();
        var debug = false;

        if(debug) {
            var rows = 40;
            var cols = 40;
            var randGridData = new bool[rows, cols];
            var r = new Random();
            for(int i = 0; i < rows * cols / 4; i++) {
                randGridData[r.Next(rows), r.Next(cols)] = true;
            }

            var gridSeq = lifeBDir.Make(
                new GridBuilder<bool>()
                    .Data(randGridData),
                ConwayLifeBuilderDirector.UpdateStrategyMode.Sequential
            );
            var gridPar = lifeBDir.Make(
                new GridBuilder<bool>()
                    .Data(randGridData),
                ConwayLifeBuilderDirector.UpdateStrategyMode.Parallel
            );
            var gridVec = lifeBDir.Make(
                new GridBuilder<bool>()
                    .Data(randGridData),
                ConwayLifeBuilderDirector.UpdateStrategyMode.Vectorized
            );

            var iter = 10_000;
            var batch = 1_000;

            Console.WriteLine($"Debugging grid update strategies with {rows}x{cols} grid; running {iter} iterations (mini-batched every {batch} iterations)");

            Console.WriteLine($"Running {iter} iterations of sequential grid update");
            var lastTime = 0L;
            var sw = Stopwatch.StartNew();
            for(var i = 0; i < iter; i++) {
                gridSeq.Update();
                if(i % batch == batch - 1) {
                    sw.Stop();

                    var elapsed = sw.ElapsedMilliseconds - lastTime;
                    Console.WriteLine($"| Iteration {i + 1} / {iter}, elapsed {elapsed}ms ({(float)elapsed / 1000} seconds) since last batch");
                    lastTime = sw.ElapsedMilliseconds;

                    if(i != iter - 1) sw.Start();
                }
            }
            sw.Stop();

            var seqT = sw.ElapsedMilliseconds;

            Console.WriteLine($"Running {iter} iterations of parallel grid update");
            lastTime = 0L;
            sw.Restart();
            for(var i = 0; i < iter; i++) {
                gridPar.Update();
                if(i % batch == batch - 1) {
                    sw.Stop();

                    var elapsed = sw.ElapsedMilliseconds - lastTime;
                    Console.WriteLine($"| Iteration {i + 1} / {iter}, elapsed {elapsed}ms ({(float)elapsed / 1000} seconds) since last batch");
                    lastTime = sw.ElapsedMilliseconds;

                    if(i != iter - 1) sw.Start();
                }
            }
            sw.Stop();

            var parT = sw.ElapsedMilliseconds;

            Console.WriteLine($"Running {iter} iterations of vectorized grid update");
            lastTime = 0L;
            sw.Restart();
            for(var i = 0; i < iter; i++) {
                gridVec.Update();
                if(i % batch == batch - 1) {
                    sw.Stop();

                    var elapsed = sw.ElapsedMilliseconds - lastTime;
                    Console.WriteLine($"| Iteration {i + 1} / {iter}, elapsed {elapsed}ms ({(float)elapsed / 1000} seconds) since last batch");
                    lastTime = sw.ElapsedMilliseconds;

                    if(i != iter - 1) sw.Start();
                }
            }
            sw.Stop();

            var vecT = sw.ElapsedMilliseconds;

            Console.WriteLine("Result:");
            Console.WriteLine($"Sequential          : {seqT}ms ({(float)seqT / 1000} seconds)");
            Console.WriteLine($"Parallel            : {parT}ms ({(float)parT / 1000} seconds)");
            Console.WriteLine($"Vectorized          : {vecT}ms ({(float)vecT / 1000} seconds)");
        } else {
            var rows = 30;
            var cols = 60;
            var randGridData = new bool[rows, cols];
            var r = new Random();
            for(int i = 0; i < rows * cols / 4; i++) {
                randGridData[r.Next(rows), r.Next(cols)] = true;
            }

            var grid = lifeBDir.Make(
                new GridBuilder<bool>()
                    .Data(randGridData),
                ConwayLifeBuilderDirector.UpdateStrategyMode.Parallel
            );

            while(true) {
                PrintGrid(grid);
                Thread.Sleep(50);
                grid.Update();
            }
        }
    }
}
