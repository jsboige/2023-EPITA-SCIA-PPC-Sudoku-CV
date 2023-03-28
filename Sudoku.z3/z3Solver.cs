using Sudoku.Shared;
using System;
using Microsoft.Z3;

namespace Sudoku.z3
{
	public class z3Solver : ISudokuSolver
	{
		public z3Solver()
		{

		}
		public SudokuGrid Solve(SudokuGrid s)
		{
			// each cell contains a value in {1, ..., 9}
			Context ctx = new Context();
			Solver solver = ctx.MkSolver();
			BoolExpr[] cellsC = new BoolExpr[9 * 9];
			IntExpr[,] X = new IntExpr[9,9];
			for (int i = 0; i < 9; i++){
				for (int j = 0; j < 9; j++){
					cellsC[i * 9 + j] = ctx.MkAnd(ctx.MkGe(X[i, j], ctx.MkInt(1)), ctx.MkLe(X[i, j], ctx.MkInt(9)));
				}
			}
			
			// each row contains a digit at most once
			BoolExpr[] rowsC = new BoolExpr[9];
			for (int i = 0; i < 9; i++)
			{
				ArithExpr[] row = new ArithExpr[9];
				for (int j = 0; j < 9; j++){
					row[j] = X[i, j];
				}
				rowsC[i] = ctx.MkDistinct(row);
			}

			// each column contains a digit at most once
			BoolExpr[] colsC = new BoolExpr[9];
			for (int j = 0; j < 9; j++)
			{
				ArithExpr[] col = new ArithExpr[9];
				for (int i = 0; i < 9; i++){
					col[i] = X[i, j];
				}
				colsC[j] = ctx.MkDistinct(col);
			}

			// each 3x3 square contains a digit at most once
			BoolExpr[] sqsC = new BoolExpr[9];
			for (int I = 0; I < 3; I++){
				for (int J = 0; J < 3; J++)
				{
					ArithExpr[] sq = new ArithExpr[9];
					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							sq[i * 3 + j] = X[3 * I + i, 3 * J + j];
						}
					}
					sqsC[I * 3 + J] = ctx.MkDistinct(sq);
				}
			}
				

			// combine constraints
			BoolExpr sudokuC = ctx.MkAnd(cellsC);
			sudokuC = ctx.MkAnd(sudokuC, ctx.MkAnd(rowsC));
			sudokuC = ctx.MkAnd(sudokuC, ctx.MkAnd(colsC));
			sudokuC = ctx.MkAnd(sudokuC, ctx.MkAnd(sqsC));

			// add known cells
			// for (int i = 0; i < 9; i++){
			// 	for (int j = 0; j < 9; j++){
			// 		if (s.Cells[i][j] != 0){
			// 			sudokuC = ctx.MkAnd(sudokuC, ctx.MkEq(X[i, j], ctx.MkInt(s.Cells[i][j])));
			// 		}
			// 	}
			// }
			BoolExpr[] instanceC = new BoolExpr[9 * 9];
			for (int i = 0; i < 9; i++){
				for (int j = 0; j < 9; j++){
					if (s.Cells[i][j] == 0){
						instanceC[i * 9 + j] = ctx.MkEq(X[i, j], ctx.MkInt(s.Cells[i][j]));
					}
				}
			}

			//create the solver
			
			solver.Add(instanceC);
			solver.Add(sudokuC);
			//check satisfiability
			SudokuGrid r = new SudokuGrid();
			if (solver.Check() == Status.SATISFIABLE) {
				Model model = solver.Model;
				
				for (int i = 0; i < 9; i++) {
					for (int j = 0; j < 9; j++) {
						r.Cells[i][j] = Int32.Parse(model.Evaluate(X[i, j]).ToString());
					}
				}
			} else {
				Console.WriteLine("failed to solve");
			}

			return r;
		}
	}
}