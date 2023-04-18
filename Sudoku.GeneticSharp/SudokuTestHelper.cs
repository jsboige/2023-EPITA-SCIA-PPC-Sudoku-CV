using GeneticSharp;
using Sudoku.Shared;
using System;
using System.Diagnostics;
using System.Linq;

namespace Sudoku.GeneticSharp
{
    public enum SudokuTestDifficulty
    {
        VeryEasy,
        Easy,
        Medium
    }
    public static class SudokuTestHelper
    {
        private static readonly string _veryEasySudokuString =
            "9.2..54.31...63.255.84.7.6..263.9..1.57.1.29..9.67.53.24.53.6..7.52..3.4.8..4195.";

        private static readonly string _easySudokuString =
            "..48...1767.9.....5.8.3...43..74.1...69...78...1.69..51...8.3.6.....6.9124...15..";

        private static readonly string _mediumSudokuString =
            "..6.......8..542...4..9..7...79..3......8.4..6.....1..2.3.67981...5...4.478319562";


        public static SudokuGrid CreateBoard(SudokuTestDifficulty difficulty)
        {
            string sudokuToParse;
            switch (difficulty)
            {
                case SudokuTestDifficulty.VeryEasy:
                    sudokuToParse = _veryEasySudokuString;
                    break;
                case SudokuTestDifficulty.Easy:
                    sudokuToParse = _easySudokuString;
                    break;
                case SudokuTestDifficulty.Medium:
                    sudokuToParse = _mediumSudokuString;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }

            return SudokuGrid.ReadSudoku(sudokuToParse);
        }

        public static SudokuGrid Eval(IChromosome sudokuChromosome, ICrossover crossover, IMutation mutation, SudokuGrid sudokuBoard, int populationSize)
        {

	        var fitnessThreshold = 0;
            int stableGenerationNb = 20;
            
            SudokuFitness fitness = new SudokuFitness(sudokuBoard);
            EliteSelection selection = new EliteSelection();
            
            var termination = new OrTermination(new ITermination[]
            {
	            new FitnessThresholdTermination(fitnessThreshold),
	            new FitnessStagnationTermination(stableGenerationNb),
			});
            

			var nbErrors = 0;
			SudokuGrid bestSudoku;
			var sw = Stopwatch.StartNew();
			var lastTime = sw.Elapsed;
			do
            {
	            Population population = new Population(populationSize, populationSize, sudokuChromosome);

	            GeneticAlgorithm ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
	            {
		            Termination = termination
	            };
                //Ajout d'opérateurs de parallélisation
	            ga.OperatorsStrategy = new TplOperatorsStrategy();
	            ga.TaskExecutor = new TplTaskExecutor();
                ga.GenerationRan+=(sender, args) => 
                {
					var bestIndividual = (ISudokuChromosome)ga.Population.BestChromosome;
					var solutions = bestIndividual.GetSudokus();
					bestSudoku = solutions[0];
					nbErrors = bestSudoku.NbErrors(sudokuBoard);
                    Console.WriteLine($"Generation {ga.GenerationsNumber}, population {ga.Population.CurrentGeneration.Chromosomes.Count}, nbErrors {nbErrors} Elapsed {sw.Elapsed - lastTime} Elapsed since initial Gen {sw.Elapsed}");
                    lastTime = sw.Elapsed;
				};

	            ga.Start();
	            ISudokuChromosome bestIndividual = (ISudokuChromosome)ga.Population.BestChromosome;
	            IList<SudokuGrid> solutions = bestIndividual.GetSudokus();
	            bestSudoku = solutions[0];
				nbErrors = bestSudoku.NbErrors(sudokuBoard);
                populationSize *= 2;
            } while (nbErrors>0);
            
            return bestSudoku;
        }
    }
}