using System;
using System.Diagnostics;
using GeneticSharp;

namespace Sudoku.GeneticSharp
{
  [DebuggerDisplay("Fitness:{Fitness}, Genes:{Length}")]
  [Serializable]
  public abstract class ChromosomeBase : IChromosome, IComparable<IChromosome>
  {
    private Gene[] m_genes;
    private int m_length;

    protected ChromosomeBase(int length)
    {
      ChromosomeBase.ValidateLength(length);
      this.m_length = length;
      this.m_genes = new Gene[length];
    }

    public double? Fitness { get; set; }

    public int Length => this.m_length;

    public static bool operator ==(ChromosomeBase first, ChromosomeBase second)
    {
      if ((object) first == (object) second)
        return true;
      return (object) first != null && (object) second != null && first.CompareTo((IChromosome) second) == 0;
    }

    public static bool operator !=(ChromosomeBase first, ChromosomeBase second) => !(first == second);

    public static bool operator <(ChromosomeBase first, ChromosomeBase second)
    {
      if ((object) first == (object) second)
        return false;
      if ((object) first == null)
        return true;
      return (object) second != null && first.CompareTo((IChromosome) second) < 0;
    }

    public static bool operator >(ChromosomeBase first, ChromosomeBase second) => !(first == second) && !(first < second);

    public abstract Gene GenerateGene(int geneIndex);

    public abstract IChromosome CreateNew();

    public virtual IChromosome Clone()
    {
      IChromosome chromosome = this.CreateNew();
      chromosome.ReplaceGenes(0, this.GetGenes());
      chromosome.Fitness = this.Fitness;
      return chromosome;
    }

    public void ReplaceGene(int index, Gene gene)
    {
      if (index < 0 || index >= this.m_length)
        throw new ArgumentOutOfRangeException(nameof (index), "There is no Gene on index {0} to be replaced.".With((object) index));
      this.m_genes[index] = gene;
      this.Fitness = new double?();
    }

    public void ReplaceGenes(int startIndex, Gene[] genes)
    {
      ExceptionHelper.ThrowIfNull(nameof (genes), (object) genes);
      if (genes.Length == 0)
        return;
      if (startIndex < 0 || startIndex >= this.m_length)
        throw new ArgumentOutOfRangeException(nameof (startIndex), "There is no Gene on index {0} to be replaced.".With((object) startIndex));
      Array.Copy((Array) genes, 0, (Array) this.m_genes, startIndex, Math.Min(genes.Length, this.m_length - startIndex));
      this.Fitness = new double?();
    }

    public void Resize(int newLength)
    {
      ChromosomeBase.ValidateLength(newLength);
      Array.Resize<Gene>(ref this.m_genes, newLength);
      this.m_length = newLength;
    }

    public Gene GetGene(int index) => this.m_genes[index];

    public Gene[] GetGenes() => this.m_genes;

    public int CompareTo(IChromosome other)
    {
      if (other == null)
        return -1;
      double? fitness1 = other.Fitness;
      double? nullable1 = this.Fitness;
      double? nullable2 = fitness1;
      if (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue)
        return 0;
      double? fitness2 = this.Fitness;
      nullable1 = fitness1;
      return !(fitness2.GetValueOrDefault() > nullable1.GetValueOrDefault() & fitness2.HasValue & nullable1.HasValue) ? -1 : 1;
    }

    public override bool Equals(object obj) => obj is IChromosome other && this.CompareTo(other) == 0;

    public override int GetHashCode() => this.Fitness.GetHashCode();

    protected virtual void CreateGene(int index) => this.ReplaceGene(index, this.GenerateGene(index));

    protected virtual void CreateGenes()
    {
      for (int index = 0; index < this.Length; ++index)
        this.ReplaceGene(index, this.GenerateGene(index));
    }

    private static void ValidateLength(int length)
    {
      if (length < 2)
        throw new ArgumentException("The minimum length for a chromosome is 2 genes.", nameof (length));
    }
  }
}
