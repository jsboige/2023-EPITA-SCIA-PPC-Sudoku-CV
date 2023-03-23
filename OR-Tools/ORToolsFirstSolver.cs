using System;
using Google.OrTools.ConstraintSolver;
using Google.OrTools.Sat;
using Sudoku.Shared;

namespace Sudoku.OR.Tools;

public class ORToolsFirstSolver : Sudoku.Shared.ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
    {
        CpModel model = new CpModel();
        return s;
    } 
}