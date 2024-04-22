using GeneticSharp;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp;

/// <summary>
/// Each type of chromosome for solving a sudoku is simply required to output a list of candidate sudokus
/// </summary>
public interface ISudokuChromosome
{
	IList<SudokuGrid> GetSudokus();
}




/// <summary>
/// Evaluates a sudoku chromosome for completion by counting duplicates in rows, columns, boxes, and differences from the target mask
/// </summary>
public class SudokuFitness : IFitness
{
	/// <summary>
	/// The target Sudoku Mask to solve.
	/// </summary>
	private readonly SudokuGrid _targetSudokuGrid;

	public SudokuFitness(SudokuGrid targetSudokuGrid)
	{
		_targetSudokuGrid = targetSudokuGrid;
	}

	/// <summary>
	/// Evaluates a chromosome according to the IFitness interface. Simply reroutes to a typed version.
	/// </summary>
	/// <param name="chromosome"></param>
	/// <returns></returns>
	public double Evaluate(IChromosome chromosome)
	{
		return Evaluate((ISudokuChromosome)chromosome);
	}

	/// <summary>
	/// Evaluates a ISudokuChromosome by summing over the fitnesses of its corresponding Sudoku boards.
	/// </summary>
	/// <param name="chromosome">a Chromosome that can build Sudokus</param>
	/// <returns>the chromosome's fitness</returns>
	public double Evaluate(ISudokuChromosome chromosome)
	{
		List<double> scores = new List<double>();

		var sudokus = chromosome.GetSudokus();
		foreach (var sudoku in sudokus)
		{
			scores.Add(Evaluate(sudoku));
		}

		return scores.Sum();
	}

	/// <summary>
	/// Evaluates a single Sudoku board by counting the duplicates in rows, boxes
	/// and the digits differing from the target mask.
	/// </summary>
	/// <param name="testSudokuGrid">the board to evaluate</param>
	/// <returns>the number of mistakes the Sudoku contains.</returns>
	/// 
	public double Evaluate(SudokuGrid testSudokuGrid)
	{
		// We use a large lambda expression to count duplicates in rows, columns and boxes
		//var cells = testSudokuGrid.Cells.Select((c, i) => new { index = i, cell = c }).ToList();
		//var toTest = cells.GroupBy(x => x.index / 9).Select(g => g.Select(c => c.cell)) // rows
		//  .Concat(cells.GroupBy(x => x.index % 9).Select(g => g.Select(c => c.cell))) //columns
		//  .Concat(cells.GroupBy(x => x.index / 27 * 27 + x.index % 9 / 3 * 3).Select(g => g.Select(c => c.cell))); //boxes
		//var toReturn = -toTest.Sum(test => test.GroupBy(x => x).Select(g => g.Count() - 1).Sum()); // Summing over duplicates
		//toReturn -= cells.Count(x => _targetSudokuGrid.Cells[x.index] > 0 && _targetSudokuGrid.Cells[x.index] != x.cell); // Mask
		var toReturn = -testSudokuGrid.NbErrors(_targetSudokuGrid);
		return toReturn;
	}


}