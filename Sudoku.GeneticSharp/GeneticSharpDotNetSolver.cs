using GeneticSharp;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp
{
	public class GeneticPermutedCellsSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {


	        //Console.WriteLine("Enter population size:");
	        //string populationSize = Console.ReadLine();
	        //int.TryParse(populationSize, out int popSize);
	        //Console.WriteLine("Enter number of generations:");
	        //string genNb = Console.ReadLine();
	        //int.TryParse(genNb, out int genNbInt);


			var permutatedCellsChromosome = new SudokuPermutatedCellsChromosome(s);
            
            

            var popSize = 400;

			//var crossover = new PartiallyMappedCrossover();
			//var crossover = new CycleCrossover();

			var crossover = new OrderedCrossover();


			//var mutation = new PartialShuffleMutation();
			//var mutation = new DisplacementMutation();
			var mutation = new TworsMutation();

			var sdkBoard = SudokuTestHelper.Eval(permutatedCellsChromosome, crossover, mutation, s, popSize);

            return sdkBoard;

        }
    }



	public class GeneticPermutationsSolver : ISudokuSolver
	{
		public SudokuGrid Solve(SudokuGrid s)
		{

			var permutatedCellsChromosome = new SudokuPermutationsChromosome(s);

			
			var popSize = 400;
			var crossover = new UniformCrossover();
			var mutation = new UniformMutation();

			var sdkBoard = SudokuTestHelper.Eval(permutatedCellsChromosome, crossover, mutation, s, popSize);

			return sdkBoard;

		}
	}


	public class GeneticCellsSolver : ISudokuSolver
	{
		public SudokuGrid Solve(SudokuGrid s)
		{

			var permutatedCellsChromosome = new SudokuPermutationsChromosome(s);

		
			var popSize = 400;
			var crossover = new UniformCrossover();
			var mutation = new UniformMutation();

			var sdkBoard = SudokuTestHelper.Eval(permutatedCellsChromosome, crossover, mutation, s, popSize);

			return sdkBoard;

		}
	}


}