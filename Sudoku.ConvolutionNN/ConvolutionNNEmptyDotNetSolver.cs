using Sudoku.Shared;

namespace Sudoku.ConvolutionNN;

public class  ConvolutionNNEmptyDotNetSolver : Sudoku.Shared.ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
    {
        return s;
    }
}