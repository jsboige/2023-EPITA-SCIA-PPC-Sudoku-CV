using GeneticSharp.Extensions;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp
{
    public class GeneticSharpDotNetSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            List<int> sdkList = new List<int>();
            for (int i = 0; i < s.Cells.Length; i++)
            {
                for (int j = 0; j < s.Cells[0].Length; j++)
                {
                    sdkList.Add(s.Cells[i][j]);
                }
            }

            SudokuBoard sdkB = new SudokuBoard(sdkList);
            var chromosome = new SudokuPermutationsChromosome(sdkB);
            var sdkBoard = SudokuTestHelper.Eval(chromosome, sdkB, 25000, 0, 200);

            for (int i = 0; i < s.Cells.Length; i++)
            {
                for (int j = 0; j < s.Cells[0].Length; j++)
                {
                    s.Cells[i][j] = sdkBoard.GetCell(i, j);
                }
            }

            return s;
        }
    }
}