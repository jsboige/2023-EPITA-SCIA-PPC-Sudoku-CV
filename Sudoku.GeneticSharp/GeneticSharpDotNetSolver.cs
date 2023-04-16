using GeneticSharp;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp
{
	public class GeneticOrderedCellsSolver : ISudokuSolver
	{
		public SudokuGrid Solve(SudokuGrid s)
		{
			var permutatedCellsChromosome = new SudokuOrderedCellsChromosome(s);
			
			var popSize = 400;

			// var crossover = new PartiallyMappedCrossover();
			var crossover = new CycleCrossover();
			// var crossover = new OrderedCrossover();
			// var crossover = new AlternatingPositionCrossover();
			// var crossover = new PositionBasedCrossover();
			
			
			//var mutation = new PartialShuffleMutation();
			//var mutation = new DisplacementMutation();
			var mutation = new TworsMutation();
			// var mutation = new ReverseSequenceMutation();
			// var mutation = new DisplacementMutation();

			var sdkBoard = SudokuTestHelper.Eval(permutatedCellsChromosome, crossover, mutation, s, popSize);

			return sdkBoard;
		}
	}
	public class GeneticPermutedCellsSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
	        var permutatedCellsChromosome = new SudokuPermutatedCellsChromosome(s);
			
			var popSize = 400;

			// var crossover = new PartiallyMappedCrossover();
			var crossover = new CycleCrossover();
			// var crossover = new OrderedCrossover();
			// var crossover = new AlternatingPositionCrossover();
			// var crossover = new PositionBasedCrossover();
			
			
			//var mutation = new PartialShuffleMutation();
			//var mutation = new DisplacementMutation();
			var mutation = new TworsMutation();
			// var mutation = new ReverseSequenceMutation();
			// var mutation = new DisplacementMutation();

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
			
			//var crossover = new CycleCrossover();
			// var crossover = new OrderedCrossover();
			var crossover = new UniformCrossover();
			
			// var mutation = new PartialShuffleMutation();
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
			// var crossover = new OrderedCrossover();
			
			var mutation = new UniformMutation();
			// var mutation = new PartialShuffleMutation();

			var sdkBoard = SudokuTestHelper.Eval(permutatedCellsChromosome, crossover, mutation, s, popSize);

			return sdkBoard;

		}
	}


}