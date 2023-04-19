using Microsoft.Z3;
using Sudoku.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Z3.LinqBinding;

namespace LinqToZ3
{

    public class LinqToZ3SolverApiZ3Byte : ISudokuSolver
    {

        private static BoolExpr CreateZ3InstanceConstraint(SudokuGrid s, Context z3Context, Expr[][] X, BoolExpr genericContraints)
        {
            var substExprs = new List<Expr>();
            var substVals = new List<Expr>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s.Cells[i][j] != 0)
                    {
                        substExprs.Add(X[i][j]);
                        substVals.Add(z3Context.MkInt(s.Cells[i][j]));
                    }
                }
            }

            BoolExpr instance_c = (BoolExpr)genericContraints.Substitute(substExprs.ToArray(), substVals.ToArray());
            return instance_c;
        }

        public SudokuGrid Solve(SudokuGrid s)
        {
            using (var z3Context = new Context())
            {
                // Create the X variable representing the Sudoku grid
                var X = new Expr[9][];
                for (int i = 0; i < 9; i++)
                {
                    X[i] = new Expr[9];
                    for (int j = 0; j < 9; j++)
                    {
                        X[i][j] = z3Context.MkIntConst($"X_{i}_{j}");
                    }
                }

                // Create the generic constraints (rows, columns, and boxes)
                BoolExpr genericConstraints = CreateGenericConstraints(z3Context, X, s);

                // Create the instance constraint using the Z3 API
                BoolExpr instanceConstraint = CreateZ3InstanceConstraint(s, z3Context, X, genericConstraints);

                // Combine the constraints and solve the Sudoku
                Solver solver = z3Context.MkSolver();
                solver.Add(genericConstraints);
                solver.Add(instanceConstraint);
                Status status = solver.Check();

                if (status == Status.SATISFIABLE)
                {
                    Model model = solver.Model;
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            s.Cells[i][j] = (byte)((IntNum)model.Evaluate(X[i][j])).Int;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("Sudoku grid has no solution.");
                }
            }

            return s;
        }

        private static BoolExpr CreateGenericConstraints(Context z3Context, Expr[][] X, SudokuGrid s)
        {
            var constraints = new List<BoolExpr>();

            // Cell constraints
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    constraints.Add(z3Context.MkAnd(z3Context.MkLe((ArithExpr)X[i][j], z3Context.MkInt(9)),
                                                     z3Context.MkGe((ArithExpr)X[i][j], z3Context.MkInt(1))));
                }
            }

            // Row constraints
            for (int r = 0; r < 9; r++)
            {
                constraints.Add(z3Context.MkDistinct(X[r]));
            }

            // Column constraints
            for (int c = 0; c < 9; c++)
            {
                constraints.Add(z3Context.MkDistinct(Enumerable.Range(0, 9).Select(i => X[i][c]).ToArray()));
            }

            // Box constraints
            for (int b = 0; b < 9; b++)
            {
                int iStart = b / 3 * 3;
                int jStart = b % 3 * 3;
                constraints.Add(z3Context.MkDistinct(new Expr[]
                {
                    X[iStart][jStart],
                    X[iStart][jStart + 1],
                    X[iStart][jStart + 2],
                    X[iStart + 1][jStart],
                    X[iStart + 1][jStart + 1],
                    X[iStart + 1][jStart + 2],
                    X[iStart + 2][jStart],
                    X[iStart + 2][jStart + 1],
                    X[iStart + 2][jStart + 2]
                }));
            }

            // Add a constraint to keep the starting numbers unchanged
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s.Cells[i][j] != 0)
                    {
                        BoolExpr cellFixed = z3Context.MkEq(X[i][j], z3Context.MkInt(s.Cells[i][j]));
                        constraints.Add(cellFixed);
                    }
                }
            }

            return z3Context.MkAnd(constraints.ToArray());
        }
    }
}