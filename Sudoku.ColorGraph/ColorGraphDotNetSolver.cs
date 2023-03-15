using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Shared;

namespace Sudoku.ColorGraph
{
    public class ColorGraphDotNetSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            // launch the solver
            new CGraph(s);
            return s;
        }
    }
}

