using System;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public class PSOSolver : ISudokuSolver
    {
        public static SudokuGrid original = new SudokuGrid();
        public SudokuGrid Solve(SudokuGrid s)
        {
            // Initialization of the hive
            original = s;
            var hive = new Hive(10, 1);
            return hive.Solve();
        }
    }
}