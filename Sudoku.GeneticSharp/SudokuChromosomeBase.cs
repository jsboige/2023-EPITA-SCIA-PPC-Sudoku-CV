using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using GeneticSharp;
using Sudoku.Shared;

namespace Sudoku.GeneticSharp
{
	/// <summary>
	/// This abstract chromosome accounts for the target mask if given, and generates an extended mask with cell domains updated according to original mask
	/// </summary>

	public abstract class SudokuChromosomeBase : ChromosomeBase, ISudokuChromosome //
	{

		/// <summary>
		/// The target sudoku board to solve
		/// </summary>
		private readonly SudokuGrid? _targetSudokuGrid;

		/// <summary>
		/// The cell domains updated from the initial mask for the board to solve
		/// </summary>
		private Dictionary<(int row, int col), List<int>> _extendedMask;

		/// <summary>
		/// Constructor that accepts an additional extended mask for quick cloning
		/// </summary>
		/// <param name="targetSudokuGrid">the target sudoku to solve</param>
		/// <param name="extendedMask">The cell domains after initial constraint propagation</param>
		/// <param name="length">The number of genes for the sudoku chromosome</param>
		protected SudokuChromosomeBase(SudokuGrid? targetSudokuGrid, Dictionary<(int row, int col), List<int>> extendedMask, int length) : base(length)
		{
			_targetSudokuGrid = targetSudokuGrid;
			_extendedMask = extendedMask;
			CreateGenes();
		}

		/// <summary>
		/// The target sudoku board to solve
		/// </summary>
		public SudokuGrid? TargetSudokuGrid => _targetSudokuGrid;

		/// <summary>
		/// The cell domains updated from the initial mask for the board to solve
		/// </summary>
		public Dictionary<(int row, int col), List<int>> ExtendedMask
		{
			get
			{
				if (_extendedMask == null)
					BuildExtenedMask();

				return _extendedMask;
			}
		}

		private void BuildExtenedMask()
		{
			// We generate 1 to 9 figures for convenience
			var indices = Enumerable.Range(1, 9).ToList();
			var extendedMask = new Dictionary<(int row, int col), List<int>>(81);
			if (_targetSudokuGrid != null)
			{
				//If target sudoku mask is provided, we generate an inverted mask with forbidden values by propagating rows, columns and boxes constraints
				var forbiddenMask = new Dictionary<(int row, int col), List<int>>();
				List<int> targetList = null;
				for (var row = 0; row < 9; row++)
				{
					for (var col = 0; col < 9; col++)
					{
						int targetCell = _targetSudokuGrid.Cells[row][col];
						if (targetCell != 0)
						{
							//We parallelize going through all 3 constraint neighborhoods

							//var boxStartIdx = (index / 27 * 27) + (index % 9 / 3 * 3);
							var cellNeighBours = SudokuGrid.CellNeighbours[row][col];
							foreach (var cellNeighBour in cellNeighBours)
							{
								if (!forbiddenMask.TryGetValue(cellNeighBour, out targetList))
								{
									//If the current neighbor cell does not have a forbidden values list, we create it
									targetList = new List<int>();
									forbiddenMask[cellNeighBour] = targetList;
								}
								if (!targetList.Contains(targetCell))
								{
									// We add current cell value to the neighbor cell forbidden values
									targetList.Add(targetCell);
								}
							}


						}
					}


				}

				// We invert the forbidden values mask to obtain the cell permitted values domains

				for (var row = 0; row < 9; row++)
				{
					for (var col = 0; col < 9; col++)
					{
						var cellIndex = (row, col);

						extendedMask[cellIndex] = indices.Where(i => !forbiddenMask[cellIndex].Contains(i)).ToList();
					}
				}


			}
			else
			{
				//If we have no sudoku mask, 1-9 numbers are allowed for all cells

				for (var row = 0; row < 9; row++)
				{
					for (var col = 0; col < 9; col++)
					{
						var cellIndex = (row, col);

						extendedMask[cellIndex] = indices;
					}
				}

			}
			_extendedMask = extendedMask;
		}

		public abstract IList<SudokuGrid> GetSudokus();

	}
}