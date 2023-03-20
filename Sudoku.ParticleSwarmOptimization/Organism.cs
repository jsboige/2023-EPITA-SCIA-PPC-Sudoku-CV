using System;
using System.Collections.Generic;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public abstract class Organism : IComparable
    {
        protected SudokuGrid solution;

        public SudokuGrid Solution => solution;

        protected int error;
        public int Error => error;

        public Organism()
        {
            solution = SudokuUtils.RandomSolution();
            error = solution.NbErrors(PSOSolver.original);
        }

        public int CompareTo(object? other)
        {
            if (other == null || other.GetType() != GetType())
                throw new ArgumentException();
            return Error.CompareTo(((Organism) other).Error);
        }

        public abstract void SearchBetterSolution();
    }
}