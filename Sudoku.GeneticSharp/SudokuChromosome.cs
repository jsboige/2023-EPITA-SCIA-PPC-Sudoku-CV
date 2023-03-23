using GeneticSharp;

namespace Sudoku.GeneticSharp;

public class SudokuChromosome : IChromosome
{
    public int CompareTo(IChromosome? other)
    {
        throw new NotImplementedException();
    }

    public Gene GenerateGene(int geneIndex)
    {
        throw new NotImplementedException();
    }

    public void ReplaceGene(int index, Gene gene)
    {
        throw new NotImplementedException();
    }

    public void ReplaceGenes(int startIndex, Gene[] genes)
    {
        throw new NotImplementedException();
    }

    public void Resize(int newLength)
    {
        throw new NotImplementedException();
    }

    public Gene GetGene(int index)
    {
        throw new NotImplementedException();
    }

    public Gene[] GetGenes()
    {
        throw new NotImplementedException();
    }

    public IChromosome CreateNew()
    {
        throw new NotImplementedException();
    }

    public IChromosome Clone()
    {
        throw new NotImplementedException();
    }

    public double? Fitness { get; set; }
    public int Length { get; }
}