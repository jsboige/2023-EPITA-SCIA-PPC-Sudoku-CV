namespace Sudoku.ParticleSwarmOptimization.TSPPSO;

public static class Shuffler<T>
{
    public static void Shuffle(T[] arr)
    {
        var rd = new Random();
        int i = arr.Length - 1;
        while (i > 0)
        {
            int k = rd.Next(i + 1);
            T temp = arr[i];
            arr[i] = arr[k];
            arr[k] = temp;
            i--;
        }


    }
}
