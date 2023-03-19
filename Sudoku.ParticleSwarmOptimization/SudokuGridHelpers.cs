using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization;

public static class SudokuGridHelpers
{
    public static int[] AvailableNumbersForColumn(SudokuGrid s, int i)
    {
        int[] histogram = {1,2,3,4,5,6,7,8,9};
        for (int j = 0; j < 9; ++j)
        {
            if (s.Cells[i][j] != 0)
            {
                histogram[j] = 0;
            }
        }

        return histogram;
    }
}