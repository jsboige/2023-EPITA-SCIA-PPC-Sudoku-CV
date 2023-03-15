using Sudoku.Shared;

namespace Sudoku.ColorGraph
{
    public class ColorGraphDotNetSolver : ISudokuSolver
    {
        public SudokuGrid Solve(SudokuGrid s)
        {
            return Dsatur(new CGraph(s));
        }
        
        private HashSet<List<Pair>> GetUncoloredVertices(CGraph g)
        {
            var uncoloredVertices = new HashSet<List<Pair>>();
            foreach (var vertex in g.nodes)
            {
                if (vertex.Item2 == 0)
                {
                    uncoloredVertices.Add(g.AdjacencyList[vertex.Item1]);
                }
            }
            
            return uncoloredVertices;
        }
        
        // Solve Color Graph using Dsatur algorithm
        private SudokuGrid Dsatur(CGraph g)
        {
            var uncoloredVertices = GetUncoloredVertices(g);
            int cpt = 0;
            while (uncoloredVertices.Count > 0)
            {
                var maxSaturation = 0;
                var maxSaturationVertex = new List<Pair>();
                foreach (var vertex in uncoloredVertices)
                {
                    var saturation = GetSaturation(vertex);
                    if (saturation > maxSaturation)
                    {
                        maxSaturation = saturation;
                        maxSaturationVertex = vertex;
                    }
                }
                
                var maxDegree = 0;
                var maxDegreeVertex = new Pair(0, 0);
                foreach (var vertex in maxSaturationVertex)
                {
                    var degree = GetDegree(vertex, g);
                    if (degree > maxDegree)
                    {
                        maxDegree = degree;
                        maxDegreeVertex = vertex;
                    }
                }
                
                var color = 1;
                var colors = new HashSet<int>();
                foreach (var vertex in maxSaturationVertex)
                {
                    if (vertex.Item2 != 0)
                    {
                        colors.Add(vertex.Item2);
                    }
                }
                
                while (colors.Contains(color))
                {
                    color++;
                }
                
                maxDegreeVertex.Item2 = color;
                
                uncoloredVertices = GetUncoloredVertices(g);
                // print length of the uncolored vertices every 10000 iterations
                if (cpt % 10 == 0)
                {
                    System.Console.WriteLine(uncoloredVertices.Count);
                }

                cpt++;
            }
            
            var result = new SudokuGrid();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result.Cells[i][j] = g.nodes[i * 9 + j].Item2;
                }
                
            }
            
            
            
            return result;
        }
        
        private int GetDegree(Pair vertex, CGraph g)
        {
            var degree = 0;
            foreach (var v in g.AdjacencyList[vertex.Item1])
            {
                if (v.Item2 != 0)
                {
                    degree++;
                }
            }
            
            return degree;
        }
        
        private int GetSaturation(List<Pair> vertex)
        {
            var saturation = 0;
            foreach (var v in vertex)
            {
                if (v.Item2 != 0)
                {
                    saturation++;
                }
            }
            
            return saturation;
        }

        
    }
}

