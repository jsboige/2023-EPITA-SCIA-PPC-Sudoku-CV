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
        
        public Hive(uint nb_explorer, uint nb_worker)
        {
            workers = new Worker[nb_worker];
            explorers = new Explorer[nb_explorer];
            
            Parallel.For(0, nb_worker, i => { workers[i] = new Worker(); });
            Parallel.For(0, nb_explorer, i => { explorers[i] = new Explorer(); });
            UpdateBestOrganism();
            Console.WriteLine($"Lowest error: {bestOrganism.Error}");
        }

        private void UpdateBestOrganism()
        {
            bestWorker = workers.Min();
            bestExplorer = explorers.Min();
            if (bestWorker.Error < bestExplorer.Error)
                bestOrganism = bestWorker;
            else
                bestOrganism = bestExplorer;
        }

        public SudokuGrid Solve()
        {
            for (int i = 0; i < 10000; i++)
            {
                if (bestOrganism.Error == 0)
                    break;
                Parallel.ForEach(explorers, organism => organism.SearchBetterSolution());
                Parallel.ForEach(workers, organism => organism.SearchBetterSolution());
                UpdateBestOrganism();
                workers.Max().Replace(bestWorker, bestExplorer);
            }
            return bestOrganism.Solution;
        }
    }
}