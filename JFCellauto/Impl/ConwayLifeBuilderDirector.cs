using JFCellauto.Algorithms;
using JFCellauto.Structs;

using VByte = System.Numerics.Vector<byte>;

namespace JFCellauto.Impl;

/// <summary>
/// A builder director that constructs <see cref="Grid{T}"/> instances following the specification and predefined rules
/// of Conway's Game of Life.
/// </summary>
public sealed class ConwayLifeBuilderDirector {
    internal class ConwayLifeVectorizedUpdateStrategy : IGridUpdateStrategy<bool> {
        private static VByte Fetch(byte[] buf, Grid<bool> grid, int i, int vSize, byte mode) {
            // mode to neighbor map:
            // 0 1 2
            // 3 X 5
            // 6 7 8
            var u = mode < 3;
            var d = mode > 5;
            var l = mode % 3 == 0;
            var r = (mode - 2) % 3 == 0;

            var offset = 0;
            if(u) offset -= grid.Bounds.Y;
            if(d) offset += grid.Bounds.Y;
            if(l) offset -= 1;
            if(r) offset += 1;

            var temp = new byte[vSize];
            for(var j = 0; j < vSize; j++) {
                var idx = i + j;
                var row = idx / grid.Bounds.Y;
                var col = idx % grid.Bounds.Y;
                var currOffset = offset;

                if(grid.WrapEdges) {
                    if(u && row == 0) {
                        currOffset += grid.Bounds.X * grid.Bounds.Y;
                    }
                    if(d && row == grid.Bounds.X - 1) {
                        currOffset -= grid.Bounds.X * grid.Bounds.Y;
                    }
                    if(l && col == 0) {
                        currOffset += grid.Bounds.Y;
                    }
                    if(r && col == grid.Bounds.Y - 1) {
                        currOffset -= grid.Bounds.Y;
                    }
                }

                if(
                    grid.WrapEdges || (
                        (!u || row > 0)
                        && (!d || row < grid.Bounds.X - 1)
                        && (!l || col > 0)
                        && (!r || col < grid.Bounds.Y - 1)
                    )
                ) {
                    temp[j] = buf[idx + currOffset];
                } else {
                    temp[j] = 0;
                }
            }

            return new VByte(temp);
        }

        public void GetNextGeneration(Grid<bool> grid, CellBuffer<bool> outBuffer) {
            var inBufferByte = Array.ConvertAll(grid.FrontBuffer.InnerBuffer(), Convert.ToByte);
            var outBufferRaw = outBuffer.InnerBuffer();
            var vSize = VByte.Count;
            var cellCount = grid.Bounds.X * grid.Bounds.Y;

            var i = 0;
            for(; i < cellCount - vSize; i += vSize) {
                var vCurr = new VByte(inBufferByte, i);
                var vNeighborSum =
                    Fetch(inBufferByte, grid, i, vSize, 0)
                    + Fetch(inBufferByte, grid, i, vSize, 1)
                    + Fetch(inBufferByte, grid, i, vSize, 2)
                    + Fetch(inBufferByte, grid, i, vSize, 3)
                    + Fetch(inBufferByte, grid, i, vSize, 5)
                    + Fetch(inBufferByte, grid, i, vSize, 6)
                    + Fetch(inBufferByte, grid, i, vSize, 7)
                    + Fetch(inBufferByte, grid, i, vSize, 8);

                for(var j = 0; j < vSize && i + j < cellCount; j++) {
                    var neighborCount = vNeighborSum[j];
                    outBufferRaw[i + j] = neighborCount == 3 || (neighborCount == 2 && inBufferByte[i + j] == 1);
                }
            }

            for(; i < cellCount; i++) {
                var x = i / grid.Bounds.Y;
                var y = i % grid.Bounds.Y;

                var neighborCount = grid.Neighbors(x, y)
                    .Where(cell => cell)
                    .Count();
                outBuffer[x, y] = neighborCount == 3 || (neighborCount == 2 && inBufferByte[i] == 1);
            }
        }
    }

    public enum UpdateStrategyMode {
        Sequential,
        Parallel,
        Vectorized,
    }

    /// <summary>
    /// Creates an empty Conway's Game of Life grid.
    /// </summary>
    /// <param name="gridBuilder">A <see cref="GridBuilder{T}"/> object that has been supplied with a bounds parameter.</param>
    /// <returns>A grid object filled with empty cells and provided with the Conway's Game of Life update rule.</returns>
    public Grid<bool> Make(IGridBuilderStep2<bool> gridBuilder, UpdateStrategyMode mode) {
        return Make(gridBuilder.Fill(false), mode);
    }

    /// <summary>
    /// Creates a Conway's Game of Life grid.
    /// </summary>
    /// <param name="gridBuilder">A <see cref="GridBuilder{T}"/> object that has been supplied with cell data.</param>
    /// <returns>A grid object filled with cells of the given data and provided with the Conway's Game of Life update rule.</returns>
    public Grid<bool> Make(IGridBuilderStep3<bool> gridBuilder, UpdateStrategyMode mode) {
        IGridUpdateStrategy<bool> updateStrategy = mode switch {
            UpdateStrategyMode.Sequential => new SequentialGridUpdateStrategy<bool>(new StepRule<bool>((x, y, val, grid) => {
                var aliveNeighbors = grid.Neighbors(x, y)
                    .Where(cell => cell)
                    .Count();

                return aliveNeighbors == 3 || (aliveNeighbors == 2 && val);
            })),
            UpdateStrategyMode.Parallel => new ParallelGridUpdateStrategy<bool>(new StepRule<bool>((x, y, val, grid) => {
                var aliveNeighbors = grid.Neighbors(x, y)
                    .Where(cell => cell)
                    .Count();

                return aliveNeighbors == 3 || (aliveNeighbors == 2 && val);
            })),
            UpdateStrategyMode.Vectorized => new ConwayLifeVectorizedUpdateStrategy(),
            _ => throw new ArgumentException("Unknown update strategy mode", nameof(mode))
        };

        return gridBuilder
            .UpdateStrategy(updateStrategy)
            .Build();
    }
}
