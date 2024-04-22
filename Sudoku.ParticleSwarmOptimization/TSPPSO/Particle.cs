namespace Sudoku.ParticleSwarmOptimization.TSPPSO {

    public class Particle
    {

        public Route CurrentRoute { get; set; }

        public Route LocalBestRoute { get; set; }

        public Route PersonalBestRoute { get; set; }

        public List<Particle> InformersList { get; set; }

        private RouteUpdater routeUpdater;

        public Particle(Route route, RouteUpdater routeUpdater)
        {
            CurrentRoute = route;
            PersonalBestRoute = route.CloneRoute();
            this.InformersList = new List<Particle>();
            this.routeUpdater = routeUpdater;
        }

        //updates a particle's velocity. The shorter the total distance the greater the velocity
        public double UpdateVelocity(Route particleItinery, double weighting, double randomDouble)
        {
            return (1.0 / (double)particleItinery.Error) * randomDouble * weighting;
        }

        //Selects a section of the route with a length  proportional to the particle's
        // relative velocity.
        public int GetSectionSize(double segmentVelocity, double totalVelocity)
        {
            return Convert.ToInt32(Math.Floor((segmentVelocity / totalVelocity) * 81));
        }


        public int[] AddSections(Route[] sections)
        {
            var rd = new Random();
            if (sections == null || sections.Length == 0)
            {
                throw new ArgumentException("Array cannot be null or empty", "sections");
            }
            if (!routeUpdater.Isinitialised)
            {
                routeUpdater.Initialise();
            }
            foreach (var routeSection in sections)
            {
                routeUpdater.AddSection(rd.Next(0, routeSection.Cells.Length), routeSection, rd.Next()%2==0);
            }
            return routeUpdater.FinalizeDestinationIndex(sections[0]);
        }

        public int[] GetOptimizedDestinationIndex(
            Route currRoute,
            Route pBRoute,
            Route lBRoute)
        {
            var r = new Random();
            //update all the velocities using the appropriate PSO constants
            double currV = UpdateVelocity(currRoute, TSP.W, 1);
            double pBV = UpdateVelocity(pBRoute, TSP.C1, r.NextDouble());
            double lBV = UpdateVelocity(lBRoute, TSP.C2, r.NextDouble());
            double totalVelocity = currV + pBV + lBV;

            //update the Segment size for each Route
            currRoute.SegmentSize = GetSectionSize(currV, totalVelocity);
            pBRoute.SegmentSize = GetSectionSize(pBV, totalVelocity);
            lBRoute.SegmentSize = GetSectionSize(lBV, totalVelocity);
            return AddSections(new[] { lBRoute, pBRoute, currRoute });
        }

        public int Optimize()
        {
            this.LocalBestRoute = this.GetLocalBestRoute();
            this.CurrentRoute.Cells = GetOptimizedDestinationIndex(
                    this.CurrentRoute,
                    this.PersonalBestRoute,
                    this.LocalBestRoute);

            int currentDistance = this.CurrentRoute.CalculateError();
            if (currentDistance < this.PersonalBestRoute.CalculateError())
            {
                this.CurrentRoute.Cells.CopyTo(this.PersonalBestRoute.Cells, 0);
            }
            return currentDistance;
        }
        
        private Route GetLocalBestRoute()
        {
            Route localBestRoute = this.PersonalBestRoute;
            foreach (Particle particle in this.InformersList)
            {
                if (localBestRoute.Error > particle.PersonalBestRoute.Error)
                {
                    localBestRoute = particle.PersonalBestRoute;
                }
            }
            return localBestRoute;
        }
    }

}
