using System.Linq;
using Sudoku.Shared;

namespace Sudoku.ColorGraph
{
    public class CGraph
    {
        private List<List<(int, int)>> AdjacencyList;

        public CGraph(SudokuGrid s)
        {
            AdjacencyList = new List<List<(int, int)>>();
            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    HashSet<(int, int)> subList = new HashSet<(int, int)>();
                    for (int cell = 0; cell < 9; cell++)
                    {
                        if (cell != j)
                            subList.Add((i * 9 + cell, s.Cells[i][cell]));
                        if (cell != i)
                            subList.Add((cell * 9 + j, s.Cells[cell][j]));
                    }
                    // insert in the hashset the cells in the same block
                    int iBlock = i / 3;
                    int jBlock = j / 3;
                    for (int a = 0; a < 3; a++)
                        for (int b = 0; b < 3; b++)
                            if (!(iBlock * 3 + a == i && jBlock * 3 + b == j))
                                subList.Add(((iBlock * 3 + a) * 9 + jBlock * 3 + b, s.Cells[iBlock * 3 + a][jBlock * 3 + b]));

                    AdjacencyList.Add(subList.ToList());
                }
            }
        }
    }
}