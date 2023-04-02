using System;
using System.Collections.Generic;
using GeneticSharp;
using GeneticSharp.Extensions;

namespace Sudoku.GeneticSharp
{
  public class MyPopulation : IPopulation
  {
    public event EventHandler BestChromosomeChanged;

    public MyPopulation(int minSize, int maxSize, IChromosome adamChromosome)
    {
      if (minSize < 2)
        throw new ArgumentOutOfRangeException(nameof(minSize), "The minimum size for a population is 2 chromosomes.");
      if (maxSize < minSize)
        throw new ArgumentOutOfRangeException(nameof(maxSize),
          "The maximum size for a population should be equal or greater than minimum size.");
      ExceptionHelper.ThrowIfNull(nameof(adamChromosome), (object)adamChromosome);
      this.CreationDate = DateTime.Now;
      this.MinSize = minSize;
      this.MaxSize = maxSize;
      this.AdamChromosome = adamChromosome;
      this.Generations = (IList<Generation>)new List<Generation>();
      this.GenerationStrategy = (IGenerationStrategy)new PerformanceGenerationStrategy(10);
    }

    public DateTime CreationDate { get; protected set; }

    public IList<Generation> Generations { get; protected set; }

    public Generation CurrentGeneration { get; protected set; }

    public int GenerationsNumber { get; protected set; }

    public int MinSize { get; set; }

    public int MaxSize { get; set; }

    public IChromosome BestChromosome { get; protected set; }

    public IGenerationStrategy GenerationStrategy { get; set; }

    protected IChromosome AdamChromosome { get; set; }

    public virtual void CreateInitialGeneration()
    {
      this.Generations = (IList<Generation>)new List<Generation>();
      this.GenerationsNumber = 0;
      List<IChromosome> chromosomes = new List<IChromosome>();
      for (int index = 0; index < this.MinSize; ++index)
      {
        IChromosome chromosome = this.AdamChromosome.CreateNew();
        if (chromosome == null)
          throw new InvalidOperationException(
            "The Adam chromosome's 'CreateNew' method generated a null chromosome. This is a invalid behavior, please, check your chromosome code.");
        chromosome.ValidateGenes();
        // print the values of the genes
        for (int i = 0; i < chromosome.Length; i++)
        {
          Console.Write(SudokuChromosome.moduloTable[(int)chromosome.GetGene(i).Value] + " ");
        }
        Console.WriteLine();
        Console.WriteLine();
        chromosomes.Add(chromosome);
      }

      this.CreateNewGeneration((IList<IChromosome>)chromosomes);
    }

    public virtual void CreateNewGeneration(IList<IChromosome> chromosomes)
    {
      ExceptionHelper.ThrowIfNull(nameof(chromosomes), (object)chromosomes);
      chromosomes.ValidateGenes();
      this.CurrentGeneration = new Generation(++this.GenerationsNumber, chromosomes);
      this.Generations.Add(this.CurrentGeneration);
      this.GenerationStrategy.RegisterNewGeneration((IPopulation)this);
    }

    public virtual void EndCurrentGeneration()
    {
      this.CurrentGeneration.End(this.MaxSize);
      if (this.BestChromosome != null && this.BestChromosome.CompareTo(this.CurrentGeneration.BestChromosome) == 0)
        return;
      this.BestChromosome = this.CurrentGeneration.BestChromosome;
      this.OnBestChromosomeChanged(EventArgs.Empty);
    }

    protected virtual void OnBestChromosomeChanged(EventArgs args)
    {
      EventHandler chromosomeChanged = this.BestChromosomeChanged;
      if (chromosomeChanged == null)
        return;
      chromosomeChanged((object)this, args);
    }
  }
}