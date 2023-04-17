using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using GeneticSharp;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp
{
    public class SudokuOrderedCellsChromosome : SudokuChromosomeBase, ISudokuChromosome
    {
	    public IList<(int row, int col)> GeneToCellLookup
	    {
		    get
		    {
			    if (_geneToCellLookup == null)
			    {
					if (TargetSudokuGrid == null)
					{
						_geneToCellLookup = Enumerable.Range(0, 81).Select(x => (x / 9, x % 9)).ToList();
					}
					else
					{
						_geneToCellLookup = new List<(int row, int col)>(Length);
						for (int i = 0; i < 9; i++)
						{
							for (int j = 0; j < 9; j++)
							{
								if (TargetSudokuGrid.Cells[i][j] == 0)
								{
									GeneToCellLookup.Add((i, j));
								}
								else
								{
									// Add 1 to the lookup table for each number that is already in the board
									this.baseLookupTable[TargetSudokuGrid.Cells[i][j] - 1]++;
								}
							}
						}
					}
					this.cloneLookupTable = new List<int>(baseLookupTable);
				}
			    return _geneToCellLookup;
		    }
	    }

	    private List<int> baseLookupTable = new List<int>(9) { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
	    private List<int> cloneLookupTable;
	    private IList<(int row, int col)>? _geneToCellLookup;

	    public SudokuOrderedCellsChromosome()
            : this(null)
        {
        }

        public SudokuOrderedCellsChromosome(SudokuGrid? targetSudokuBoard)
            : this(targetSudokuBoard, null)
        {
        }

        public SudokuOrderedCellsChromosome(
	        SudokuGrid? targetSudokuBoard,
            Dictionary<(int x, int y), List<int>> extendedMask)
            : base(targetSudokuBoard, extendedMask, targetSudokuBoard?.NbEmptyCells?? 81)
        {
        }

        public SudokuOrderedCellsChromosome(
	        SudokuGrid targetSudokuBoard,
	        Dictionary<(int x, int y), List<int>> extendedMask,
	        IList<(int row, int col)> objGeneToCellLookup,
	        List<int> objBaseLookupTable)
	        : base(targetSudokuBoard, extendedMask, targetSudokuBoard?.NbEmptyCells ?? 81)
        {
            this._geneToCellLookup = objGeneToCellLookup;
            this.baseLookupTable = objBaseLookupTable;
            this.cloneLookupTable = new List<int>(objBaseLookupTable);
        }

        public static Random Random = new Random();

        public override Gene GenerateGene(int geneIndex)
        {
	        var targetCell = GeneToCellLookup[geneIndex];

	        var availableFromLookup = this.cloneLookupTable.Select(( value, ind) => (value, ind))
		        .Where((tuple => tuple.value < 9)).Select(tuple => tuple.ind+1).ToArray();
            var availableFromCoherence = this.ExtendedMask[(targetCell.row, targetCell.col)];
            var crossedAvail = availableFromCoherence.Where(i => availableFromLookup.Contains(i)).ToArray();
            int figureValue;
			if (crossedAvail.Length==0)
            {
	            // If no value is available from the mask and the lookuptable, we take the minimum value (index + 1) from the lookup table
				figureValue= this.cloneLookupTable.Select((value, ind) => (value, ind)).MinBy(tuple => tuple.value).ind + 1;
            }
            else
            {
	            // If there are values available from the mask and the lookuptable, we take a random value from the crossed list
				var figureIndex = Random.Next(crossedAvail.Count());
				figureValue = crossedAvail[figureIndex];
			}
           
            var geneValue = cloneLookupTable[figureValue-1]*9 + figureValue-1;
            cloneLookupTable[figureValue-1] += 1;

            Gene gene = new Gene(geneValue);
            return gene;
        }

        public override IChromosome CreateNew()
        {
	        return (IChromosome)new SudokuOrderedCellsChromosome(this.TargetSudokuGrid, this.ExtendedMask,
		        GeneToCellLookup, baseLookupTable);
        }

        public override bool UsesPermutations()
        {
	        return false;
        }

        public override IList<SudokuGrid> GetSudokus()
        {
            var toReturn = new List<SudokuGrid>();
            var computedSudoku = TargetSudokuGrid.CloneSudoku();
            toReturn.Add(computedSudoku);
            var genes = GetGenes();
            for (int geneIndex = 0; geneIndex < this.Length; geneIndex++)
            {
                var cellIndex = GeneToCellLookup[geneIndex];
				computedSudoku.Cells[cellIndex.row][cellIndex.col] = (int)genes[geneIndex].Value % 9 + 1 ;
			}
            return toReturn;
        }
    }
}