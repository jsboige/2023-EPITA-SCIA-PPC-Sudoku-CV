using System;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public class Organism : IComparable
    {
        private bool isWorker;
        private SudokuGrid original;
        private SudokuGrid solution;
        private int error;
        public int Error
        {
            get { return error; }
        }

        public Organism(SudokuGrid s, bool is_worker)
        {
            isWorker = is_worker;
            original = s;
            solution = s.CloneSudoku();
            
            // Fill the Sudoku grid with random values but repecting column constraint
            Random r = new Random();
            for (int i = 0; i < 9; ++i)
            {
                int[] possibleNumbers = SudokuGridHelpers.AvailableNumbersForColumn(solution, i);
                for (int j = 0; j < 9; ++j)
                {
                    if (solution.Cells[i][j] != 0)
                    {
                        continue;
                    }

                    int randomIndex = 0;
                    do
                    {
                        randomIndex =  r.Next(0, possibleNumbers.Length);
                    } while (possibleNumbers[randomIndex] == 0);
                    solution.Cells[i][j] = possibleNumbers[randomIndex];
                    possibleNumbers[randomIndex] = 0;
                }
            }
            error = solution.NbErrors(s);
            Console.WriteLine($"Error: {Error} worker? {is_worker}");
        }

        public int CompareTo(object other)
        {
            if (other.GetType() != GetType())
                throw new ArgumentException();
            return Error.CompareTo(((Organism) other).Error);
        }
    }
}