using Sudoku.Shared;

using Google.OrTools.LinearSolver;
using Solver = Google.OrTools.LinearSolver.Solver;


namespace Sudoku.OrTools
{

    public class OrToolsMIPSolver : ISudokuSolver
    {
        private const int GridSize = 9;
        private const int CellSize = 3;

        public SudokuGrid Solve(SudokuGrid s)
        {
            Solver solver = Solver.CreateSolver("CBC_MIXED_INTEGER_PROGRAMMING");
            
            
            // Step 2: Create variables.
            Variable[,,] x = new Variable[GridSize, GridSize, GridSize];
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    // Initial values.
                    for (int k = 0; k < GridSize; k++)
                    {
                        x[i, j, k] = solver.MakeBoolVar($"x[{i},{j},{k}]");
                    }
                }
            }

            // Step 3: Initialize variables in case of known (defined) values.
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    bool defined = s.Cells[i][j] != 0;
                    if (defined)
                    {
                        solver.Add(x[i, j, s.Cells[i][j] - 1] == 1);
                    }
                }
            }

            // Step 4: All bins of a cell must have sum equals to 1
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    var value = new LinearExpr();                    
                    for (int k = 0; k < GridSize; k++)
                    {
                        value += x[i, j, k];
                    }

                    solver.Add(value == 1);
                }
            }

            // Step 5: Create variables.
            for (int k = 0; k < GridSize; k++)
            {
                // AllDifferent on rows.
                for (int i = 0; i < GridSize; i++)
                {
                    var value = new LinearExpr();
                    for (int j = 0; j < GridSize; j++)
                    {
                        value += x[i, j, k];
                    }

                    solver.Add(value == 1);
                }

                // AllDifferent on columns.
                for (int j = 0; j < GridSize; j++)
                {
                    var value = new LinearExpr();
                    for (int i = 0; i < GridSize; i++)
                    {
                        value += x[i, j, k];
                    }

                    solver.Add(value == 1);
                }

                // AllDifferent on regions.
                for (int rowIdx = 0; rowIdx < GridSize; rowIdx += CellSize)
                {
                    for (int colIdx = 0; colIdx < GridSize; colIdx += CellSize)
                    {
                        var sum = new LinearExpr();
                        for (int j = colIdx; j < colIdx + CellSize; j++)
                        {
                            for (int i = rowIdx; i < rowIdx + CellSize; i++)
                            {
                                sum += x[i, j, k];
                            }
                        }
                        solver.Add(sum == 1);
                    }
                }
            }

            // Solve and print out the solution.
            Solver.ResultStatus status = solver.Solve();
            
            
            if (status is Solver.ResultStatus.FEASIBLE or Solver.ResultStatus.OPTIMAL)
            {
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        int val = -1;
                        for (int k = 0; k < GridSize; k++)
                        {
                            if (x[i, j, k].SolutionValue() > 0.5)
                            {
                                val = k + 1;
                                break;
                            }
                        }
                        s.Cells[i][j] = val;
                    }
                }
            }

            return s;
        }
    }
}