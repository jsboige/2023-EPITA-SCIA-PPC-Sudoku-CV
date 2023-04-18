using GeneticSharp;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp;

/// <summary>
/// This simple chromosome simply represents each cell by a gene with value between 1 and 9, accounting for the target mask if given
/// </summary>
/// 
//sudokuboard -> sudokugrid
public class SudokuCellsChromosome : SudokuChromosomeBase, ISudokuChromosome
{

	public SudokuCellsChromosome() : this(null)
	{
	}

	/// <summary>
	/// Basic constructor with target sudoku to solve
	/// </summary>
	/// <param name="targetSudokuGrid">the target sudoku to solve</param>
	public SudokuCellsChromosome(SudokuGrid targetSudokuGrid) : this(targetSudokuGrid, null) { }

	/// <summary>
	/// Constructor with additional precomputed domains for faster cloning
	/// </summary>
	/// <param name="targetSudokuGrid">the target sudoku to solve</param>
	/// <param name="extendedMask">The cell domains after initial constraint propagation</param>
	public SudokuCellsChromosome(SudokuGrid targetSudokuGrid, Dictionary<(int row, int col), List<int>> extendedMask) : base(targetSudokuGrid, extendedMask, 81)
	{
	}

	public override Gene GenerateGene(int geneIndex)
	{
		var row = geneIndex / 9;
		var col = geneIndex % 9;
		/*
		var cellIndex = (row, col);
		//If a target mask exist and has a digit for the cell, we use it.
		if (TargetSudokuGrid != null && TargetSudokuGrid.Cells[row][col] != 0)
		{
			return new Gene(TargetSudokuGrid.Cells[row][col]);
		}
		// otherwise we use a random digit amongst those permitted.
		var rnd = RandomizationProvider.Current;
		var targetIdx = rnd.GetInt(0, ExtendedMask[cellIndex].Count);
		return new Gene(ExtendedMask[cellIndex][targetIdx]);
		*/
		Gene toReturn = new Gene(Permutations[row][col]);
		return toReturn;
	}

	public override IChromosome CreateNew()
	{
		return new SudokuCellsChromosome(TargetSudokuGrid, ExtendedMask);
	}

	public override bool UsesPermutations()
	{
		return false;
	}

	/// <summary>
	/// Builds a single Sudoku from the 81 genes
	/// </summary>
	/// <returns>A Sudoku board built from the 81 genes</returns>
	public override IList<SudokuGrid> GetSudokus()
	{
		var genes = GetGenes();
		var indices = Enumerable.Range(0, 9).ToList();
		var cellGrid = indices.Select(row => indices.Select(col => (int)genes[row * 9 + col].Value).ToArray()).ToArray();
		var sudoku = new SudokuGrid() { Cells = cellGrid };
		return new List<SudokuGrid>(new[] { sudoku });
	}
}