using Sudoku.Shared;
using fr.epita;

namespace Sudoku.ChocoSolver;

public class ChocoSolverBase : ISudokuSolver
{
	public SudokuGrid Solve(SudokuGrid s)
	{
		var toSolve = s.CloneSudoku();
		var javaSolver = new fr.epita.SudokuSolver(toSolve.Cells);

		javaSolver.solve();

		return toSolve;
	}
}