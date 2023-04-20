using Sudoku.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Norvig
{
    public class NorvigArraySolver : ISudokuSolver
    {
        private static int[] digits = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        private static int[] rows = Enumerable.Range(0, 9).ToArray();
        private static int[] cols = digits;
        private List<(int, int)> squares { get; }
        private List<List<(int, int)>> unitlist { get; }
        private Dictionary<(int, int), List<List<(int, int)>>> units { get; }
        private Dictionary<(int, int), HashSet<(int, int)>> peers { get; }

        public NorvigArraySolver()
        {
            unitlist = new List<List<(int, int)>>();
            for (int i = 0; i < 9; i++)
            {
                unitlist.Add(Enumerable.Range(0, 9).Select(x => (i, x)).ToList());
                unitlist.Add(Enumerable.Range(0, 9).Select(x => (x, i)).ToList());
            }
            for (int i = 0; i < 9; i += 3)
            {
                for (int j = 0; j < 9; j += 3)
                {
                    unitlist.Add((from r in Enumerable.Range(i, 3)
                        from c in Enumerable.Range(j, 3)
                        select (r, c)).ToList());
                }
            }

            units = new Dictionary<(int, int), List<List<(int, int)>>>();
            foreach (int i in Enumerable.Range(0, 9))
            {
                foreach (int j in Enumerable.Range(0, 9))
                {
                    units[(i, j)] = unitlist.Where(u => u.Contains((i, j))).ToList();
                }
            }

            peers = new Dictionary<(int, int), HashSet<(int, int)>>();
            foreach (int i in Enumerable.Range(0, 9))
            {
                foreach (int j in Enumerable.Range(0, 9))
                {
                    peers[(i, j)] = new HashSet<(int, int)>(units[(i, j)].SelectMany(u => u).Where(u => u != (i, j)));
                }
            }
        }


        private List<(int, int)> Cross(int[] A, int[] B)
        {
            var result = new List<(int, int)>();
            foreach (int a in A)
            {
                foreach (int b in B)
                {
                    result.Add((a, b));
                }
            }
            return result;
        }

        public Dictionary<(int, int), int> GridValues(int[][] grid)
        {
            var dict = new Dictionary<(int, int), int>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    dict[(i, j)] = grid[i][j];
                }
            }
            return dict;
        }

        public Dictionary<(int, int), List<int>> ParseGrid(int[][] grid)
        {
            var values = new Dictionary<(int, int), List<int>>();

            // Initialize the values dictionary with the keys and the possible values.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    values[(i, j)] = new List<int>(Enumerable.Range(1, 9));
                }
            }

            // Iterate through the grid and assign the given values.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int cellValue = grid[i][j];
                    if (cellValue != 0)
                    {
                        if (Assign(values, (i, j), cellValue) == null)
                        {
                            return null;
                        }
                    }
                }
            }

            return values;
        }


        public Dictionary<(int, int), List<int>> Assign(Dictionary<(int, int), List<int>> values, (int, int) s, int d)
        {
            List<int> otherValues = new List<int>(values[s]);
            otherValues.Remove(d);

            foreach (int otherValue in otherValues)
            {
                if (Eliminate(values, s, otherValue) == null)
                {
                    return null;
                }
            }

            return values;
        }
  public Dictionary<(int, int), List<int>> Eliminate(Dictionary<(int, int), List<int>> values, (int, int) s, int d)
        {
            if (!values[s].Contains(d))
            {
                return values; // Already eliminated
            }

            values[s].Remove(d);

            if (values[s].Count == 0)
            {
                return null; // Contradiction: removed last value
            }
            else if (values[s].Count == 1)
            {
                int d2 = values[s][0];

                foreach (var s2 in peers[s])
                {
                    if (values[s2].Contains(d2) && Eliminate(values, s2, d2) == null)
                    {
                        return null;
                    }
                }
            }

            foreach (var u in units[s])
            {
                List<(int, int)> dPlaces = new List<(int, int)>();

                foreach (var s2 in u)
                {
                    if (values[s2].Contains(d))
                    {
                        dPlaces.Add(s2);
                    }
                }

                if (dPlaces.Count == 0)
                {
                    return null; // Contradiction: no place for this value
                }
                else if (dPlaces.Count == 1)
                {
                    if (Assign(values, dPlaces[0], d) == null)
                    {
                        return null;
                    }
                }
            }

            return values;
        }

  public Dictionary<(int, int), List<int>> Search(Dictionary<(int, int), List<int>> values)
  {
      if (values == null)
      {
          return null;
      }
      if (values.All(kv => kv.Value.Count == 1))
      {
          return values;
      }

      var unassignedSquares = values.Where(kv => kv.Value.Count > 1).ToList();
      var square = unassignedSquares.OrderBy(kv => kv.Value.Count).First().Key;
      var candidates = values[square];

      foreach (var value in candidates.ToList()) // Create a copy of the collection with ToList()
      {
          var newValues = Assign(new Dictionary<(int, int), List<int>>(values), square, value);
          var result = Search(newValues);
          if (result != null)
          {
              return result;
          }
      }

      return null;
  }



        private Dictionary<(int, int), List<int>> Some(IEnumerable<Dictionary<(int, int), List<int>>> seq)
        {
            foreach (var e in seq)
            {
                if (e != null)
                {
                    return e;
                }
            }

            return null;
        }

        public SudokuGrid Solve(SudokuGrid s)
        {
            Dictionary<(int, int), List<int>> values = Search(ParseGrid(s.Cells));

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    s.Cells[i][j] = values[(i, j)][0];
                }
            }

            return s;
        }
    }
}