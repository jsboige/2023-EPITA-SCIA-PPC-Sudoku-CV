using System;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public class Worker : Organism
    {
        public override void SearchBetterSolution()
        {
            // FIXME: If is 'worker' organism then: Evolve or Mutate
            throw new System.NotImplementedException();
        }

        // Replace this Worker by merging worker and explorer
        public void Replace(Worker worker, Explorer explorer)
        {
            var rd = new Random();
            solution = worker.solution;
            
            foreach (var rowIndex in SudokuGrid.NeighbourIndices)
            {
                foreach (var colIndex in SudokuGrid.NeighbourIndices)
                {
                    // Not in solution
                    if (PSOSolver.original.Cells[rowIndex][colIndex]>0 && PSOSolver.original.Cells[rowIndex][colIndex] != solution.Cells[rowIndex][colIndex])
                    {
                        if (rd.Next(0, 100) < 50)
                        {
                            solution.Cells[rowIndex][colIndex] = explorer.Solution.Cells[rowIndex][colIndex];
                        }
                    }
                }
            }
        }
    }
}