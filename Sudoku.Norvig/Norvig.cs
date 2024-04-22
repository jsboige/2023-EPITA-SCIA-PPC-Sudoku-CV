using Sudoku.Shared;

namespace Sudoku.Norvig;

public class NorvigSolver:ISudokuSolver
{
    
    private static string digits = "123456789";
    private static string rows = "ABCDEFGHI";
    private static string cols = digits;
    private List<string> squares { get; }
    private List<List<string>> unitlist { get; }
    private Dictionary<string, List<List<string>>> units { get; }
    private Dictionary<string, HashSet<string>> peers { get; }

    public NorvigSolver()
    {
        squares = Cross(rows, cols);
        unitlist = new List<List<string>>();
        foreach (char c in cols)
        {
            unitlist.Add(Cross(rows, c.ToString()));
        }
        foreach (char r in rows)
        {
            unitlist.Add(Cross(r.ToString(), cols));
        }
        foreach (var rs in new string[] { "ABC", "DEF", "GHI" })
        {
            foreach (var cs in new string[] { "123", "456", "789" })
            {
                unitlist.Add(Cross(rs, cs));
            }
        }
        units = new Dictionary<string, List<List<string>>>();
        foreach (string s in squares)
        {
            units[s] = unitlist.Where(u => u.Contains(s)).ToList();
        }
        peers = new Dictionary<string, HashSet<string>>();
        foreach (string s in squares)
        {
            peers[s] = new HashSet<string>(units[s].SelectMany(u => u).Where(u => u != s));
        }
    }
    public Dictionary<string, string> GridValues(int[][] grid)
    {

        var chars = new List<char>();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                var c = (char)('0' + grid[i][j]);
                chars.Add(c);
            }
        }

        return squares.Zip(chars, (s, c) => new { s, c })
            .ToDictionary(x => x.s, x => x.c.ToString());
    }
    public Dictionary<string, string> ParseGrid(int[][] grid)
    {
        var values = squares.ToDictionary(s => s, s => digits);

        foreach (var kvp in GridValues(grid))
        {
            if (digits.Contains(kvp.Value) && Assign(values, kvp.Key, kvp.Value) == null)
            {
                return null;
            }
        }

        return values;
    }


    private List<string> Cross(string A, string B)
    {
        var result = new List<string>();
        foreach (char a in A)
        {
            foreach (char b in B)
            {
                result.Add($"{a}{b}");
            }
        }
        return result;
    }
    
    public Dictionary<string, string> Assign(Dictionary<string, string> values, string s, string d)
    {
        var otherValues = values[s].Replace(d, "");

        foreach (var c in otherValues)
        {
            if (Eliminate(values, s, c.ToString()) == null)
            {
                return null;
            }
        }

        return values;
    }
    
    public Dictionary<string, string> Eliminate(Dictionary<string, string> values, string s, string d)
    {
        if (!values[s].Contains(d))
        {
            return values; // Already eliminated
        }

        values[s] = values[s].Replace(d, "");

        if (values[s].Length == 0)
        {
            return null; // Contradiction: removed last value
        }
        else if (values[s].Length == 1)
        {
            string d2 = values[s];

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
            List<string> dPlaces = new List<string>();

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
    private  Dictionary<string, string> Search(Dictionary<string, string> values)
    {
        if (values == null)
        {
            return null; // Failed earlier
        }

        if (squares.All(s => values[s].Length == 1))
        {
            return values; // Solved!
        }

        // Choose the unfilled square s with the fewest possibilities
        var minSquare = squares.Where(s => values[s].Length > 1).OrderBy(s => values[s].Length).First();

        return Some(values[minSquare].Select(d => 
            Search(Assign(new Dictionary<string, string>(values), minSquare, d.ToString()))));
    }

    private Dictionary<string, string> Some(IEnumerable<Dictionary<string, string>> seq)
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
        Dictionary<string, string> values = Search(ParseGrid(s.Cells));
        List<KeyValuePair<string, string>> values_list = values.ToList();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                s.Cells[i][j] = Int32.Parse(values_list[j + i * 9].Value);
            }   
        }
        return s;
    }
}