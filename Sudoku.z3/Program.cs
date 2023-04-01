using System;
using Microsoft.Z3;
using System;

namespace SudokuSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            // Définir une grille de sudoku
            int[,] grid = new int[,]
            {
                { 0, 3, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 7, 0, 0, 0, 0, 5, 0 },
                { 5, 0, 0, 0, 1, 8, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 8, 0, 0 },
                { 0, 2, 0, 0, 9, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 4, 0, 3, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 9, 4 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            // Créer un solveur Z3
            using (Context ctx = new Context())
            {
                Solver solver = ctx.MkSolver();

                // Créer une variable pour chaque case de la grille
                IntExpr[,] vars = new IntExpr[9, 9];
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        vars[i, j] = (IntExpr)ctx.MkIntConst($"{i},{j}");
                        solver.Add(ctx.MkAnd(ctx.MkGe(vars[i, j], ctx.MkInt(1)), ctx.MkLe(vars[i, j], ctx.MkInt(9))));
                    }
                }

                // Ajouter les contraintes de rangée, de colonne et de région
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        // Contrainte de rangée
                        BoolExpr rowConstraint = ctx.MkTrue();
                        for (int k = 0; k < 9; k++)
                        {
                            if (k != j)
                            {
                                var notEq =  ctx.MkNot(ctx.MkEq(vars[i, j], vars[i, k]));
                                rowConstraint = ctx.MkAnd(rowConstraint, notEq);
                            }
                        }

                        // Contrainte de colonne
                        BoolExpr colConstraint = ctx.MkTrue();
                        for (int k = 0; k < 9; k++)
                        {
                            if (k != i)
                            {
                                colConstraint = ctx.MkAnd(colConstraint, ctx.MkNot(ctx.MkEq(vars[i, j], vars[k, j])));
                            }
                        }

                        // Contrainte de région
                        int rowRegion = (i / 3) * 3;
                        int colRegion = (j / 3) * 3;
                        BoolExpr regionConstraint = ctx.MkTrue();
                        for (int k = rowRegion; k < rowRegion + 3; k++)
                        {
                            for (int l = colRegion; l < colRegion + 3; l++)
                            {
                                if (k != i || l != j)
                                {
                                    regionConstraint = ctx.MkAnd(regionConstraint, ctx.MkNot(ctx.MkEq(vars[i, j], vars[k, l])));
                                }
                            }
                        } 
                    
                        // Ajouter les contraintes à la grille
                        solver.Assert(rowConstraint);
                        solver.Assert(colConstraint);
                        solver.Assert(regionConstraint);

                        // Ajouter les valeurs connues de la grille
                        if (grid[i, j] != 0)
                        {
                            solver.Assert(ctx.MkEq(vars[i, j], ctx.MkInt(grid[i, j])));
                        } 
                    }
                        
                }
                
                // Résoudre le sudoku
                if (solver.Check() == Status.SATISFIABLE)
                {
                    Model model = solver.Model;
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            Console.Write($"{model.Evaluate(vars[i, j])} ");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Le sudoku n'a pas de solution.");
                } 
            }
        }
    }
}
