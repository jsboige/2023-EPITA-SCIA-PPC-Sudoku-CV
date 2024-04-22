using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public class Hive
    {
        private Worker[] workers;
        private Explorer[] explorers;

        private Worker bestWorker;
        private Explorer bestExplorer;
        private Organism bestOrganism;

        public Organism BestOrganism => bestOrganism;

        public Hive(uint nb_explorer, uint nb_worker)
        {
            workers = new Worker[nb_worker];
            explorers = new Explorer[nb_explorer];
            
            Parallel.For(0, nb_worker, i => { workers[i] = new Worker(); });
            Parallel.For(0, nb_explorer, i => { explorers[i] = new Explorer(); });
            UpdateBestOrganism();
        }

        private void UpdateBestOrganism()
        {
            bestWorker = workers.Min();
            bestExplorer = explorers.Min();
            if (PSOSolver.verbose)
                Console.WriteLine("Best Worker: " + bestWorker.Error + " Best Explorer:" + bestExplorer.Error);
            if (bestOrganism == null || bestWorker.Error < bestOrganism.Error)
                bestOrganism = bestWorker;
            if (bestOrganism == null || bestExplorer.Error < bestOrganism.Error)
                bestOrganism = bestExplorer;
        }

        public SudokuGrid Solve(uint max_epoch = 1000, uint max_age = 1000)
        {
            // Start the search for a solution
            for (int i = 0; i < max_epoch; i++)
            {
                if (PSOSolver.verbose)
                    Console.WriteLine("Epoch: " + i + " Error: " + bestOrganism.Error);
                // If the best solution is perfect, stop the search
                if (bestOrganism.Error == 0)
                    break;
                
                Parallel.ForEach(explorers, organism => organism.SearchBetterSolution(max_age));
                Parallel.ForEach(workers, organism => organism.SearchBetterSolution(max_age));
                UpdateBestOrganism();
                workers.Max()?.Replace(bestWorker, bestExplorer);
            }
            return bestOrganism.Solution;
        }
    }
}