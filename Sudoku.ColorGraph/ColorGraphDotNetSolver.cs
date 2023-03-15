using Sudoku.Shared;

namespace Sudoku.ColorGraph
{
    public class ColorGraphDotNetSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            new CGraph(s);
	        return s;
        }
    }
}

