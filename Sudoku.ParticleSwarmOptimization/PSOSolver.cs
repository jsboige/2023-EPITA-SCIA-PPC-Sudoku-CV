using System;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public class PSOSolver : ISudokuSolver
    {
        public static SudokuGrid original;
        public SudokuGrid Solve(SudokuGrid s)
        {
            // Initialization of the hive
            original = s;
            var hive = new Hive(10, 1);
            Console.WriteLine("test");
            return hive.Solve();
        }
    }
}