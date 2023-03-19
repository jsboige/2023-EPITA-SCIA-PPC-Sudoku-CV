using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sudoku.Shared;

namespace Sudoku.ParticleSwarmOptimization
{
    public class Hive
    {
        private uint size;

        private Organism[] organisms;

        public Hive(uint size, uint nb_worker, SudokuGrid s)
        {
            this.size = size;
            organisms = new Organism[size];
            
            var result = Parallel.For(0, size, i => { organisms[i] = new Organism(s, i < nb_worker); });
            Organism minOrganism = organisms.Min();
            Console.WriteLine($"Lowest error: {minOrganism.Error}");
        }
    }
}