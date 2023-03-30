using System;
using System.Collections.Generic;
using System.Linq;
using DlxLib;

namespace Sudoku.DLXlib
{
    class Row
    {
        private int _rowIndex;
        private int _colIndex;
        private int _boxIndex;

        public Row(int row, int col, int num)
        {
            int rowIndex = row * 9 + col;
            int colIndex = 81 + row * 9 + num - 1;
            int boxIndex = 162 + (row / 3 * 3 + col / 3) * 9 + num - 1;

            this._rowIndex = rowIndex;
            this._colIndex = colIndex;
            this._boxIndex = boxIndex;
        }
    }


    
    public class SudokuSolver
    {
        public int[][] Solve(int[][] inputSudoku)
        {
            var dlx = new Dlx();

            var matrix = GenerateExactCoverMatrix(inputSudoku);

            var solutions = dlx.Solve<IEnumerable<Row>>(matrix);

            return ConvertToSudokuGrid(solutions.First());
        }

        private IEnumerable<Row> GenerateExactCoverMatrix(int[][] inputSudoku)
        {
            var rows = new List<Row>();

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (inputSudoku[row][col] == 0)
                    {
                        for (int num = 1; num <= 9; num++)
                        {
                            rows.Add(new Row(row, col, num));
                        }
                    }
                    else
                    {
                        rows.Add(new Row(row, col, inputSudoku[row][col]));
                    }
                }
            }

            return rows;
        }

        private int[][] ConvertToSudokuGrid(Solution solution)
        {
            var sudokuGrid = new int[9][];

            for (int i = 0; i < 9; i++)
            {
                sudokuGrid[i] = new int[9];
            }

            foreach (var row in solution.RowIndexes)
            {
                int rowIdx = row / 9;
                int colIdx = row % 9;
                int num = row % 9 + 1;
                sudokuGrid[rowIdx][colIdx] = num;
            }

            return sudokuGrid;
        }

    }
}