using GeneticSharp;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp;

/// <summary>
/// This more elaborated chromosome manipulates rows instead of cells, and each of its 9 gene holds an integer for the index of the row's permutation amongst all that respect the target mask.
/// Permutations are computed once when a new Sudoku is encountered, and stored in a static dictionary for further reference.
/// </summary>
public class SudokuPermutationsChromosome : SudokuChromosomeBase, ISudokuChromosome
{
	/// <summary>
	/// This constructor assumes no mask
	/// </summary>
	public SudokuPermutationsChromosome() : this(null) { }

	/// <summary>
	/// Constructor with a mask sudoku to solve, assuming a length of 9 genes
	/// </summary>
	/// <param name="targetSudokuGrid">the target sudoku to solve</param>
	public SudokuPermutationsChromosome(SudokuGrid targetSudokuGrid) : this(targetSudokuGrid, 9) { }

	/// <summary>
	/// Constructor with a mask and a number of genes
	/// </summary>
	/// <param name="targetSudokuGrid">the target sudoku to solve</param>
	/// <param name="length">the number of genes</param>
	public SudokuPermutationsChromosome(SudokuGrid targetSudokuGrid, int length) : this(targetSudokuGrid, null, length) { }

	/// <param name="targetSudokuGrid">the target sudoku to solve</param>
	/// <param name="extendedMask">The cell domains after initial constraint propagation</param>
	/// <param name="length">The number of genes for the sudoku chromosome</param>
	public SudokuPermutationsChromosome(SudokuGrid targetSudokuGrid,
		Dictionary<(int row, int col), List<int>> extendedMask, int length)
		: base(targetSudokuGrid, extendedMask, length)
	{
	}
	
	public SudokuPermutationsChromosome(
		SudokuGrid targetSudokuBoard,
		Dictionary<(int x, int y), List<int>> extendedMask,
		IList<(int row, int col)> objGeneToCellLookup,
		List<int> objBaseLookupTable)
		: base(targetSudokuBoard, extendedMask, 9)
	{
	}

	/// <summary>
	/// generates a chromosome gene from its index containing a random row permutation
	/// amongst those respecting the target mask. 
	/// </summary>
	/// <param name="geneIndex">the index for the gene</param>
	/// <returns>a gene generated for the index</returns>
	public override Gene GenerateGene(int geneIndex)
	{
		Gene toReturn = new Gene(PermutationsGenes[geneIndex]);
		return toReturn;
	}

	public override IChromosome CreateNew()
	{
		var toReturn = new SudokuPermutationsChromosome(TargetSudokuGrid, ExtendedMask, Length);
		return toReturn;
	}

	public override bool UsesPermutations()
	{
		return true;
	}

	/// <summary>
	/// builds a single Sudoku from the given row permutation genes
	/// </summary>
	/// <returns>a list with the single Sudoku built from the genes</returns>
	public override IList<SudokuGrid> GetSudokus()
	{
		var listInt = new List<int[]>(81);
		for (int i = 0; i < 9; i++)
		{
			var perm = GetPermutation(i).ToArray();
			listInt.Add(perm);
		}
		var sudoku = new SudokuGrid() { Cells = listInt.ToArray() };
		return new List<SudokuGrid>(new[] { sudoku });
	}


	/// <summary>
	/// Gets the permutation to apply from the index of the row concerned
	/// </summary>
	/// <param name="rowIndex">the index of the row to permute</param>
	/// <returns>the index of the permutation to apply</returns>
	protected virtual List<int> GetPermutation(int rowIndex)
	{
		int permIDx = GetPermutationIndex(rowIndex);
		return GetPermutation(rowIndex, permIDx);
	}

	/// <summary>
	/// Gets the permutation for a row and given a permutation index, according to the corresponding row's available permutations
	/// </summary>
	/// <param name="rowIndex">the row index for the permutation</param>
	/// <param name="permIDx">the permutation index to retrieve</param>
	/// <returns></returns>
	protected virtual List<int> GetPermutation(int rowIndex, int permIDx)
	{

		// we use a modulo operator in case the gene was swapped:
		// It may contain a number higher than the number of available permutations. 
		var perm = GetRowsPermutations()[rowIndex][permIDx % GetRowsPermutations()[rowIndex].Count].ToList();
		return perm;
	}


	/// <summary>
	/// Gets the permutation to apply from the index of the row concerned
	/// </summary>
	/// <param name="rowIndex">the index of the row to permute</param>
	/// <returns>the index of the permutation to apply</returns>
	protected virtual int GetPermutationIndex(int rowIndex)
	{
		return (int)GetGene(rowIndex).Value;
	}
}