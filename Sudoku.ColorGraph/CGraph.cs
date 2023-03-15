using System.Linq;
using Sudoku.Shared;

namespace Sudoku.ColorGraph
{
    
    public class Pair
    {
        public int Item1;
        public int Item2;
        public Pair(int item1, int item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
       
    }
    
    public class CGraph
    {
        public List<List<Pair>> AdjacencyList;
        public List<Pair> nodes;
        public CGraph(SudokuGrid s)
        {
            nodes = new List<Pair>();
            for (int i = 0; i < 81 ; i++)
                nodes.Add(new Pair(i, 0));
            
            AdjacencyList = new List<List<Pair>>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    HashSet<Pair> subList = new HashSet<Pair>();
                    for (int cell = 0; cell < 9; cell++)
                    {
                        if (cell != j)
                        {
                            nodes[i * 9 + cell].Item2 = s.Cells[i][cell];
                            subList.Add(nodes[i * 9 + cell]);
                        }
                        if (cell != i)
                        {
                            nodes[cell * 9 + j].Item2 = s.Cells[cell][j];
                            subList.Add(nodes[cell * 9 + j]);
                        }
                    }
                    // insert in the hashset the cells in the same block
                    int iBlock = i / 3;
                    int jBlock = j / 3;
                    for (int a = 0; a < 3; a++)
                        for (int b = 0; b < 3; b++)
                            if (!(iBlock * 3 + a == i && jBlock * 3 + b == j))
                            {
                                nodes[(iBlock * 3 + a) * 9 + jBlock * 3 + b].Item2 = s.Cells[iBlock * 3 + a][jBlock * 3 + b];
                                subList.Add(nodes[(iBlock * 3 + a) * 9 + jBlock * 3 + b]);
                            }

                    AdjacencyList.Add(subList.ToList());
                }
            }
        }
    }
}