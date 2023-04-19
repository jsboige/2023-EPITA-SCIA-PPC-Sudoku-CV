using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization.TSPPSO {

    public class Route
    {
        public int[] Cells{ get; set; }
        public int Error { get; set; }
        public int SegmentSize { get; set; }

        public Route () {
            Cells = new int[81];
        }
        public Route(SudokuGrid original)
        {
            Cells = new int[81];
            var used = new List<int>();
            
            // Add constraints from original sudoku
            foreach (var rowIndex in SudokuGrid.NeighbourIndices)
                foreach (var colIndex in SudokuGrid.NeighbourIndices) {
                    if (original.Cells[rowIndex][colIndex] > 0) {
                        var city = (original.Cells[rowIndex][colIndex] - 1) + (rowIndex * 9);
                        Cells[rowIndex * 9 + colIndex] = city;
                        used.Add(city);
                    } else
                        Cells[rowIndex * 9 + colIndex] = -1;
                }
            
            // Fill the rest with the remaining values
            var remaining = new List<int>();
            for (int i = 0; i < 81; i++)
                if (!used.Contains(i))
                    remaining.Add(i);
            var remainingIndex = 0;
            for (int i = 0; i < 81; i++)
                if (Cells[i] == -1)
                    Cells[i] = remaining[remainingIndex++];

            CalculateError();
        }

        public void Print()
        {
            for (int i = 0; i < 81; i++)
            {
                if (Cells[i] < 10)
                    Console.Write(" ");
                Console.Write(Cells[i] + " ");
                if ((i + 1) % 9 == 0)
                    Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static Route CloneRoute(Route original)
        {
            var clone = new Route();
            Array.Copy(original.Cells, clone.Cells, original.Cells.Length);
            clone.Error = original.Error;
            return clone;
        }

        public Route CloneRoute()
        {
            return CloneRoute(this);
        }

        public void CopyTo(Route destination)
        {
            Array.Copy(Cells, destination.Cells, Cells.Length);
        }

        public int CalculateError()
        {
            var original = TSPPSOSolver.original;
            var error = 0;
            for (int i = 0; i < 81; i++)
            {
                var row = i / 9;
                var col = i % 9;
                var value = Cells[i] % 9 + 1;
                if (original.Cells[row][col] > 0 && original.Cells[row][col] != value)
                    error++;
            }

            // We use a large lambda expression to count duplicates in rows, columns and boxes
            error += SudokuGrid.AllNeighbours.Select(n => n.Select(nx => this.Cells[nx.row * 9 + nx.column] % 9))
                .Sum(n => n.GroupBy(x => x).Select(g => g.Count() - 1).Sum());

            this.Error = error;
            return error;
        }

        public SudokuGrid ToSudokuGrid()
        {
            var grid = new SudokuGrid();
            for (int i = 0; i < 81; i++)
            {
                var row = i / 9;
                var col = i % 9;
                grid.Cells[row][col] = Cells[i] % 9 + 1;
            }

            return grid;
        }
    }
}