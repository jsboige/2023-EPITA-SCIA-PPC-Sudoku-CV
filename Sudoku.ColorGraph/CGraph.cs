using System.Linq;
using Sudoku.Shared;

namespace Sudoku.ColorGraph
{
    public class CGraph
    {
        private List<List<int>> AdjacencyList = new List<List<int>>();

        public CGraph(SudokuGrid s)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var neighbors = SudokuGrid.AllNeighbours[i * j + j];
                    Console.WriteLine(neighbors);
                    // AdjacencyList.Add(neighbors.Select(n => n.row * 9 + n.column).ToList());
                }
            }
        }
    }
}