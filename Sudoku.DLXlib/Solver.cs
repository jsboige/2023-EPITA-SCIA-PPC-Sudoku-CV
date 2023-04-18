﻿using System;
using System.Collections.Generic;
using System.Linq;
using DlxLib;
using Sudoku.Shared;

namespace Sudoku.DLXlib
{
    public class DLXSudokuSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid inputSudoku)
        {

            var dlx = new Dlx();

            var matrix = Generate(inputSudoku);

            IEnumerable<Solution> solutions = dlx.Solve(matrix);

            return ConvertToSudokuGrid(solutions.First(), matrix);
        }


        private int[,] Generate(SudokuGrid inputSudoku)
        {
            int[,] dlxMatrix = new int[729, 324];
            int cellValue;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    cellValue = inputSudoku.Cells[i][j];
                    if (cellValue != 0)
                    {
                        int[] knownRow = new int[324];
                        knownRow[i + j * 9] = 1;
                        knownRow[81 + j * 9 + cellValue - 1] = 1;
                        knownRow[81 * 2 + i * 9 + cellValue - 1] = 1;
                        knownRow[81 * 3 + ((i / 3) + (j / 3) * 3) * 9 + cellValue - 1] = 1;
                        for (int k = 0; k < 324; k++)
                        {
                            dlxMatrix[(i + j * 9) * 9, k] = knownRow[k];
                        }
                    }
                    else
                    {
                        for (int val = 1; val <= 9; val++)
                        {
                            cellValue = val;
                            int[] knownRow = new int[324];
                            knownRow[i + j * 9] = 1;
                            knownRow[81 + j * 9 + cellValue - 1] = 1;
                            knownRow[81 * 2 + i * 9 + cellValue - 1] = 1;
                            knownRow[81 * 3 + ((i / 3) + (j / 3) * 3) * 9 + cellValue - 1] = 1;
                            for (int k = 0; k < 324; k++)
                            {
                                dlxMatrix[(i + j * 9) * 9 + val - 1, k] = knownRow[k];
                            }
                        }
                    }
                }
            }
            return dlxMatrix;
        }

        /*

        private int[,] Generate(SudokuGrid inputSudoku)
        {
            int[,] dlxMatrix = new int[729, 324];

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int value = inputSudoku.Cells[row][col];

                    if (value == 0)
                    {
                        for (int num = 1; num <= 9; num++)
                        {
                            AddAssignmentToDLXMatrix(dlxMatrix, row, col, num);
                        }
                    }
                    else
                    {
                        AddAssignmentToDLXMatrix(dlxMatrix, row, col, value);
                    }
                }
            }

            return dlxMatrix;
        }

        private static void AddAssignmentToDLXMatrix(int[,] dlxMatrix, int row, int col, int num)
        {
            int rowIndex = row * 81 + col * 9 + num - 1;

            // Cell constraint
            dlxMatrix[rowIndex, row * 9 + col] = 1;

            // Row constraint
            dlxMatrix[rowIndex, 81 + row * 9 + num - 1] = 1;

            // Column constraint
            dlxMatrix[rowIndex, 162 + col * 9 + num - 1] = 1;

            // Box constraint
            int boxRow = row / 3;
            int boxCol = col / 3;
            int boxIndex = boxRow * 3 + boxCol;
            dlxMatrix[rowIndex, 243 + boxIndex * 9 + num - 1] = 1;
        }
        */

        private SudokuGrid ConvertToSudokuGrid(Solution solution, int[,] inputMatrix)
        {
            var sudokuGrid = new SudokuGrid(); //new int[9][];

            foreach (var row in solution.RowIndexes)
            {
                int cellId = -1;
                for (int i = 0; i < 81; i++)
                {
                    if (inputMatrix[row, i] == 1)
                    {
                        cellId = i;
                        break;
                    }
                }
                int cellValue = -1;
                for (int i = 81; i < 162; i++)
                {
                    if (inputMatrix[row, i] == 1)
                    {
                        cellValue = i % 9;
                        break;
                    }
                }
                sudokuGrid.Cells[cellId % 9][cellId / 9] = cellValue + 1;
            }

            return sudokuGrid;
        }
    }
}