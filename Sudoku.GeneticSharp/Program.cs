using GeneticSharp;
using GeneticSharp.Extensions;
using Sudoku.Benchmark;
using Sudoku.Shared;


namespace Sudoku.GeneticSharp
{
    class Program
    {
        public static void Main(string[] args)
        {
            // Create a sudoku board from SudokuGrid
            List<SudokuGrid> sudokus = SudokuHelper.GetSudokus(SudokuDifficulty.Easy);
            SudokuGrid sdk = sudokus[0];
            
            // Make the sdk an enumerable<int>
            List<int> sdkList = new List<int>();
            for (int i = 0; i < sdk.Cells.Length; i++)
            {
                for (int j = 0; j < sdk.Cells[0].Length; j++)
                {
                    sdkList.Add(sdk.Cells[i][j]);
                }
            }

            // Solve a sudoku using GeneticSharp extension
            SudokuBoard sdkB = new SudokuBoard(sdkList);
            

            // Print the solved sudoku
            //Console.WriteLine(sdkB.ToString());
            
            var sudoku = SudokuTestHelper.CreateBoard(SudokuTestDifficulty.VeryEasy);

            //the cells chromosome should solve the sudoku in less than 30 generations with 500 chromosomes
            var chromosome = new SudokuCellsChromosome(sudoku);
            var fitness = SudokuTestHelper.Eval(chromosome, sudoku, 500, 0, 30);
            Console.WriteLine($"Fitness: {fitness}");
        }
    }
}

