using Google.OrTools.ConstraintSolver;
using Sudoku.Shared;

namespace Sudoku.OrTools
{
    public class OrToolsCpSolver : ISudokuSolver
    {
        private const int GridSize = 9;
        public SudokuGrid Solve(SudokuGrid s)
        {
            Solver _solver = new Solver("CpSimple");
            
            // Create all variables and constants of the sudoku grid

            IntVar[,] gridVars = new IntVar[GridSize, GridSize];
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (s.Cells[i][j] == 0)
                    {
                        gridVars[i, j] = _solver.MakeIntVar(1, 9, $"{i}{j}");
                    }
                    else
                    {
                        gridVars[i, j] = _solver.MakeIntConst(s.Cells[i][j]);
                    }
                }
            }

            // Add constraints
            for (int i = 0; i < GridSize; i++)
            {
                
                // Columns and Rows constaints
                IntVar[] col = new IntVar[9];
                IntVar[] row = new IntVar[9];

                for (int j = 0; j < GridSize; j++)
                {
                    row[j] = gridVars[i, j]; 
                    col[j] = gridVars[j, i]; 
                    
                    if (i % 3 == 1 && j % 3 == 1) // if we are at the center of a square
                    {
                        // Square constraints
                        IntVar[] square = new IntVar[9];
                        int x = 0;
                        for (int k = i - 1; k < i + 2; k++)
                        {
                            int y = 0;
                            for (int l = j - 1; l < j + 2; l++)
                            {
                                square[3*x + y] = gridVars[k, l];
                                y++;
                            }
                            x++;
                        }
                    
                        _solver.Add(_solver.MakeAllDifferent(square));
                    }
                }
                _solver.Add(_solver.MakeAllDifferent(col));
                _solver.Add(_solver.MakeAllDifferent(row));
            }

            // Call the solver
            DecisionBuilder db =
                _solver.MakePhase(gridVars.Flatten(), Solver.CHOOSE_FIRST_UNBOUND, Solver.ASSIGN_MIN_VALUE);
            
            // Imprint the solution
            _solver.NewSearch(db);
            while (_solver.NextSolution())
            {
                for (int row = 0; row < GridSize; row++)
                {
                    for (int col = 0; col < GridSize; col++)
                    {
                        s.Cells[row][col] = (int)gridVars[row, col].Value();
                    }
                }
                
                break;
            }

            return s;
        }
    }
}

