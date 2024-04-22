namespace Sudoku.ParticleSwarmOptimization.TSPPSO {

    public class SwarmOptimizer
    {

        public Route BestGlobalItinery;

        public void AddInformersToParticle(List<Particle> informers)
        {
            foreach (Particle tspParticle in informers)
            {
                tspParticle.InformersList.Clear();
                tspParticle.InformersList.AddRange(informers);
                tspParticle.InformersList.Remove(tspParticle);
            }
        }

        public Particle[] BuildSwarm(RouteUpdater routeUpdater)
        {
            var swarm = new Particle[TSP.SwarmSize];
            for (int i = 0; i < TSP.SwarmSize; ++i)
            {
                swarm[i] = new Particle(new Route(TSPPSOSolver.original), routeUpdater);
            }
            int[] particleIndex = this.InitArray(TSP.SwarmSize);
            this.UpdateInformers(swarm, particleIndex, TSP.MaxInformers);
            return swarm;
        }

        public void UpdateInformers(Particle[] swarm, int[] particleIndex, int maxInformers)
        {
            Shuffler<int>.Shuffle(particleIndex);
            var informers = new List<Particle>();
            int informersCount = maxInformers + 1;
            for (int i = 1; i < particleIndex.Length + 1; i++)
            {
                informers.Add(swarm[particleIndex[i - 1]]);
                if (i % informersCount == 0)
                {
                    this.AddInformersToParticle(informers);
                    informers.Clear();
                }
            }
            //the number of informers added here
            //will be less than the informer count
            this.AddInformersToParticle(informers);
        }

        public int Optimize(Particle[] swarm)
        {
            this.BestGlobalItinery = swarm[0].CurrentRoute.CloneRoute();
            int[] particleIndex = this.InitArray(TSP.SwarmSize);
            int epoch = 0;
            int staticEpochs = 0;
            while (epoch < TSP.MaxEpochs)
            {
                bool isDistanceImproved = false;
                foreach (Particle particle in swarm)
                {
                    // distance = error in sudoku
                    int distance = particle.Optimize();
                    if (distance < this.BestGlobalItinery.Error)
                    {
                        particle.CurrentRoute.CopyTo(this.BestGlobalItinery);
                        isDistanceImproved = true;
                    }
                }
                if (!isDistanceImproved)
                {
                    staticEpochs++;
                    if (staticEpochs == TSP.MaxStaticEpochs)
                    {
                        this.UpdateInformers(swarm, particleIndex, TSP.MaxInformers);
                        staticEpochs = 0;
                    }
                }
                epoch++;
            }
            return this.BestGlobalItinery.Error;
        }

        private int[] InitArray(int size)
        {
            var arr = new int[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = i;
            }
            return arr;
        }
    }
}