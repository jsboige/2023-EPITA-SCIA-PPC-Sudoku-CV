using Sudoku.Shared;

namespace Sudoku.Demo
{
	public class EmptyDotNetSolver : ISudokuSolver
	{
		public SudokuGrid Solve(SudokuGrid s)
		{
			return s;
		}
	}
}