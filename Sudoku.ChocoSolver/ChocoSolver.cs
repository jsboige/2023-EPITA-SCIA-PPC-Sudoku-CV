using Sudoku.Shared;
using fr.epita;

namespace Sudoku.ChocoSolver;

public class ChocoSolverDefault : ISudokuSolver
{
	protected fr.epita.SudokuSolver JavaSolver;
	public SudokuGrid Solve(SudokuGrid s)
	{
		var toSolve = s.CloneSudoku();
		this.JavaSolver = new fr.epita.SudokuSolver(toSolve.Cells);
		Solve();
		return toSolve;
	}

	protected virtual void Solve()
	{
		this.JavaSolver.solve(-1);
	}
}

public class ChocoSolverInputOrderLBSearch : ChocoSolverDefault
{
	protected override void Solve()
	{
		this.JavaSolver.solve(0);
	}
}

public class ChocoSolverDomOverWDegSearch : ChocoSolverDefault
{
	protected override void Solve()
	{
		this.JavaSolver.solve(1);
	}
}

public class ChocoSolverMinDomLBSearch : ChocoSolverDefault
{
	protected override void Solve()
	{
		this.JavaSolver.solve(2);
	}
}

public class ChocoSolverRandomSearch : ChocoSolverDefault
{
	protected override void Solve()
	{
		this.JavaSolver.solve(3);
	}
}

public class ChocoSolverConflictHistorySearch : ChocoSolverDefault
{
	protected override void Solve()
	{
		this.JavaSolver.solve(4);
	}
}

public class ChocoSolverActivityBasedSearch : ChocoSolverDefault
{
	protected override void Solve()
	{
		this.JavaSolver.solve(5);
	}
}

public class ChocoSolverFailureRateBasedSearch : ChocoSolverDefault
{
	protected override void Solve()
	{
		this.JavaSolver.solve(6);
	}
}