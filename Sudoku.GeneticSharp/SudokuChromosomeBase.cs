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

		public abstract bool UsesPermutations();

		/// <summary>
		/// The target sudoku board to solve
		/// </summary>
		private readonly SudokuGrid? _targetSudokuGrid;

		/// <summary>
		/// The cell domains updated from the initial mask for the board to solve
		/// </summary>
		private Dictionary<(int row, int col), List<int>> _extendedMask;
		
		/// <summary>
		/// The list of row permutations accounting for the mask
		/// </summary>
		private IList<IList<IList<int>>> _targetRowsPermutations;

		private IList<IList<IList<int>>> _RowsPermutations = new List<IList<IList<int>>>();
		
		private IList<int> _permutationsGenes = new List<int>();
		
		private IList<IList<int>> _permutations = new List<IList<int>>();

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
		
		public IList<int> PermutationsGenes => _permutationsGenes;
		
		public IList<IList<int>> Permutations => _permutations;
		
		public bool CheckPermutation(IList<int> permutation, int row)
		{
			var toReturn = true;
			for (int i = 0; i < 9; i++)
			{
				if (TargetSudokuGrid != null && TargetSudokuGrid.Cells[row][i] != 0)
				{
					if (permutation[i] != TargetSudokuGrid.Cells[row][i])
					{
						toReturn = false;
						break;
					}
				}
			}
			return toReturn;
		}
		
		protected override void CreateGenes()
		{
			var rnd = RandomizationProvider.Current;

			if (UsesPermutations())
			{
				for (int index = 0; index < 9; ++index)
				{
					IList<IList<int>> permutations = TargetRowsPermutations[index];
					IList<IList<int>> rowPermutations = _RowsPermutations[index];

					int[] permTargetIdxs = permutations.Where((perm) => CheckPermutation(perm, index)).Select((perm, idx) => idx).ToArray();
					int permIdx;
					if (permTargetIdxs.Length != 0)         // if at least one permutation respect the mask
					{
						permIdx = rowPermutations.IndexOf(permutations[permTargetIdxs[rnd.GetInt(0, permTargetIdxs.Length)]]);
					}
					else                                    // if no permutation respect the mask
					{
						permIdx = rnd.GetInt(0, rowPermutations.Count);
					}

					_permutationsGenes.Add(permIdx);
					_permutations.Add(rowPermutations[permIdx]);
				}
			}
			

			for (int index = 0; index < this.Length; ++index)
				this.ReplaceGene(index, this.GenerateGene(index));
		}

		/// <summary>
		/// This method computes for each row the list of digit permutations that respect the target mask, that is the list of valid rows discarding columns and boxes
		/// </summary>
		/// <param name="SudokuGrid">the target sudoku to account for</param>
		/// <returns>the list of permutations available</returns>
		public IList<IList<IList<int>>> GetRowsPermutations()
		{
			if (TargetSudokuGrid == null)
			{
				return UnfilteredPermutations;
			}

			// we store permutations to compute them once only for each target Sudoku
			if (!_rowsPermutations.TryGetValue(TargetSudokuGrid, out var toReturn))
			{
				// Since this is a static member we use a lock to prevent parallelism.
				// This should be computed once only.
				lock (_rowsPermutations)
				{
					if (!_rowsPermutations.TryGetValue(TargetSudokuGrid, out toReturn))
					{
						toReturn = GetRowsPermutationsUncached();
						_rowsPermutations[TargetSudokuGrid] = toReturn;
					}
				}
			}
			if (this._RowsPermutations.Count == 0)
				this._RowsPermutations = toReturn;
			return toReturn;
		}

		private IList<IList<IList<int>>> GetRowsPermutationsUncached()
		{
			var toReturn = new List<IList<IList<int>>>(9);
			for (int i = 0; i < 9; i++)
			{
				var tempList = new List<IList<int>>();
				foreach (var perm in AllPermutations)
				{
					// Permutation should be compatible with current row extended mask domains
					if (Range9.All(j => ExtendedMask[(i, j)].Contains(perm[j])))
					{
						tempList.Add(perm);
					}
				}
				toReturn.Add(tempList);
			}

			return toReturn;
		}


		/// <summary>
		/// Produces 9 copies of the complete list of permutations
		/// </summary>
		public static IList<IList<IList<int>>> UnfilteredPermutations
		{
			get
			{
				if (!_unfilteredPermutations.Any())
				{
					lock (_unfilteredPermutations)
					{
						if (!_unfilteredPermutations.Any())
						{
							_unfilteredPermutations = Range9.Select(i => AllPermutations).ToList();
						}
					}
				}
				return _unfilteredPermutations;
			}
		}

		/// <summary>
		/// Builds the complete list permutations for {1,2,3,4,5,6,7,8,9}
		/// </summary>
		public static IList<IList<int>> AllPermutations
		{
			get
			{
				if (!_allPermutations.Any())
				{
					lock (_allPermutations)
					{
						if (!_allPermutations.Any())
						{
							_allPermutations = GetPermutations(Enumerable.Range(1, 9), 9);
						}
					}
				}
				return _allPermutations;
			}
		}

		/// <summary>
		/// The list of row permutations accounting for the mask
		/// </summary>
		public IList<IList<IList<int>>> TargetRowsPermutations
		{
			get
			{
				if (_targetRowsPermutations == null)
				{
					_targetRowsPermutations = GetRowsPermutations();
				}
				return _targetRowsPermutations;
			}
		}

		/// <summary>
		/// The list of compatible permutations for a given Sudoku is stored in a static member for fast retrieval
		/// </summary>
		private static readonly IDictionary<SudokuGrid, IList<IList<IList<int>>>> _rowsPermutations = new Dictionary<SudokuGrid, IList<IList<IList<int>>>>();

		/// <summary>
		/// The list of row indexes is used many times and thus stored for quicker access.
		/// </summary>
		private static readonly List<int> Range9 = Enumerable.Range(0, 9).ToList();

		/// <summary>
		/// The complete list of unfiltered permutations is stored for quicker access
		/// </summary>
		private static IList<IList<int>> _allPermutations = (IList<IList<int>>)new List<IList<int>>();
		private static IList<IList<IList<int>>> _unfilteredPermutations = (IList<IList<IList<int>>>)new List<IList<IList<int>>>();

		/// <summary>
		/// Computes all possible permutation for a given set
		/// </summary>
		/// <typeparam name="T">the type of elements the set contains</typeparam>
		/// <param name="list">the list of elements to use in permutations</param>
		/// <param name="length">the size of the resulting list with permuted elements</param>
		/// <returns>a list of all permutations for given size as lists of elements.</returns>
		static IList<IList<T>> GetPermutations<T>(IEnumerable<T> list, int length)
		{
			if (length == 1) return list.Select(t => (IList<T>)(new T[] { t }.ToList())).ToList();

			var enumeratedList = list.ToList();
			return (IList<IList<T>>)GetPermutations(enumeratedList, length - 1)
				.SelectMany(t => enumeratedList.Where(e => !t.Contains(e)),
					(t1, t2) => (IList<T>)t1.Concat(new T[] { t2 }).ToList()).ToList();
		}

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