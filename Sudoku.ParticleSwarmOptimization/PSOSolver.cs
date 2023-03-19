using System;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public class PSOSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            // Initialization of the hive
            var hive = new Hive(10, 1, s);
            Console.WriteLine("test");
            throw new System.NotImplementedException();
        }
    }
}