using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using GeneticSharp;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp
{
    public class SudokuPermutatedCellsChromosome : SudokuChromosomeBase, ISudokuChromosome
    {
	    private List<int> baseLookupTable = new List<int>(9) { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	    private List<int> cloneLookupTable = new List<int>(9) { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	    private IList<(int row, int col)>? _geneToCellLookup;

	    public SudokuPermutatedCellsChromosome()
            : this(null)
        {
        }

        public SudokuPermutatedCellsChromosome(SudokuGrid? targetSudokuBoard)
            : this(targetSudokuBoard, null)
        {
        }

        public SudokuPermutatedCellsChromosome(
	        SudokuGrid? targetSudokuBoard,
            Dictionary<(int x, int y), List<int>> extendedMask)
            : base(targetSudokuBoard, extendedMask, 81)
        {
        }

        public SudokuPermutatedCellsChromosome(
	        SudokuGrid targetSudokuBoard,
	        Dictionary<(int x, int y), List<int>> extendedMask,
	        List<int> objBaseLookupTable)
	        : base(targetSudokuBoard, extendedMask, 81)
        {
            this.baseLookupTable = objBaseLookupTable;
            this.cloneLookupTable = new List<int>(objBaseLookupTable);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            if (this.cloneLookupTable.Count == 0)
            {
	            this.cloneLookupTable = new List<int>(baseLookupTable);
            }
            
            var row = geneIndex / 9;
            var col = geneIndex % 9;
            
            Gene gene = new Gene(cloneLookupTable[Permutations[row][col] - 1] * 9 + Permutations[row][col] - 1);
            cloneLookupTable[Permutations[row][col] - 1] += 1;
		    
            return gene;
        }

        public override IChromosome CreateNew()
        {
	        return (IChromosome)new SudokuPermutatedCellsChromosome(this.TargetSudokuGrid, this.ExtendedMask, baseLookupTable);
        }

        public override bool UsesPermutations()
        {
	        return true;
        }

        public override IList<SudokuGrid> GetSudokus()
        {
	        var genes = GetGenes();
            var indices = Enumerable.Range(0, 9).ToList();
            var cellGrid = indices.Select(row => indices.Select(col => (int)genes[row * 9 + col].Value % 9 + 1).ToArray()).ToArray();
            var sudoku = new SudokuGrid() { Cells = cellGrid };
            return new List<SudokuGrid>(new[] { sudoku });
        }
    }
}