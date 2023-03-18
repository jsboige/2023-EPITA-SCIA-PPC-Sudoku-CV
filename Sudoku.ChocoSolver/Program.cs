using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

using Sudoku.Shared;

namespace Sudoku.ChocoSolver
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Empty Program");
            string filePath  = "Puzzles/Sudoku_Easy51.txt";
            var sudoku = Shared.SudokuGrid.ReadSudokuFile(filePath);
            
            ChocoSolver solver = new ChocoSolver();
            var solved = solver.Solve(sudoku[0]);
        }
    }
}
