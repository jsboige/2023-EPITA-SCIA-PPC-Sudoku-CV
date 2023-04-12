using GeneticSharp.Extensions;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp
{
    public class GeneticSharpDotNetSolver : ISudokuSolver
    {
        private SudokuBoard GridToBoard(SudokuGrid s)
        {
            List<int> sdkList = new List<int>();
            for (int i = 0; i < s.Cells.Length; i++)
            {
                for (int j = 0; j < s.Cells[0].Length; j++)
                {
                    sdkList.Add(s.Cells[i][j]);
                }
            }

            return new SudokuBoard(sdkList);
        }
        
        
        private SudokuGrid BoardToGrid(SudokuBoard s)
        {
            SudokuGrid sdkGrid = new SudokuGrid();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sdkGrid.Cells[i][j] = s.GetCell(i, j);
                }
            }
            return sdkGrid;
        }
        
        public SudokuGrid Solve(SudokuGrid s)
        {
            SudokuBoard sdkB = GridToBoard(s);
            
            SudokuChromosome chromosome = new SudokuChromosome(sdkB);
            // var chromosome = new SudokuCellsChromosome(sdkB);
            
            Console.WriteLine("Enter population size:");
            string populationSize = Console.ReadLine();
            int.TryParse(populationSize, out int popSize);
            Console.WriteLine("Enter number of generations:");
            string genNb = Console.ReadLine();
            int.TryParse(genNb, out int genNbInt);
            
            var sdkBoard = SudokuTestHelper.Eval(chromosome, sdkB, popSize, 0, genNbInt);

            return BoardToGrid(sdkBoard);
        }
    }
}