using System;
using System.Collections.Generic;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public abstract class Organism : IComparable
    {
        protected SudokuGrid solution;

        public SudokuGrid Solution => solution;

        private int error;
        public int Error
        {
            get { return error; }
        }

        public Organism()
        {
            solution = PSOSolver.original.CloneSudoku();
            
            // Fill the Sudoku grid with random values but respecting line constraint
            Random r = new Random();
            foreach (var rowIndex in SudokuGrid.NeighbourIndices)
            {
                // Get possible numbers in a column
                var possibleNumbers = new List<int>(){1,2,3,4,5,6,7,8,9};
                for (int j = 0; j < 9; ++j)
                {
                    if (solution.Cells[rowIndex][j] != 0)
                        possibleNumbers.Remove(solution.Cells[rowIndex][j]);
                }

                // Place possible numbers
                foreach (var colIndex in SudokuGrid.NeighbourIndices)
                {
                    if (solution.Cells[rowIndex][colIndex] != 0)
                        continue;

                    var randomIndex =  r.Next(0, possibleNumbers.Count);
                    solution.Cells[rowIndex][colIndex] = possibleNumbers[randomIndex];
                    possibleNumbers.RemoveAt(randomIndex);
                }
            }
            error = solution.NbErrors(PSOSolver.original);
        }

        public int CompareTo(object other)
        {
            if (other.GetType() != GetType())
                throw new ArgumentException();
            return Error.CompareTo(((Organism) other).Error);
        }

        public abstract void SearchBetterSolution();
    }
}