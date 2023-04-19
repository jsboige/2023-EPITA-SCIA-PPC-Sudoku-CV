using Sudoku.Shared;
using Sudoku.ParticleSwarmOptimization.TSPPSO;

namespace Sudoku.ParticleSwarmOptimization {

    public class TSP
    {
        public static int SwarmSize = 100;
        public static int MaxEpochs = 1000;
        public static int MaxStaticEpochs = 100;
        public static float W = 0.729f;
        public static float C1 = 1.49445f;
        public static float C2 = 1.49445f;
        public static int MaxInformers = 5;
        public TSP() {
        }

        public SudokuGrid Optimize() {
            ResultManager resultManager = new ResultManager();
            SwarmOptimizer swarmOptimizer = new SwarmOptimizer();
            RouteUpdater routeUpdater = new RouteUpdater();
            Particle[] swarm = swarmOptimizer.BuildSwarm(routeUpdater);
            for (int i = 0; i < 100; i++)
            {
                int distance = swarmOptimizer.Optimize(swarm);
                resultManager.UpdateResults(distance);
                if (TSPPSOSolver.verbose) {
                    System.Console.WriteLine("Epoch: " + i + " Distance: " + distance);
                    swarmOptimizer.BestGlobalItinery.Print();
                }
            }
            return swarmOptimizer.BestGlobalItinery.ToSudokuGrid();
        }
    }

}